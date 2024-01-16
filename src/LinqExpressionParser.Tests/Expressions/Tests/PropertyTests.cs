using System.Linq.Expressions;

using ValueParserTestGenerator.Codes;

namespace LinqExpressionParser.Tests.Expressions.Tests
{
    [ValueParserTestClass("PropertyTest", nameof(TestAssert))]
    public partial class PropertyTests : ExpressionParserTestBase
    {
        private static readonly ValueParserTestData<User, string> UserName;
        private static readonly ValueParserTestData<User, List<Review>> UserReviews;
        private static readonly ValueParserTestData<User, int> UserReviewsCount;
        private static readonly ValueParserTestData<User, bool> UserIsBlocked;

        protected override VerifySettings VerifySettings { get; }

        public PropertyTests()
        {
            VerifySettings = new();
            VerifySettings.UseDirectory("PropertyTests");
        }

        static PropertyTests()
        {
            {
                Expression<Func<User, string>> exp = u => u.Name;
                ValueSegment segment = new PropertySegment(nameof(User.Name));
                UserName = new(exp, segment);
            }

            {
                Expression<Func<User, List<Review>>> exp = u => u.Reviews;
                ValueSegment segment = new PropertySegment(nameof(User.Reviews));
                UserReviews = new(exp, segment);
            }

            {
                Expression<Func<User, int>> exp = u => u.Reviews.Count;
                ValueSegment segment = new PropertySegment(nameof(User.Reviews), new PropertySegment("Count"));
                UserReviewsCount = new(exp, segment);
            }

            {
                Expression<Func<User, bool>> exp = u => u.IsBlocked;
                ValueSegment segment = new PropertySegment(nameof(User.IsBlocked));
                UserIsBlocked = new(exp, segment);
            }
        }
    }
}
