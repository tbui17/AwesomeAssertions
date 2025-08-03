using System.Collections.Generic;
using AwesomeAssertions.Common.Mismatch;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

internal class StringEqualityStrategy : IStringComparisonStrategy
{
    private readonly IEqualityComparer<string> comparer;
    private readonly string predicateDescription;

    public StringEqualityStrategy(IEqualityComparer<string> comparer, string predicateDescription)
    {
        this.comparer = comparer;
        this.predicateDescription = predicateDescription;
    }

    public string ExpectationDescription => $"Expected {{context:string}} to {predicateDescription} ";

    public void ValidateAgainstMismatch(AssertionChain assertionChain, string subject, string expected)
    {
        if (comparer.Equals(subject, expected))
        {
            return;
        }

        if (ValidateAgainstSuperfluousWhitespace(assertionChain, subject, expected))
        {
            return;
        }

        var msg = new IndexMismatchErrorMessageFactory
        {
            ExpectationDescription = ExpectationDescription,
            Context = new TextSpanFactory(comparer).CreateAggregate(subject, expected)
        }.Create();

        if (msg is null)
        {
            return;
        }

        assertionChain
            .FailWith(() => new FailReason(msg));
    }

    private bool ValidateAgainstSuperfluousWhitespace(AssertionChain assertion, string subject, string expected)
    {
        assertion
            .ForCondition(!(expected.Length > subject.Length && comparer.Equals(expected.TrimEnd(), subject)))
            .FailWith($"{ExpectationDescription}{{0}}{{reason}}, but it misses some extra whitespace at the end.", expected)
            .Then
            .ForCondition(!(subject.Length > expected.Length && comparer.Equals(subject.TrimEnd(), expected)))
            .FailWith($"{ExpectationDescription}{{0}}{{reason}}, but it has unexpected whitespace at the end.", expected);

        return !assertion.Succeeded;
    }
}
