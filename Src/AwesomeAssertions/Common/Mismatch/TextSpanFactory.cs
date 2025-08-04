using System;
using System.Collections.Generic;

namespace AwesomeAssertions.Common.Mismatch;

internal class TextSpanFactory(IEqualityComparer<string> comparer)
{
    private readonly TruncateHelper truncateHelper = new();

    public MismatchContext CreateContext(string subject, string expected)
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

    public MismatchContext CreateContextReversed(string subject, string expected)
    {
        var lastMismatch = subject.IndexOfLastMismatch(expected, comparer);
        if (lastMismatch < 0)
        {
            throw new NotImplementedException();
        }
        var revSubject = subject.Reversed();
        var revExpected = expected.Reversed();
        var mismatch = revSubject.IndexOfFirstMismatch(revExpected, comparer);


        var range1 = truncateHelper.GetTruncationRange(revSubject, mismatch);

        var subjSpan = TextSpan.Create(revSubject).Subspan(range1);

        var range2 = truncateHelper.GetTruncationRange(revExpected, mismatch);
        var expSpan = TextSpan.Create(revExpected).Subspan(range2);

        var subj = new EscapeNewLinesMismatchTextSpanDecorator(new ReverseMismatchTextSpan(new MismatchTextSpan(subjSpan, mismatch)));
        var exp = new EscapeNewLinesMismatchTextSpanDecorator(new ReverseMismatchTextSpan(new MismatchTextSpan(expSpan, mismatch)));





        return new MismatchContext
        {
            SubjectSpan = subj,
            ExpectedSpan = exp,
            AdjustedIndexOfMismatch = subj.MismatchIndex,
            IndexOfMismatch = lastMismatch,
            Reversed = true,
            Subject = subject,
            Expected = expected
        };
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
