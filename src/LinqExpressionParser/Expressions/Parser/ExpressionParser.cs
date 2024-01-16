#pragma warning disable CA2254 // 템플릿은 정적 표현식이어야 합니다.

using LinqExpressionParser.Expressions.Exceptions;
using LinqExpressionParser.Segments;
using LinqExpressionParser.Segments.Enums;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using static LinqExpressionParser.Expressions.Maps.Methods.MethodMapOptions;
using static LinqExpressionParser.Expressions.Maps.Operators.OperatorMapOptions;

namespace LinqExpressionParser.Expressions.Parser
{
    public class ExpressionParser(ILogger<ExpressionParser> logger, IOptions<ExpressionParserOptions> options) : IExpressionParser
    {
        private readonly ExpressionParserOptions _options = options.Value;

        public Expression<Func<T, V>> ParseValueExpression<T, V>(ValueSegment value)
        {
            ParameterExpression parameterExp = Expression.Parameter(typeof(T));
            ParameterCollection parameters = new(parameterExp);

            Expression valueExp;
            try
            {
                valueExp = GetValueExpression(parameters, value);
            }
            catch (ExpressionParseExceptionBase e)
            {
                logger.LogDebug(e, null);
                throw;
            }
            catch (Exception e)
            {
                logger.LogDebug(e, null);
                throw new UndefinedExpressionParseException(e);
            }

            if (!typeof(V).IsAssignableFrom(valueExp.Type))
            {
                InvalidBodyTypeException exception = new(typeof(V), valueExp.Type, value, valueExp);
                logger.LogDebug(exception, null);
                throw exception;
            }

            Expression<Func<T, V>> lambda = Expression.Lambda<Func<T, V>>(valueExp, parameterExp);
            return lambda;
        }

        public LambdaExpression ParseSelectorExpression<T>(SelectorSegment selector)
        {
            ParameterExpression parameterExp = Expression.Parameter(typeof(T));
            ParameterCollection parameters = new(parameterExp);

            MemberInitExpression selectorBodyExp;
            Dictionary<string, Expression> selectedItemExps = new(selector.SelectedItems.Count);
            try
            {
                foreach (var selectedItem in selector.SelectedItems)
                {
                    string name = selectedItem.Key;
                    Expression exp = GetValueExpression(parameters, selectedItem.Value);
                    selectedItemExps.Add(name, exp);
                }

                selectorBodyExp = GetAnonymousInitExpression(selectedItemExps);
            }
            catch (ExpressionParseExceptionBase e)
            {
                logger.LogDebug(e, null);
                throw;
            }
            catch (Exception e)
            {
                logger.LogDebug(e, null);
                throw new UndefinedExpressionParseException(e);
            }

            LambdaExpression lambda = Expression.Lambda(selectorBodyExp, parameterExp);
            return lambda;
        }

        private Expression GetValueExpression(ParameterCollection parameters, ValueSegment value)
        {
            Expression valueExp = value switch
            {
                PropertySegment p => GetPropertyExpression(parameters, p),
                LambdaMethodSegment l => GetLambdaMethodCallExpression(parameters, l),
                MethodSegment f => GetMethodCallExpression(parameters, f),
                ConstantSegment c => GetConstantExpression(c),
                OperationSegment o => GetOperationExpression(parameters, o),
                _ => throw new ArgumentException($"Wrong segment type. type: {value.GetType().Name}", nameof(value))
            };

            return valueExp;
        }

        private Expression GetLambdaMethodCallExpression(ParameterCollection parameters, LambdaMethodSegment lambda)
        {
            string methodName = lambda.Name;
            if (!_options.MethodMapOptions.LambdaMethodMap.TryGetValue(methodName, out List<GetLambdaMethodCallExpressionDelegate>? lambdaMethodCallExpressionDelegates))
            {
                throw new LambdaMethodNotMappedException(methodName);
            }

            LambdaParameterDeclaringSegment parameterDeclaringSegment = lambda.ParameterDeclaringSegment;
            Expression sourceExp = GetPropertyExpression(parameters, parameterDeclaringSegment.Property);

            Type propertyIEnumerableType = sourceExp.Type.GetInterface("IEnumerable`1") ?? throw new InvalidLambdaSourceException(sourceExp, parameterDeclaringSegment);
            Type elementType = propertyIEnumerableType.GenericTypeArguments[0];

            ParameterExpression parameterExp = Expression.Parameter(elementType);
            string prefix = parameterDeclaringSegment.Prefix;

            List<Expression> argExps = new(lambda.Arguments.Count + 1) { sourceExp };

            using (IDisposable prefixScope = parameters.CreateParameterScope(prefix, parameterExp))
            {
                foreach (var argument in lambda.Arguments)
                {
                    Expression argExp = GetValueExpression(parameters, argument);
                    argExps.Add(argExp);
                }
            }

            Expression? result = null;
            IEnumerator<GetLambdaMethodCallExpressionDelegate> delegateEnumerator = lambdaMethodCallExpressionDelegates.GetEnumerator();
            while (delegateEnumerator.MoveNext() && result is null)
            {
                result = delegateEnumerator.Current(parameterExp, argExps);
            }

            if (result is null)
            {
                throw new LambdaMethodNotMappedException(methodName, argExps.Select(a => a.Type));
            }

            return result;
        }

        private Expression GetMethodCallExpression(ParameterCollection parameters, MethodSegment method)
        {
            string methodName = method.Name;
            if (!_options.MethodMapOptions.MethodMap.TryGetValue(methodName, out List<GetMethodCallExpressionDelegate>? methodCallExpressionDelegates))
            {
                throw new MethodNotMappedException(methodName);
            }

            List<Expression> argExps = new(method.Arguments.Count);
            foreach (var argument in method.Arguments)
            {
                Expression argExp = GetValueExpression(parameters, argument);
                argExps.Add(argExp);
            }

            Expression? result = null;
            IEnumerator<GetMethodCallExpressionDelegate> delegateEnumerator = methodCallExpressionDelegates.GetEnumerator();
            while (delegateEnumerator.MoveNext() && result is null)
            {
                result = delegateEnumerator.Current(argExps);
            }

            if (result is null)
            {
                throw new MethodNotMappedException(methodName, argExps.Select(a => a.Type));
            }

            return result;
        }

        private static Expression GetPropertyExpression(ParameterCollection parameters, PropertySegment property)
        {
            PropertySegment? segment = property;
            Expression result = parameters.GetOrNull(segment.Name)!;

            if (result is null)
            {
                result = parameters.GetDefault();
            }
            else
            {
                segment = segment.Child ?? throw new PrefixChildNotExistException(segment.Name, (ParameterExpression)result);
            }

            PropertyInfo? propertyInfo;
            while (segment is not null)
            {
                propertyInfo = result.Type.GetProperty(segment.Name, BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo is null)
                {
                    throw new PropertyNotExistException(result.Type, segment.Name);
                }

                segment = segment.Child;
                result = Expression.Property(result, propertyInfo);
            }

            return result;
        }

        private static ConstantExpression GetConstantExpression(ConstantSegment constant)
        {
            return Expression.Constant(constant.Value, constant.ConstantType switch
            {
                EConstantType.String => typeof(string),
                EConstantType.Int => typeof(int),
                EConstantType.Double => typeof(double),
                EConstantType.Bool => typeof(bool),
                EConstantType.Null => typeof(object),
                _ => throw new InvalidEnumArgumentException(nameof(constant.ConstantType), (int)constant.ConstantType, typeof(EConstantType))
            });
        }

        private Expression GetOperationExpression(ParameterCollection parameters, OperationSegment operation)
        {
            Stack<Expression> expressionStack = new();
            List<SegmentBase> prefixSegments = InfixToPrefix(operation.Segments);

            foreach (var segment in prefixSegments.AsEnumerable().Reverse())
            {
                if (segment is SelectorSegment or ParenthesesSegment)
                {
                    throw new InvalidOperationSegmentsException(operation.Segments, EInvalidSegmentError.WrongSegmentType);
                }

                logger.LogDebug("Start parse segment. Type: {segmentType}", segment.GetType().FullName);

                Expression parsedExpression;
                switch (segment)
                {
                    case OperatorSegment o:
                        if (!expressionStack.TryPop(out Expression? left) || !expressionStack.TryPop(out Expression? right))
                        {
                            throw new InvalidOperationSegmentsException(operation.Segments, EInvalidSegmentError.WrongOperatorCount);
                        }
                        parsedExpression = GetOperatorExpression(o.Operator, left, right);
                        break;

                    case MethodSegment f:
                        parsedExpression = GetMethodCallExpression(parameters, f);
                        break;

                    case LambdaMethodSegment l:
                        parsedExpression = GetLambdaMethodCallExpression(parameters, l);
                        break;

                    case PropertySegment p:
                        parsedExpression = GetPropertyExpression(parameters, p);
                        break;

                    case ConstantSegment c:
                        parsedExpression = GetConstantExpression(c);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                expressionStack.Push(parsedExpression);
            }

            if (expressionStack.Count > 1)
            {
                throw new InvalidOperationSegmentsException(operation.Segments, EInvalidSegmentError.WrongOperandCount);
            }

            return expressionStack.Pop();
        }

        private Expression GetOperatorExpression(EOperator @operator, Expression left, Expression right)
        {
            if (!_options.OperatorMapOptions.OperatorMaps.TryGetValue(@operator, out IEnumerable<GetOperatorExpressionDelegate>? getOperatorExpressionDelegates))
            {
                throw new OperatorNotMappedException(@operator);
            }

            Expression? result = null;
            IEnumerator<GetOperatorExpressionDelegate> delegateEnumerator = getOperatorExpressionDelegates.GetEnumerator();
            while (delegateEnumerator.MoveNext() && result is null)
            {
                result = delegateEnumerator.Current(left, right);
            }

            if (result is null)
            {
                throw new OperatorNotMappedException(@operator, left.Type, right.Type);
            }

            return result;
        }

        private static List<SegmentBase> InfixToPrefix(IEnumerable<SegmentBase> infixSegments)
        {
            IEnumerable<SegmentBase> reversedInfix = infixSegments.Reverse();
            List<SegmentBase> postfix = new(infixSegments.Count());
            Stack<OperatorSegment> operatorStack = new();
            InfixToPostFixRecursive(postfix, operatorStack, reversedInfix);
            while (operatorStack.Count > 0)
            {
                postfix.Add(operatorStack.Pop());
            }

            postfix.Reverse();
            return postfix;
        }

        private static void InfixToPostFixRecursive(List<SegmentBase> result, Stack<OperatorSegment> stack, IEnumerable<SegmentBase> segments)
        {
            foreach (var segment in segments)
            {
                switch (segment)
                {
                    case OperatorSegment o:
                        int priority = GetOperatorPriority(o);
                        while (stack.Count > 0 && priority >= GetOperatorPriority(stack.Peek()))
                        {
                            result.Add(stack.Pop());
                        }
                        stack.Push(o);
                        break;

                    case ParenthesesSegment p:
                        if (p.Value is not OperationSegment operation)
                        {
                            result.Add(p.Value);
                            break;
                        }

                        IEnumerable<SegmentBase> reversedSegments = operation.Segments.Reverse();
                        Stack<OperatorSegment> parenthesesStack = new();
                        InfixToPostFixRecursive(result, parenthesesStack, reversedSegments);
                        while (parenthesesStack.Count > 0)
                        {
                            result.Add(parenthesesStack.Pop());
                        }

                        break;

                    default:
                        result.Add(segment);
                        break;
                }
            }
        }

        private static int GetOperatorPriority(OperatorSegment segment)
        {
            return segment.Operator switch
            {
                EOperator.Multiply or EOperator.Divide => 0,
                EOperator.Add or EOperator.Subtract => 1,
                >= EOperator.EQ and <= EOperator.LTE => 2,
                EOperator.And => 3,
                EOperator.Or => 4,
                _ => throw new InvalidEnumArgumentException(nameof(OperatorSegment.Operator), (int)segment.Operator, typeof(EOperator))
            };
        }

        private static MemberInitExpression GetAnonymousInitExpression(Dictionary<string, Expression> propertyAssignments)
        {
            AssemblyName assemblyName = new("_<>tempAssembly");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("_<>tempModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("_<>AnonymousType", TypeAttributes.Public);

            foreach (var assignment in propertyAssignments)
            {
                Type type = assignment.Value.Type;
                string name = assignment.Key;

                FieldBuilder fieldBuilder = typeBuilder.DefineField($"_{name}", type, FieldAttributes.Private);
                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.None, type, null);

                MethodBuilder getterBuilder = typeBuilder.DefineMethod($"get_{name}", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, type, null);
                ILGenerator getterIL = getterBuilder.GetILGenerator();
                getterIL.Emit(OpCodes.Ldarg_0);
                getterIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getterIL.Emit(OpCodes.Ret);

                MethodBuilder setterBuilder = typeBuilder.DefineMethod($"set_{name}", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, null, [type]);
                ILGenerator setterIL = setterBuilder.GetILGenerator();
                setterIL.Emit(OpCodes.Ldarg_0);
                setterIL.Emit(OpCodes.Ldarg_1);
                setterIL.Emit(OpCodes.Stfld, fieldBuilder);
                setterIL.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getterBuilder);
                propertyBuilder.SetSetMethod(setterBuilder);
            }

            Type anonymousType = typeBuilder.CreateType();
            NewExpression newExp = Expression.New(anonymousType);

            IEnumerable<MemberAssignment> assignments = propertyAssignments.Select(set => Expression.Bind(anonymousType.GetProperty(set.Key)!, set.Value));
            MemberInitExpression initExpression = Expression.MemberInit(newExp, assignments);

            return initExpression;
        }
    }
}