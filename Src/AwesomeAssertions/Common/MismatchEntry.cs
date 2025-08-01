using System;
using System.Linq;

namespace AwesomeAssertions.Common;

internal class MismatchEntry
{
    private ElisionConfiguration ElisionConfiguration { get; set; } = new();

    private const int LengthOfWhitespace = 1;

    public required string Text { get; set; }

    public required int MismatchIndex { get; set; }

    public bool StartElided => GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex() > 0;

    public bool EndElided => Text.Length > GetTextSpan().End;

    private int VisibleTextLengthBeforeMismatch => MismatchIndex - GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex();

    private (int Start, int End) VisibleTextRange => (GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex(), MismatchIndex);

    private TextSpan GetTextSpan()
    {
        var trimStart = GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex();
        var subjectLength = GetLengthOfPhraseToShowOrDefaultLength(Text[trimStart..]);

        return new TextSpan
        {
            OriginalText = Text,
            Start = trimStart,
            End = trimStart + subjectLength
        };
    }

    private string VisibleText2 => Text[VisibleTextRange.Start..VisibleTextRange.End];

    public string VisibleText => GetTextSpan().VisibleText;

    public int NewlineCharacterCount => VisibleText2.Count(c => c is '\r' or '\n');

    // TODO
    public int WhitespaceCount
    {
        get
        {
            int count = VisibleTextLengthBeforeMismatch + NewlineCharacterCount;
            return StartElided ? count + 1 : count;
        }
    }

    private int GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex()
    {
        if (MismatchIndex <= ElisionConfiguration.DefaultCharactersToKeep)
        {
            return 0;
        }

        var indexToStartSearchingForWordBoundary =
            Math.Max(MismatchIndex - (ElisionConfiguration.MaxCharactersToKeep + LengthOfWhitespace), 0);

        var phraseLengthToCheckForWordBoundary =
            (ElisionConfiguration.MaxCharactersToKeep - ElisionConfiguration.MinCharactersToKeep) + LengthOfWhitespace;

        var indexOfWordBoundary = Text
                .IndexOf(' ', indexToStartSearchingForWordBoundary, phraseLengthToCheckForWordBoundary) -
            indexToStartSearchingForWordBoundary;

        if (indexOfWordBoundary >= 0)
        {
            return indexToStartSearchingForWordBoundary + indexOfWordBoundary + LengthOfWhitespace;
        }

        return MismatchIndex - ElisionConfiguration.DefaultCharactersToKeep;
    }

    private int GetLengthOfPhraseToShowOrDefaultLength(string value)
    {
        var indexOfWordBoundary = value
            .LastIndexOf(' ', Math.Min(ElisionConfiguration.MaxStringPrintLength + LengthOfWhitespace, value.Length) - 1);

        if (indexOfWordBoundary >= ElisionConfiguration.MinStringPrintLength)
        {
            return indexOfWordBoundary;
        }

        return Math.Min(ElisionConfiguration.StringPrintLength, value.Length);
    }
}
