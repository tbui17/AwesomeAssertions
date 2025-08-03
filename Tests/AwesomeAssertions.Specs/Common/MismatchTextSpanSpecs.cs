using System;
using AwesomeAssertions.Common;
using AwesomeAssertions.Common.Mismatch;
using Xunit;

namespace AwesomeAssertions.Specs.Common;

public class MismatchTextSpanSpecs
{
    [Fact]
    public void Run()
    {
        var text = "this is a very long string with lots of words that most won't be displayed in the error message!";
        var span = new TextSpanFactory(StringComparer.Ordinal).Create(text, "this is a very long string with lots of words that most won't be displayed in the error".Length);
        span.StartTruncated.Should().BeTrue();
        span.EndTruncated.Should().BeFalse();
    }
}
