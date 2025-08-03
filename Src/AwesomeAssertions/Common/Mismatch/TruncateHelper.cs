using System;

namespace AwesomeAssertions.Common.Mismatch;

internal class TruncateHelper(TruncationOptions options)
{
    private const int LengthOfWhitespace = 1;

    public int GetStartIndexOfPhraseToShowBeforeTheTargetIndex(string text, int targetIndex)
    {
        // if the slice of interest is small enough, we do not need to elide the start of the string
        if (targetIndex <= options.DefaultCharactersToKeep)
        {
            return 0;
        }

        int indexToStartSearchingForWordBoundary =
            Math.Max(targetIndex - (options.MaxCharactersToKeep + LengthOfWhitespace), 0);

        int phraseLengthToCheckForWordBoundary =
            (options.MaxCharactersToKeep - options.MinCharactersToKeep) + LengthOfWhitespace;

        int indexOfWordBoundary = text
                .IndexOf(' ', indexToStartSearchingForWordBoundary, phraseLengthToCheckForWordBoundary) -
            indexToStartSearchingForWordBoundary;

        if (indexOfWordBoundary >= 0)
        {
            return indexToStartSearchingForWordBoundary + indexOfWordBoundary + LengthOfWhitespace;
        }

        return targetIndex - options.DefaultCharactersToKeep;
    }

    public int GetLengthOfPhraseToShowOrDefaultLength(string text, int start)
    {
        int indexOfWordBoundary = text
            .LastIndexOf(' ',
                Math.Min(start + options.MaxStringPrintLength + LengthOfWhitespace, text.Length) - 1);

        if (indexOfWordBoundary >= (start + options.MinStringPrintLength))
        {
            return indexOfWordBoundary - start;
        }

        return Math.Min(options.StringPrintLength, text.Length - start);
    }

    public SpanRange GetTruncationRange(string text, int targetIndex)
    {
        var start = GetStartIndexOfPhraseToShowBeforeTheTargetIndex(text, targetIndex);
        var length = GetLengthOfPhraseToShowOrDefaultLength(text, start);
        return new SpanRange(start, start + length);
    }
}
