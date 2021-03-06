﻿using Meziantou.Analyzer.Rules;
using Xunit;
using TestHelper;

namespace Meziantou.Analyzer.Test.Rules
{
    public sealed class MakeMethodStaticAnalyzerTests_Properties
    {
        private static ProjectBuilder CreateProjectBuilder()
        {
            return new ProjectBuilder()
                .WithAnalyzer<MakeMethodStaticAnalyzer>(id: "MA0041")
                .WithCodeFixProvider<MakeMethodStaticFixer>();
        }

        [Fact]
        public async System.Threading.Tasks.Task ExpressionBodyAsync()
        {
            const string SourceCode = @"
class TestClass
{
    int [||]A => throw null;
}
";
            const string CodeFix = @"
class TestClass
{
    static int A => throw null;
}
";
            await CreateProjectBuilder()
                  .WithSourceCode(SourceCode)
                  .ShouldFixCodeWith(CodeFix)
                  .ValidateAsync();
        }

        [Fact]
        public async System.Threading.Tasks.Task AccessInstanceProperty_NoDiagnosticAsync()
        {
            const string SourceCode = @"
class TestClass
{
    int A => TestProperty;

    public int TestProperty { get; }
}
";
            await CreateProjectBuilder()
                  .WithSourceCode(SourceCode)
                  .ValidateAsync();
        }

        [Fact]
        public async System.Threading.Tasks.Task AccessInstanceMethodInLinqQuery_Where_NoDiagnosticAsync()
        {
            const string SourceCode = @"
class TestClass
{
    int A { get; set; }
}
";
            await CreateProjectBuilder()
                  .WithSourceCode(SourceCode)
                  .ValidateAsync();
        }

        [Fact]
        public async System.Threading.Tasks.Task AccessStaticPropertyAsync()
        {
            const string SourceCode = @"
class TestClass
{
    public int [||]A => TestProperty;

    public static int TestProperty => 0;
}
";
            const string CodeFix = @"
class TestClass
{
    public static int A => TestProperty;

    public static int TestProperty => 0;
}
";
            await CreateProjectBuilder()
                  .WithSourceCode(SourceCode)
                  .ShouldFixCodeWith(CodeFix)
                  .ValidateAsync();
        }

        [Fact]
        public async System.Threading.Tasks.Task AccessStaticMethodAsync()
        {
            const string SourceCode = @"
class TestClass
{
    int [||]A => TestMethod();

    public static int TestMethod() => 0;
}
";
            await CreateProjectBuilder()
                  .WithSourceCode(SourceCode)
                  .ValidateAsync();
        }

        [Fact]
        public async System.Threading.Tasks.Task AccessStaticFieldAsync()
        {
            const string SourceCode = @"
class TestClass
{
    int [||]A => _a;

    static int _a;
}
";
            const string CodeFix = @"
class TestClass
{
    static int A => _a;

    static int _a;
}
";
            await CreateProjectBuilder()
                  .WithSourceCode(SourceCode)
                  .ShouldFixCodeWith(CodeFix)
                  .ValidateAsync();
        }

        [Fact]
        public async System.Threading.Tasks.Task AccessInstanceFieldAsync()
        {
            const string SourceCode = @"
class TestClass
{
    int A => _a;

    public int _a;
}
";
            await CreateProjectBuilder()
                  .WithSourceCode(SourceCode)
                  .ValidateAsync();
        }

        [Fact]
        public async System.Threading.Tasks.Task MethodImplementAnInterfaceAsync()
        {
            const string SourceCode = @"
class TestClass : ITest
{
    public int A { get; }
}

interface ITest
{
    int A { get; }
}
";
            await CreateProjectBuilder()
                  .WithSourceCode(SourceCode)
                  .ValidateAsync();
        }

        [Fact]
        public async System.Threading.Tasks.Task MethodExplicitlyImplementAnInterfaceAsync()
        {
            const string SourceCode = @"
class TestClass : ITest
{
    int ITest.A { get; }
}

interface ITest
{
    int A { get; }
}
";
            await CreateProjectBuilder()
                  .WithSourceCode(SourceCode)
                  .ValidateAsync();
        }
    }
}
