using System;

namespace AwesomeAssertions.Common.Mismatch;

internal class TextSpanFactory
{
    private const int LengthOfWhitespace = 1;

    private const int DefaultCharactersToKeep = 10;

    private const int MinCharactersToKeep = 5;

    private const int MaxCharactersToKeep = 15;

    private int StringPrintLength { get; } = AssertionConfiguration.Current.Formatting.StringPrintLength;

    private int MinStringPrintLength => StringPrintLength - 5;

    private int MaxStringPrintLength => StringPrintLength + 10;

    public IMismatchTextSpan Create(string text, int mismatchIndex)
    {
        var textSpan = CreateBase(text, mismatchIndex);
        return new EscapeNewLinesMismatchTextSpanDecorator(textSpan);
    }

    public IMismatchTextSpan CreateReversed(string text, int mismatchIndex)
    {
        var start = GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex(text, mismatchIndex);
        var subjectLength = GetLengthOfPhraseToShowOrDefaultLength(text, start);

        var textSpan = new TextSpan
        {
            Start = start,
            End = start + subjectLength,
            Text = text
        };

        var reversed = new ReverseTextSpanDecorator(textSpan);

        var mismatchTextSpan = new MismatchTextSpan(reversed, mismatchIndex);

        return new ReverseMismatchTextSpanDecorator(mismatchTextSpan);
    }

    public MismatchTextSpan CreateBase(string text, int mismatchIndex)
    {
        var start = GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex(text, mismatchIndex);
        var subjectLength = GetLengthOfPhraseToShowOrDefaultLength(text, start);

        var textSpan = new TextSpan
        {
            Start = start,
            End = start + subjectLength,
            Text = text
        };

        return new MismatchTextSpan(textSpan, mismatchIndex);
    }

    private static int GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex(string text, int mismatchIndex)
    {
        // if the slice of interest is small enough, we do not need to elide the start of the string
        if (mismatchIndex <= DefaultCharactersToKeep)
        {
            return 0;
        }

        int indexToStartSearchingForWordBoundary =
            Math.Max(mismatchIndex - (MaxCharactersToKeep + LengthOfWhitespace), 0);

        int phraseLengthToCheckForWordBoundary =
            (MaxCharactersToKeep - MinCharactersToKeep) + LengthOfWhitespace;

        int indexOfWordBoundary = text
                .IndexOf(' ', indexToStartSearchingForWordBoundary, phraseLengthToCheckForWordBoundary) -
            indexToStartSearchingForWordBoundary;

        if (indexOfWordBoundary >= 0)
        {
            return indexToStartSearchingForWordBoundary + indexOfWordBoundary + LengthOfWhitespace;
        }

        return mismatchIndex - DefaultCharactersToKeep;
    }

    private int GetLengthOfPhraseToShowOrDefaultLength(string text, int start)
    {
        int indexOfWordBoundary = text
            .LastIndexOf(' ',
                Math.Min(start + MaxStringPrintLength + LengthOfWhitespace, text.Length) - 1);

        if (indexOfWordBoundary >= (start + MinStringPrintLength))
        {
            return indexOfWordBoundary - start;
        }

        return Math.Min(StringPrintLength, text.Length - start);
    }
}
