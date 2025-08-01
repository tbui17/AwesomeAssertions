using AwesomeAssertions.Common;
using Xunit;

namespace AwesomeAssertions.Specs.Common;

public class MismatchEntrySpecs
{
    [Fact]
    public void Run()
    {
        var s = new MismatchEntry
        {
            Text = "this is a long text that differs in between two words",
            MismatchIndex = 23
        };
        s.VisibleText.Should().Be("a long text that differs in between two words");
    }

    [Fact]
    public void Run2()
    {
        var s = new MismatchEntry
        {
            Text = "A\r\nB",
            MismatchIndex = 5
        };
        s.VisibleText.Should().Be("A\r\nB");
    }
}
