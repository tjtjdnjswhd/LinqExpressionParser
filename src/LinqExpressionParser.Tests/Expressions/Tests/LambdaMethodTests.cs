using LinqExpressionParser.Segments.Enums;

using System.Linq.Expressions;

using ValueParserTestGenerator.Codes;

namespace LinqExpressionParser.Tests.Expressions.Tests
{
    [ValueParserTestClass("LambdaMethodTest", nameof(TestAssert))]
    public partial class LambdaMethodTests : ExpressionParserTestBase
    {
        private static readonly ValueParserTestData<User, double> UserSumOrderSetsAvgOrdersPricePerItem;
        private static readonly ValueParserTestData<User, int> UserMaxReviewsRating;
        private static readonly ValueParserTestData<User, int> UserMinReviewsRating;
        private static readonly ValueParserTestData<User, int> UserSumPointsBalance;
        private static readonly ValueParserTestData<User, double> UserAveragePointsBalance;
        private static readonly ValueParserTestData<User, bool> UserAnyReviews;
        private static readonly ValueParserTestData<User, bool> UserAnyReviewsGTOne;
        private static readonly ValueParserTestData<User, bool> UserAllReviewsGTOne;
        private static readonly ValueParserTestData<User, int> UserCountReviews;
        private static readonly ValueParserTestData<User, int> UserCountReviewsGTOne;

        protected override VerifySettings VerifySettings { get; }

        public LambdaMethodTests()
        {
            VerifySettings = new();
            VerifySettings.UseDirectory("LambdaMethodTests");
        }

        static LambdaMethodTests()
        {
            //Max
            {
                ValueSegment segment = new LambdaMethodSegment(
                    name: "Max",
                    parameterDeclearingSegment: new LambdaParameterDeclaringSegment(
                        prefix: "r",
                        property: new PropertySegment(
                            name: nameof(User.Reviews))),
                   arguments: new PropertySegment("r", new PropertySegment(nameof(Review.Rating))));

                Expression<Func<User, int>> exp = u => u.Reviews.Max(r => r.Rating);
                UserMaxReviewsRating = new(exp, segment);
            }

            //Min
            {
                ValueSegment segment = new LambdaMethodSegment(
                    name: "Min",
                    parameterDeclearingSegment: new LambdaParameterDeclaringSegment(
                        prefix: "r",
                        property: new PropertySegment(
                            name: nameof(User.Reviews))),
                   arguments: new PropertySegment("r", new PropertySegment(nameof(Review.Rating))));

                Expression<Func<User, int>> exp = u => u.Reviews.Min(r => r.Rating);
                UserMinReviewsRating = new ValueParserTestData<User, int>(exp, segment);
            }

            //Sum
            {
                ValueSegment segment = new LambdaMethodSegment(
                    name: "Sum",
                    parameterDeclearingSegment: new LambdaParameterDeclaringSegment(
                        prefix: "p",
                        property: new PropertySegment(nameof(User.Points))),
                    arguments: new PropertySegment("p", new PropertySegment(nameof(Point.Balance))));

                Expression<Func<User, int>> exp = u => u.Points.Sum(p => p.Balance);
                UserSumPointsBalance = new ValueParserTestData<User, int>(exp, segment);
            }

            //Average
            {
                ValueSegment segment = new LambdaMethodSegment(
                    name: "Avg",
                    parameterDeclearingSegment: new LambdaParameterDeclaringSegment(
                        prefix: "p",
                        property: new PropertySegment(nameof(User.Points))),
                    arguments: new PropertySegment("p", new PropertySegment(nameof(Point.Balance))));

                Expression<Func<User, double>> exp = u => u.Points.Average(p => p.Balance);
                UserAveragePointsBalance = new ValueParserTestData<User, double>(exp, segment);
            }

            //Any
            {
                ValueSegment segment = new LambdaMethodSegment(
                    name: "Any",
                    parameterDeclearingSegment: new LambdaParameterDeclaringSegment(
                        prefix: "r",
                        property: new PropertySegment(nameof(User.Reviews))));

#pragma warning disable CA1860 // 'Enumerable.Any()' 확장 메서드 사용 금지
                Expression<Func<User, bool>> exp = u => u.Reviews.Any();
#pragma warning restore CA1860 // 'Enumerable.Any()' 확장 메서드 사용 금지
                UserAnyReviews = new ValueParserTestData<User, bool>(exp, segment);
            }

            //AnyPredicate
            {
                ValueSegment segment = new LambdaMethodSegment(
                    name: "Any",
                    parameterDeclearingSegment: new LambdaParameterDeclaringSegment(
                        prefix: "r",
                        property: new PropertySegment(nameof(User.Reviews))),
                    arguments: new OperationSegment(new PropertySegment("r", new PropertySegment(nameof(Review.Rating))), new OperatorSegment(EOperator.GT), new ConstantSegment(1)));

                Expression<Func<User, bool>> exp = u => u.Reviews.Any(r => r.Rating > 1);
                UserAnyReviewsGTOne = new ValueParserTestData<User, bool>(exp, segment);
            }

            //All
            {
                ValueSegment segment = new LambdaMethodSegment(
                    name: "All",
                    parameterDeclearingSegment: new LambdaParameterDeclaringSegment(
                        prefix: "r",
                        property: new PropertySegment(nameof(User.Reviews))),
                    arguments: new OperationSegment(new PropertySegment("r", new PropertySegment(nameof(Review.Rating))), new OperatorSegment(EOperator.GT), new ConstantSegment(1)));

                Expression<Func<User, bool>> exp = u => u.Reviews.All(r => r.Rating > 1);
                UserAllReviewsGTOne = new ValueParserTestData<User, bool>(exp, segment);
            }

            //Count
            {
                ValueSegment segment = new LambdaMethodSegment(
                    name: "Count",
                    parameterDeclearingSegment: new LambdaParameterDeclaringSegment(
                        prefix: "r",
                        property: new PropertySegment(nameof(User.Reviews))));

                Expression<Func<User, int>> exp = u => u.Reviews.Count();
                UserCountReviews = new ValueParserTestData<User, int>(exp, segment);
            }

            //CountPredicate
            {
                ValueSegment segment = new LambdaMethodSegment(
                    name: "Count",
                    parameterDeclearingSegment: new LambdaParameterDeclaringSegment(
                        prefix: "r",
                        property: new PropertySegment(nameof(User.Reviews))),
                    arguments: new OperationSegment(new PropertySegment("r", new PropertySegment(nameof(Review.Rating))), new OperatorSegment(EOperator.GT), new ConstantSegment(1)));

                Expression<Func<User, int>> exp = u => u.Reviews.Count(r => r.Rating > 1);
                UserCountReviewsGTOne = new ValueParserTestData<User, int>(exp, segment);
            }

            {
                ValueSegment segment = new LambdaMethodSegment(
                    name: "Sum",
                    parameterDeclearingSegment: new LambdaParameterDeclaringSegment(
                        prefix: "o",
                        property: new PropertySegment(nameof(User.OrderSets))),
                    arguments: new LambdaMethodSegment("AVG",
                        parameterDeclearingSegment: new LambdaParameterDeclaringSegment(
                            prefix: "oo",
                            property: new PropertySegment("o", new PropertySegment(nameof(OrderSet.Orders)))),
                        arguments: new PropertySegment("oo", new PropertySegment(nameof(Order.PricePerItem)))));

                Expression<Func<User, double>> exp = u => u.OrderSets.Sum(o => o.Orders.Average(oo => oo.PricePerItem));
                UserSumOrderSetsAvgOrdersPricePerItem = new(exp, segment);
            }
        }
    }
}
