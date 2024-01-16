using LinqExpressionParser.Expressions.Exceptions;

using System.Linq.Expressions;

namespace LinqExpressionParser.Expressions
{
    internal class ParameterCollection
    {
        private readonly Dictionary<string, ParameterExpression> _collection = [];

        public ParameterCollection(ParameterExpression defaultParameter)
        {
            _collection.Add(string.Empty, defaultParameter);
        }

        public ParameterExpression GetDefault()
        {
            return _collection[string.Empty];
        }

        public ParameterExpression? GetOrNull(string prefix)
        {
            return _collection.TryGetValue(prefix, out ParameterExpression? value) ? value : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="parameterExp"></param>
        /// <returns></returns>
        /// <exception cref="PrefixDuplicateException"></exception>
        public IDisposable CreateParameterScope(string prefix, ParameterExpression parameterExp)
        {
            if (!_collection.TryAdd(prefix, parameterExp))
            {
                ParameterExpression firstParameter = _collection[prefix];
                throw new PrefixDuplicateException(prefix, firstParameter, parameterExp);
            }

            return new ParameterScope(this, prefix);
        }

        private bool Remove(string prefix) => _collection.Remove(prefix);

        private class ParameterScope(ParameterCollection collection, string prefix) : IDisposable
        {
            public void Dispose()
            {
                collection.Remove(prefix);
            }
        }
    }
}