using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace AwesomeAssertions.Common.Mismatch;

internal class MismatchContext
{
    public required IMismatchTextSpan Subject { get; init; }
    public required ITextSpan Expected { get; init; }
}

internal class IndexMismatchErrorMessageFactory
{
    private const string Indentation = "  ";
    private const string Prefix = Indentation + "\"";
    private const string Suffix = "\"";
    private const string ArrowDown = "\u2193";
    private const string ArrowUp = "\u2191";
    private const string Ellipsis = "\u2026";

    public string Subject { get; set; } = "";

    public string Expected { get; set; } = "";

    public string ExpectationDescription { get; set; } = "";

    public IEqualityComparer<string> Comparer { get; set; } = StringComparer.Ordinal;

    public bool Reversed { get; set; } = false;

    private readonly TextSpanFactory textSpanFactory = new();

    private MismatchContext GetMismatchTextSpans(int indexOfMismatch)
    {
        return new MismatchContext
        {
            Subject = textSpanFactory.Create(Subject, indexOfMismatch),
            Expected = textSpanFactory.Create(Expected, indexOfMismatch)
        };
    }

    [CanBeNull]
    private IndexMismatchErrorMessage CreateImpl()
    {
        var indexOfMismatch = Subject.IndexOfFirstMismatch2(Expected, Comparer);

        if (indexOfMismatch < 0)
        {
            return null;
        }

        return new IndexMismatchErrorMessage
        {
            ExpectationDescription = ExpectationDescription,
            LocationDescription = CreateLocationDescription(indexOfMismatch),
            MismatchSegment = GetMismatchSegmentImpl(GetMismatchTextSpans(indexOfMismatch))
        };
    }

    [CanBeNull]
    private IndexMismatchErrorMessage CreateReversed()
    {
        throw new NotImplementedException();
    }

    [CanBeNull]
    public IndexMismatchErrorMessage Create()
    {
        return Reversed ? CreateReversed() : CreateImpl();
    }

    private string CreateLocationDescription(int indexOfMismatch)
    {
        var matchingString = Subject[..indexOfMismatch];
        int lineNumber = matchingString.Count(c => c == '\n');

        if (lineNumber > 0)
        {
            var indexOfLastNewlineBeforeMismatch = matchingString.LastIndexOf('\n');
            var column = matchingString.Length - indexOfLastNewlineBeforeMismatch;
            return $"on line {lineNumber + 1} and column {column} (index {indexOfMismatch})";
        }

        var prefix = Reversed ? "before" : "at";
        return $"{prefix} index {indexOfMismatch}";
    }

    public static IndexMismatchErrorMessage CreateFailureMessage(
        string expectationDescription,
        string subject,
        string expected,
        int indexOfMismatch)
    {
        string locationDescription = $"at index {indexOfMismatch}";
        var matchingString = subject[..indexOfMismatch];
        int lineNumber = matchingString.Count(c => c == '\n');

        if (lineNumber > 0)
        {
            var indexOfLastNewlineBeforeMismatch = matchingString.LastIndexOf('\n');
            var column = matchingString.Length - indexOfLastNewlineBeforeMismatch;
            locationDescription = $"on line {lineNumber + 1} and column {column} (index {indexOfMismatch})";
        }

        var mismatchSegment = GetMismatchSegment(subject, expected, indexOfMismatch);

        return new IndexMismatchErrorMessage
        {
            ExpectationDescription = expectationDescription,
            LocationDescription = locationDescription,
            MismatchSegment = mismatchSegment,
        };
    }

    /// <summary>
    /// Get the mismatch segment between <paramref name="expected"/> and <paramref name="subject"/>,
    /// when they differ at index <paramref name="firstIndexOfMismatch"/>.
    /// </summary>
    private static TextSegment GetMismatchSegment(string subject, string expected, int firstIndexOfMismatch)
    {
        var factory = new TextSpanFactory();
        var subjectEntry = factory.Create(subject, firstIndexOfMismatch);
        var expectedEntry = factory.Create(expected, firstIndexOfMismatch);

        int whiteSpaceCountBeforeArrow = subjectEntry.MismatchIndex + Prefix.Length;

        if (subjectEntry.StartElided)
        {
            whiteSpaceCountBeforeArrow += 1;
        }

        return new TextSegment
        {
            Lines =
            {
                new()
                {
                    Text = $"{ArrowDown} (actual)",
                    Indent = whiteSpaceCountBeforeArrow
                },
                new()
                {
                    Text = WrapText(subjectEntry)
                },
                new()
                {
                    Text = WrapText(expectedEntry)
                },
                new()
                {
                    Text = $"{ArrowUp} (expected)",
                    Indent = whiteSpaceCountBeforeArrow
                }
            }
        };
    }

    private static TextSegment GetMismatchSegmentImpl(MismatchContext ctx)
    {
        var subjectEntry = ctx.Subject;
        var expectedEntry = ctx.Expected;

        int whiteSpaceCountBeforeArrow = subjectEntry.MismatchIndex + Prefix.Length;

        if (subjectEntry.StartElided)
        {
            whiteSpaceCountBeforeArrow += 1;
        }

        return new TextSegment
        {
            Lines =
            {
                new()
                {
                    Text = $"{ArrowDown} (actual)",
                    Indent = whiteSpaceCountBeforeArrow
                },
                new()
                {
                    Text = WrapText(subjectEntry)
                },
                new()
                {
                    Text = WrapText(expectedEntry)
                },
                new()
                {
                    Text = $"{ArrowUp} (expected)",
                    Indent = whiteSpaceCountBeforeArrow
                }
            }
        };
    }

    private static string WrapText(ITextSpan textSpan)
    {
        var innerPrefix = textSpan.StartElided ? Ellipsis : "";
        var innerSuffix = textSpan.EndElided ? Ellipsis : "";
        return $"{Prefix}{innerPrefix}{textSpan.VisibleText}{innerSuffix}{Suffix}";
    }
}
