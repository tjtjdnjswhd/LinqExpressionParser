using LinqExpressionParser.Segments.Enums;

using System.Linq.Expressions;

using ValueParserTestGenerator.Codes;

namespace LinqExpressionParser.Tests.Expressions.Tests
{
    [ValueParserTestClass("OperationTest", nameof(TestAssert))]
    public partial class OperationTests : ExpressionParserTestBase
    {
        private static readonly ValueParserTestData<User, double> ConstantMultiply;
        private static readonly ValueParserTestData<User, int> UserRoleIdAdd;
        private static readonly ValueParserTestData<User, int> UserRoleIdSubtract;
        private static readonly ValueParserTestData<User, bool> UserEQAnd;
        private static readonly ValueParserTestData<OrderSet, bool> OrderSetMessageEQNull;

        protected override VerifySettings VerifySettings { get; }

        public OperationTests()
        {
            VerifySettings = new();
            VerifySettings.UseDirectory("OperationTests");
        }

        static OperationTests()
        {
            //ConstantAdd
            {
                ValueSegment segment = new OperationSegment(new ConstantSegment(3.0), new OperatorSegment(EOperator.Multiply), new ConstantSegment(12));
                Expression<Func<User, double>> exp = u => 3.0 * 12;
                ConstantMultiply = new ValueParserTestData<User, double>(exp, segment);
            }

            //UserRoleIdAdd
            {
                ValueSegment segment = new OperationSegment(new PropertySegment(nameof(User.RoleId)), new OperatorSegment(EOperator.Add), new ConstantSegment(3));
                Expression<Func<User, int>> exp = u => u.RoleId + 3;
                UserRoleIdAdd = new ValueParserTestData<User, int>(exp, segment);
            }

            //UserRoleIdSubtract
            {
                ValueSegment segment = new OperationSegment(new PropertySegment(nameof(User.RoleId)), new OperatorSegment(EOperator.Subtract), new ConstantSegment(3));
                Expression<Func<User, int>> exp = u => u.RoleId - 3;
                UserRoleIdSubtract = new ValueParserTestData<User, int>(exp, segment);
            }

            //UserEQAnd
            {
                ValueSegment segment = new OperationSegment(
                    new PropertySegment(nameof(User.Id)),
                    new OperatorSegment(EOperator.EQ),
                    new ConstantSegment(1),
                    new OperatorSegment(EOperator.And),
                    new PropertySegment(nameof(User.IsBlocked)));
                Expression<Func<User, bool>> exp = u => u.Id == 1 && u.IsBlocked == true;
                UserEQAnd = new ValueParserTestData<User, bool>(exp, segment);
            }

            //OrderSetMessageEQNull
            {
                ValueSegment segment = new OperationSegment(
                    new PropertySegment(nameof(OrderSet.Message)), new OperatorSegment(EOperator.EQ), new ConstantSegment());
                Expression<Func<OrderSet, bool>> exp = o => o.Message == null;
                OrderSetMessageEQNull = new ValueParserTestData<OrderSet, bool>(exp, segment);
            }
        }
    }
}