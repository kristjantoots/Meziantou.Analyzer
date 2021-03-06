﻿using Meziantou.Analyzer.Rules;
using Xunit;
using TestHelper;

namespace Meziantou.Analyzer.Test.Rules
{
    public sealed class DoNotUseStringGetHashCodeAnalyzerTests
    {
        private static ProjectBuilder CreateProjectBuilder()
        {
            return new ProjectBuilder()
                .WithAnalyzer<DoNotUseStringGetHashCodeAnalyzer>()
                .WithCodeFixProvider<DoNotUseStringGetHashCodeFixer>();
        }

        [Fact]
        public async System.Threading.Tasks.Task GetHashCode_ShouldReportDiagnosticAsync()
        {
            const string SourceCode = @"
class TypeName
{
    public void Test()
    {
        [||]""a"".GetHashCode();
        System.StringComparer.Ordinal.GetHashCode(""a"");
        new object().GetHashCode();
    }
}";
            const string CodeFix = @"
class TypeName
{
    public void Test()
    {
        System.StringComparer.Ordinal.GetHashCode(""a"");
        System.StringComparer.Ordinal.GetHashCode(""a"");
        new object().GetHashCode();
    }
}";
            await CreateProjectBuilder()
                  .WithSourceCode(SourceCode)
                  .ShouldFixCodeWith(CodeFix)
                  .ValidateAsync();
        }
    }
}
