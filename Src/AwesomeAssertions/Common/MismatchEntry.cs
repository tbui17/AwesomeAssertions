using System;
using System.Linq;

namespace AwesomeAssertions.Common;

public class ElisionConfiguration
{
    public int DefaultCharactersToKeep { get; set; } = 10;
    public int MinCharactersToKeep { get; set; } = 5;
    public int MaxCharactersToKeep { get; set; } = 15;
    public int StringPrintLength { get; set; } = AssertionConfiguration.Current.Formatting.StringPrintLength;
    public int MinStringPrintLength => StringPrintLength - 5;
    public int MaxStringPrintLength => StringPrintLength + 10;
}

internal class MismatchEntry
{
    // formatting

    public ElisionConfiguration ElisionConfiguration { get; set; } = new();

    private const int LengthOfWhitespace = 1;

    private int PhraseLengthToCheckForWordBoundary => (ElisionConfiguration.MaxCharactersToKeep - ElisionConfiguration.MinCharactersToKeep) + LengthOfWhitespace;

    public int StringPrintLength { get; set; } = AssertionConfiguration.Current.Formatting.StringPrintLength;

    public int MinStringPrintLength => StringPrintLength - 5;

    public int MaxStringPrintLength => StringPrintLength + 10;

    // impl

    public required string Text { get; set; }

    public required int MismatchIndex { get; set; }

    public int Ending => GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex() + SubjectLength;

    public int SubjectLength => GetLengthOfPhraseToShowOrDefaultLength(Text[GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex()..]);

    public bool RightElided => Text.Length > Ending;

    public int VisibleTextLengthBeforeMismatch => MismatchIndex - GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex();

    public bool LeftElided => GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex() > 0;

    public TextSpan GetTextSpan()
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

    public (int Start, int End) VisibleTextRange => (GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex(), MismatchIndex);
    public string VisibleText2 => Text[VisibleTextRange.Start..VisibleTextRange.End];
    public string VisibleText => GetTextSpan().VisibleText;
    public int NewlineCharacterCount => VisibleText2.Count(c => c is '\r' or '\n');

    private int GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex()
    {
        if (MismatchIndex <= ElisionConfiguration.DefaultCharactersToKeep)
        {
            return 0;
        }

        var indexToStartSearchingForWordBoundary = Math.Max(MismatchIndex - (ElisionConfiguration.MaxCharactersToKeep + LengthOfWhitespace), 0);

        var indexOfWordBoundary = Text
                .IndexOf(' ', indexToStartSearchingForWordBoundary, PhraseLengthToCheckForWordBoundary) -
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
            .LastIndexOf(' ', Math.Min(MaxStringPrintLength + LengthOfWhitespace, value.Length) - 1);

        if (indexOfWordBoundary >= MinStringPrintLength)
        {
            return indexOfWordBoundary;
        }

        return Math.Min(StringPrintLength, value.Length);
    }
}
