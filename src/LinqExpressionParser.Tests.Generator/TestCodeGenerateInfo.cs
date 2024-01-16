#nullable enable

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

namespace LinqExpressionParser.Tests.Generator
{
    public class TestCodeGenerateInfo(ClassDeclarationSyntax testClassSyntax, string testMethodName, IEnumerable<TestDataInfo> testDataInfos)
    {
        public ClassDeclarationSyntax TestClassSyntax { get; } = testClassSyntax;
        public string TestMethodName { get; } = testMethodName;
        public IEnumerable<TestDataInfo> TestDataInfos { get; } = testDataInfos;
    }

    public class TestDataInfo(string identifierName, string assertMethod)
    {
        public string IdentifierName { get; } = identifierName;
        public string AssertMethod { get; } = assertMethod;
    }
}