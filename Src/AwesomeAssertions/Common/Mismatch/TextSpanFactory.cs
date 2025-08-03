using System;
using System.Collections.Generic;

namespace AwesomeAssertions.Common.Mismatch;

internal class TextSpanFactory(IEqualityComparer<string> comparer)
{
    private readonly TruncateHelper truncateHelper = new(new TruncationOptions());

    public MismatchContext CreateAggregate(string subject, string expected)
    {
        var mismatch = subject.IndexOfFirstMismatch2(expected, comparer);
        if (mismatch < 0)
        {
            throw new NotImplementedException();
        }

        var range1 = truncateHelper.GetTruncationRange(subject, mismatch);

        var subjSpan = TextSpan.Create(subject).Subspan(range1);

        var range2 = truncateHelper.GetTruncationRange(expected, mismatch);
        var expSpan = TextSpan.Create(expected).Subspan(range2);

        return new MismatchContext
        {
            SubjectSpan = new EscapeNewLinesMismatchTextSpanDecorator(subjSpan),
            ExpectedSpan = new EscapeNewLinesMismatchTextSpanDecorator(expSpan),
            AdjustedIndexOfMismatch = subjSpan.GetRelativeOffset(mismatch - range1.Start),
            IndexOfMismatch = mismatch,
            Reversed = false,
            Subject = subject,
            Expected = expected
        };
    }

    public IMismatchTextSpan Create(string text, int mismatchIndex)
    {
        return new EscapeNewLinesMismatchTextSpanDecorator(new MismatchTextSpan(CreateTextSpan(text, mismatchIndex),
            mismatchIndex));
    }

    public IMismatchTextSpan CreateReversed(string text, int mismatchIndex)
    {
        return new EscapeNewLinesMismatchTextSpanDecorator(
            new ReverseMismatchTextSpan(new MismatchTextSpan(CreateTextSpan(text, mismatchIndex), mismatchIndex)));
    }

    private SubTextSpan CreateTextSpan(string text, int mismatchIndex)
    {
        var range = truncateHelper.GetTruncationRange(text, mismatchIndex);

        var span = TextSpan.Create(text).Subspan(range);
        return span;
    }
}
