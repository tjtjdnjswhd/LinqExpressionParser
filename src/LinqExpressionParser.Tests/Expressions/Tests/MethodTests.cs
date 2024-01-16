using System.Linq.Expressions;

using ValueParserTestGenerator.Codes;

namespace LinqExpressionParser.Tests.Expressions.Tests
{
    [ValueParserTestClass("MethodTest", nameof(TestAssert))]
    public partial class MethodTests : ExpressionParserTestBase
    {
        private static readonly ValueParserTestData<User, bool> UserNameContains;
        private static readonly ValueParserTestData<User, int> UserNameCompareEmail;
        private static readonly ValueParserTestData<User, string> UserNameConcatEmail;
        private static readonly ValueParserTestData<User, int> UserEmailIndexOf;
        private static readonly ValueParserTestData<User, bool> UserEmailStartsWith;
        private static readonly ValueParserTestData<User, bool> UserEmailEndsWith;
        private static readonly ValueParserTestData<User, string> UserSubstringName;
        private static readonly ValueParserTestData<User, string> UserTrimName;
        private static readonly ValueParserTestData<User, string> UserTrimStartName;
        private static readonly ValueParserTestData<User, string> UserTrimEndName;
        private static readonly ValueParserTestData<User, string> UserToLowerName;
        private static readonly ValueParserTestData<User, string> UserToUpperName;
        private static readonly ValueParserTestData<User, DateTimeOffset> UserAddYearsLastLoginAt;
        private static readonly ValueParserTestData<User, DateTimeOffset> UserAddMonthsLastLoginAt;
        private static readonly ValueParserTestData<User, DateTimeOffset> UserAddDaysLastLoginAt;
        private static readonly ValueParserTestData<User, DateTimeOffset> UserAddHoursLastLoginAt;
        private static readonly ValueParserTestData<User, DateTimeOffset> UserAddMinutesLastLoginAt;
        private static readonly ValueParserTestData<User, DateTimeOffset> UserAddSecondsLastLoginAt;
        private static readonly ValueParserTestData<User, DateTimeOffset> UserAddMillisecondsLastLoginAt;

        protected override VerifySettings VerifySettings { get; }

        public MethodTests()
        {
            VerifySettings = new();
            VerifySettings.UseDirectory("MethodTests");
        }

        static MethodTests()
        {
            //Contains
            {
                ValueSegment segment = new MethodSegment(
                    name: "Contains",
                    new PropertySegment(nameof(User.Name)), new ConstantSegment("abc"));
                Expression<Func<User, bool>> exp = u => u.Name.Contains("abc");
                UserNameContains = new(exp, segment);
            }

            //Compare
            {
                ValueSegment segment = new MethodSegment(
                    name: "Compare",
                    new PropertySegment(nameof(User.Name)), new PropertySegment(nameof(User.Email)));
                Expression<Func<User, int>> exp = u => string.Compare(u.Name, u.Email);
                UserNameCompareEmail = new ValueParserTestData<User, int>(exp, segment);
            }

            //Concat
            {
                ValueSegment segment = new MethodSegment("Concat", new PropertySegment(nameof(User.Name)), new PropertySegment(nameof(User.Email)));
                Expression<Func<User, string>> exp = u => string.Concat(u.Name, u.Email);
                UserNameConcatEmail = new ValueParserTestData<User, string>(exp, segment);
            }

            //IndexOf
            {
                ValueSegment segment = new MethodSegment("IndexOf", new PropertySegment(nameof(User.Email)), new ConstantSegment("\'\"3"));
                Expression<Func<User, int>> exp = u => u.Email.IndexOf("\'\"3");
                UserEmailIndexOf = new ValueParserTestData<User, int>(exp, segment);
            }

            //StartsWith
            {
                ValueSegment segment = new MethodSegment("StartsWith", new PropertySegment(nameof(User.Email)), new ConstantSegment("56"));
                Expression<Func<User, bool>> exp = u => u.Email.StartsWith("56");
                UserEmailStartsWith = new ValueParserTestData<User, bool>(exp, segment);
            }

            //EndsWith
            {
                ValueSegment segment = new MethodSegment("EndsWith", new PropertySegment(nameof(User.Email)), new ConstantSegment("56"));
                Expression<Func<User, bool>> exp = u => u.Email.EndsWith("56");
                UserEmailEndsWith = new ValueParserTestData<User, bool>(exp, segment);
            }

            //Substring
            {
                ValueSegment segment = new MethodSegment("Substring", new PropertySegment(nameof(User.Name)), new ConstantSegment(6));
                Expression<Func<User, string>> exp = u => u.Name.Substring(6);
                UserSubstringName = new ValueParserTestData<User, string>(exp, segment);
            }

            //Trim
            {
                ValueSegment segment = new MethodSegment("Trim", new PropertySegment(nameof(User.Name)));
                Expression<Func<User, string>> exp = u => u.Name.Trim();
                UserTrimName = new ValueParserTestData<User, string>(exp, segment);
            }

            //TrimStart
            {
                ValueSegment segment = new MethodSegment("TrimStart", new PropertySegment(nameof(User.Name)));
                Expression<Func<User, string>> exp = u => u.Name.TrimStart();
                UserTrimStartName = new ValueParserTestData<User, string>(exp, segment);
            }

            //TrimEnd
            {
                ValueSegment segment = new MethodSegment("TrimEnd", new PropertySegment(nameof(User.Name)));
                Expression<Func<User, string>> exp = u => u.Name.TrimEnd();
                UserTrimEndName = new ValueParserTestData<User, string>(exp, segment);
            }

            //ToLower
            {
                ValueSegment segment = new MethodSegment("ToLower", new PropertySegment(nameof(User.Name)));
                Expression<Func<User, string>> exp = u => u.Name.ToLower();
                UserToLowerName = new ValueParserTestData<User, string>(exp, segment);
            }

            //ToUpper
            {
                ValueSegment segment = new MethodSegment("ToUpper", new PropertySegment(nameof(User.Name)));
                Expression<Func<User, string>> exp = u => u.Name.ToUpper();
                UserToUpperName = new ValueParserTestData<User, string>(exp, segment);
            }

            //AddYears
            {
                ValueSegment segment = new MethodSegment("AddYears", new PropertySegment(nameof(User.LastLoginAt)), new ConstantSegment(4));
                Expression<Func<User, DateTimeOffset>> exp = u => u.LastLoginAt.AddYears(4);
                UserAddYearsLastLoginAt = new ValueParserTestData<User, DateTimeOffset>(exp, segment);
            }

            //AddMonths
            {
                ValueSegment segment = new MethodSegment("AddMonths", new PropertySegment(nameof(User.LastLoginAt)), new ConstantSegment(4));
                Expression<Func<User, DateTimeOffset>> exp = u => u.LastLoginAt.AddMonths(4);
                UserAddMonthsLastLoginAt = new ValueParserTestData<User, DateTimeOffset>(exp, segment);
            }

            //AddDays
            {
                ValueSegment segment = new MethodSegment("AddDays", new PropertySegment(nameof(User.LastLoginAt)), new ConstantSegment(4.0));
                Expression<Func<User, DateTimeOffset>> exp = u => u.LastLoginAt.AddDays(4);
                UserAddDaysLastLoginAt = new ValueParserTestData<User, DateTimeOffset>(exp, segment);
            }

            //AddHours
            {
                ValueSegment segment = new MethodSegment("AddHours", new PropertySegment(nameof(User.LastLoginAt)), new ConstantSegment(4.0));
                Expression<Func<User, DateTimeOffset>> exp = u => u.LastLoginAt.AddHours(4);
                UserAddHoursLastLoginAt = new ValueParserTestData<User, DateTimeOffset>(exp, segment);
            }

            //AddMinutes
            {
                ValueSegment segment = new MethodSegment("AddMinutes", new PropertySegment(nameof(User.LastLoginAt)), new ConstantSegment(4.0));
                Expression<Func<User, DateTimeOffset>> exp = u => u.LastLoginAt.AddMinutes(4);
                UserAddMinutesLastLoginAt = new ValueParserTestData<User, DateTimeOffset>(exp, segment);
            }

            //AddSeconds
            {
                ValueSegment segment = new MethodSegment("AddSeconds", new PropertySegment(nameof(User.LastLoginAt)), new ConstantSegment(4.0));
                Expression<Func<User, DateTimeOffset>> exp = u => u.LastLoginAt.AddSeconds(4);
                UserAddSecondsLastLoginAt = new ValueParserTestData<User, DateTimeOffset>(exp, segment);
            }

            //AddMilliseconds
            {
                ValueSegment segment = new MethodSegment("AddMilliseconds", new PropertySegment(nameof(User.LastLoginAt)), new ConstantSegment(4.0));
                Expression<Func<User, DateTimeOffset>> exp = u => u.LastLoginAt.AddMilliseconds(4);
                UserAddMillisecondsLastLoginAt = new ValueParserTestData<User, DateTimeOffset>(exp, segment);
            }
        }
    }
}
