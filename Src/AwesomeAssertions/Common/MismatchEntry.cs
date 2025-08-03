using System;
using System.Linq;

namespace AwesomeAssertions.Common;

internal class MismatchEntryFactory
{
    private ElisionConfiguration ElisionConfiguration { get; init; } = new();
    private const int LengthOfWhitespace = 1;

    public static MismatchEntry Create2(string text, int mismatchIndex)
    {
        var factory = new MismatchEntryFactory();
        return factory.Create(text, mismatchIndex);
    }

    public MismatchEntry Create(string text, int mismatchIndex)
    {
        var start = GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex(text, mismatchIndex);
        var subjectLength = GetLengthOfPhraseToShowOrDefaultLength(text, start);
        var textSpan = new TextSpan
        {
            Start = start,
            End = start + subjectLength,
            Text = text
        };

        return new MismatchEntry(textSpan, mismatchIndex);
    }

    private int GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex(string text, int mismatchIndex)
    {
        // if the slice of interest is small enough, we do not need to elide the start of the string
        if (mismatchIndex <= ElisionConfiguration.DefaultCharactersToKeep)
        {
            return 0;
        }

        int indexToStartSearchingForWordBoundary =
            Math.Max(mismatchIndex - (ElisionConfiguration.MaxCharactersToKeep + LengthOfWhitespace), 0);

        int phraseLengthToCheckForWordBoundary =
            (ElisionConfiguration.MaxCharactersToKeep - ElisionConfiguration.MinCharactersToKeep) + LengthOfWhitespace;

        int indexOfWordBoundary = text
                .IndexOf(' ', indexToStartSearchingForWordBoundary, phraseLengthToCheckForWordBoundary) -
            indexToStartSearchingForWordBoundary;

        if (indexOfWordBoundary >= 0)
        {
            return indexToStartSearchingForWordBoundary + indexOfWordBoundary + LengthOfWhitespace;
        }

        return mismatchIndex - ElisionConfiguration.DefaultCharactersToKeep;
    }

    private int GetLengthOfPhraseToShowOrDefaultLength(string text, int start)
    {
        int indexOfWordBoundary = text
            .LastIndexOf(' ',
                Math.Min(start + ElisionConfiguration.MaxStringPrintLength + LengthOfWhitespace, text.Length) - 1);

        if (indexOfWordBoundary >= (start + ElisionConfiguration.MinStringPrintLength))
        {
            return indexOfWordBoundary - start;
        }

        return Math.Min(ElisionConfiguration.StringPrintLength, text.Length - start);
    }
}

internal class MismatchEntry(TextSpan textSpan, int mismatchIndex)
{
    public TextSpan TextSpan => textSpan;

    public int RelativeMismatchIndex => mismatchIndex - textSpan.Start;

    public string LeftSegment => textSpan.VisibleText[..RelativeMismatchIndex];

    public string VisibleText => textSpan.VisibleText.EscapeNewLines();

    public int WhitespaceCount
    {
        get
        {
            // compensate for newlines
            int count = LeftSegment.Length + LeftSegment.Count(c => c is '\r' or '\n');

            return textSpan.StartElided ? count + 1 : count;
        }
    }
}
