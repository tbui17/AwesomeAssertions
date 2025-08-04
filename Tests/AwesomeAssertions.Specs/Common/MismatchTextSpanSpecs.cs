using System;
using AwesomeAssertions.Common;
using AwesomeAssertions.Common.Mismatch;
using AwesomeAssertions.Execution;
using Xunit;

namespace AwesomeAssertions.Specs.Common;

public class MismatchTextSpanSpecs
{
    private readonly TextSpanFactory textSpanFactory = new(StringComparer.Ordinal);

    [Fact]
    public void Run()
    {
        var expectedIndex = 100 + (4 * Environment.NewLine.Length);

        var subject = """
                      @startuml
                      Alice -> Bob : Authentication Request
                      Bob --> Alice : Authentication Response

                      Alice -> Bob : Another authentication Request
                      Alice <-- Bob : Another authentication Response
                      @enduml
                      """;

        var expected = """
                       @startuml
                       Alice -> Bob : Authentication Request
                       Bob --> Alice : Authentication Response

                       Alice -> Bob : Invalid authentication Request
                       Alice <-- Bob : Another authentication Response
                       @enduml
                       """;

        var ctx = textSpanFactory.CreateContext(subject,expected);
        using var _ = new AssertionScope();
        ctx.SubjectSpan.VisibleText.Should().Be(@"-> Bob : Another authentication Request\r\nAlice <-- Bob :");
        ctx.ExpectedSpan.VisibleText.Should().Be(@"-> Bob : Invalid authentication Request\r\nAlice <-- Bob :");
        ctx.IndexOfMismatch.Should().Be(expectedIndex);
    }

    [Fact]
    public void Run2()
    {
        var expectedIndex = 100 + (4 * Environment.NewLine.Length);

        var subject = """
                      @startuml
                      Alice -> Bob : Authentication Request
                      Bob --> Alice : Authentication Response

                      Alice -> Bob : Another authentication Request
                      Alice <-- Bob : Another authentication Response
                      @enduml
                      """;

        var expected = """
                       @startuml
                       Alice -> Bob : Authentication Request
                       Bob --> Alice : Authentication Response

                       Alice -> Bob : Invalid authentication Request
                       Alice <-- Bob : Another authentication Response
                       @enduml
                       """;

        var ctx = textSpanFactory.CreateContext(subject,expected);
        using var _ = new AssertionScope();
        ctx.SubjectSpan.VisibleText.Should().Be(@"-> Bob : Another authentication Request\r\nAlice <-- Bob :");
        ctx.ExpectedSpan.VisibleText.Should().Be(@"-> Bob : Invalid authentication Request\r\nAlice <-- Bob :");
        ctx.IndexOfMismatch.Should().Be(expectedIndex);
    }


    [Fact]
    public void Run3()
    {
        var subject = "from which this person was coming from this is a long text and thaT differs in between two words";
        var expected = "such and when to do more like she should and man woman whicH differs in between two words";
        var ctx = textSpanFactory.CreateContextReversed(subject,expected);
        using var _ = new AssertionScope();

        ctx.IndexOfMismatch.Should().Be("from which this person was coming from this is a long text and thaT".Length - 1);
        ctx.AdjustedIndexOfMismatch.Should().BeLessThan(ctx.IndexOfMismatch);
        ctx.SubjectSpan.VisibleText.Should().Contain("thaT");
        ctx.SubjectSpan.StartTruncated.Should().BeTrue();
        ctx.SubjectSpan.EndTruncated.Should().BeTrue();
        ctx.ExpectedSpan.VisibleText.Should().Contain("whicH");
        ctx.ExpectedSpan.StartTruncated.Should().BeTrue();
        ctx.ExpectedSpan.EndTruncated.Should().BeTrue();
    }

    [Fact]
    public void Run4()
    {
        string subject = 'A' + new string('B', 99);
        string expected = 'A' + new string('B', 98) + 'C';

        var ctx = textSpanFactory.CreateContextReversed(subject,expected);
        using var _ = new AssertionScope();

        ctx.SubjectSpan.VisibleText.Should().NotContain("A");
        ctx.ExpectedSpan.VisibleText.Should().NotContain("A");

        ctx.IndexOfMismatch.Should().Be(99);
    }
}
