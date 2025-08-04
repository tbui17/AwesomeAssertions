using System;
using System.Linq;
using AwesomeAssertions.Execution;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Specs.Primitives;

/// <content>
/// The [Not]EndWith specs.
/// </content>
public partial class StringAssertionSpecs
{
    public class EndWith
    {
        [Fact]
        public void When_asserting_string_ends_with_a_suffix_it_should_not_throw()
        {
            // Arrange
            string actual = "ABC";
            string expectedSuffix = "BC";

            // Act / Assert
            actual.Should().EndWith(expectedSuffix);
        }

        [Fact]
        public void When_asserting_string_ends_with_the_same_value_it_should_not_throw()
        {
            // Arrange
            string actual = "ABC";
            string expectedSuffix = "ABC";

            // Act / Assert
            actual.Should().EndWith(expectedSuffix);
        }

        [Fact]
        public void When_string_does_not_end_with_expected_phrase_it_should_throw()
        {
            // Act
            Action act = () =>
            {
                using var a = new AssertionScope();
                "ABC".Should().EndWith("AB", "it should");
            };

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected string*it should*index 2*ABC*AB*");
        }

        [Fact]
        public void When_string_ending_is_compared_with_null_it_should_throw()
        {
            // Act
            Action act = () => "ABC".Should().EndWith(null);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage(
                "Cannot compare string end with <null>.*");
        }

        [Fact]
        public void When_string_ending_is_compared_with_empty_string_it_should_not_throw()
        {
            // Act / Assert
            "ABC".Should().EndWith("");
        }

        [Fact]
        public void When_string_ending_is_compared_with_string_that_is_longer_it_should_throw()
        {
            // Act
            Action act = () => "ABC".Should().EndWith("00ABC");

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected string to end with " +
                "\"00ABC\", but " +
                "\"ABC\" is too short.");
        }

        [Fact]
        public void Correctly_stop_further_execution_when_inside_assertion_scope()
        {
            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                "ABC".Should().EndWith("00ABC").And.EndWith("CBA00");
            };

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "*\"00ABC\"*");
        }

        [Fact]
        public void When_string_ending_is_compared_and_actual_value_is_null_then_it_should_throw()
        {
            // Arrange
            string someString = null;

            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                someString.Should().EndWith("ABC");
            };

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected someString to end with \"ABC\", but found <null>.");
        }
    }

    public class NotEndWith
    {
        [Fact]
        public void When_asserting_string_does_not_end_with_a_value_and_it_does_not_it_should_succeed()
        {
            // Arrange
            string value = "ABC";

            // Act / Assert
            value.Should().NotEndWith("AB");
        }

        [Fact]
        public void When_asserting_string_does_not_end_with_a_value_but_it_does_it_should_fail_with_a_descriptive_message()
        {
            // Arrange
            string value = "ABC";

            // Act
            Action action = () =>
                value.Should().NotEndWith("BC", "because of some {0}", "reason");

            // Assert
            action.Should().Throw<XunitException>().WithMessage(
                "Expected value not to end with \"BC\" because of some reason, but found \"ABC\".");
        }

        [Fact]
        public void When_asserting_string_does_not_end_with_a_value_that_is_null_it_should_throw()
        {
            // Arrange
            string value = "ABC";

            // Act
            Action action = () =>
                value.Should().NotEndWith(null);

            // Assert
            action.Should().Throw<ArgumentNullException>().WithMessage(
                "Cannot compare end of string with <null>.*");
        }

        [Fact]
        public void When_asserting_string_does_not_end_with_a_value_that_is_empty_it_should_throw()
        {
            // Arrange
            string value = "ABC";

            // Act
            Action action = () =>
                value.Should().NotEndWith("");

            // Assert
            action.Should().Throw<XunitException>().WithMessage(
                "Expected value not to end with \"\", but found \"ABC\".");
        }

        [Fact]
        public void When_asserting_string_does_not_end_with_a_value_and_actual_value_is_null_it_should_throw()
        {
            // Arrange
            string someString = null;

            // Act
            Action act = () =>
            {
                using var _ = new AssertionScope();
                someString.Should().NotEndWith("ABC", "some {0}", "reason");
            };

            // Assert
            act.Should().Throw<XunitException>().WithMessage(
                "Expected someString not to end with \"ABC\"*some reason*, but found <null>.");
        }
    }

    public class Diff
    {
        [Fact]
        public void When_long_string_does_not_end_with_single_char_it_should_show_aligned_diff()
        {
            // Act
            Action act = () => "ABCDEFGHI".Should().EndWith("H");

            // Assert
            act.Should().Throw<XunitException>().WithMessage("""
                                                             Expected string to end with *, but they differ before index 8:
                                                                        ↓ (actual)
                                                               "ABCDEFGHI"
                                                                       "H"
                                                                        ↑ (expected).
                                                             """);
        }

        [Fact]
        public void When_long_string_does_not_end_with_long_string_it_should_show_aligned_diff()
        {
            // Act
            Action act = () => "ABCDEFGHI".Should().EndWith("DEFGHX");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("""
                    Expected string to end with *, but they differ before index 8:
                               ↓ (actual)
                      "ABCDEFGHI"
                         "DEFGHX"
                               ↑ (expected).
                    """);
        }

        [Fact]
        public void When_long_string_does_not_end_with_long_string_at_middle_it_should_show_aligned_diff()
        {
            // Act
            Action act = () => "ABCDEFGHI".Should().EndWith("DEXGHI");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("""
                             Expected string to end with *, but they differ before index 5:
                                     ↓ (actual)
                               "ABCDEFGHI"
                                  "DEXGHI"
                                     ↑ (expected).
                             """);
        }

        [Fact]
        public void When_short_strings_have_no_common_chars_it_should_show_aligned_diff()
        {
            // Act
            Action act = () => "ABC".Should().EndWith("XY");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("""
                    Expected string to end with *, but they differ before index 2:
                         ↓ (actual)
                      "ABC"
                       "XY"
                         ↑ (expected).
                    """);
        }

        [Fact]
        public void When_one_string_is_long_they_are_right_aligned22()
        {
            // Act
            Action act = () => "from cancel this waver was coming from this is a long text pat thaT differs in between two words".Should().Be("such and when to this the other they should and sad rhino whicH differs in between two words");


            var exc = act.Should().Throw<XunitException>();
            using var _ = new AssertionScope();

            var lines = exc.Which.Message.Split('\n');

            lines[1].IndexOf("↓", StringComparison.Ordinal)
                .Should().Be(lines[2].IndexOf("f", StringComparison.Ordinal))
                .And.Be(lines[3].IndexOf("s", StringComparison.Ordinal))
                .And.Be(lines[4].IndexOf("↑", StringComparison.Ordinal));


            // Assert
            exc
                .WithMessage("""
                             Expected * to be the same string, but they differ at index 0:
                                ↓ (actual)
                               "from cancel this waver was coming from this is a long text…"
                               "such and when to this the other they should and sad rhino…"
                                ↑ (expected).
                             """
                );
        }

        [Fact]
        public void actual_and_expected_messages_are_right_aligned()
        {
            // Arrange
            const string subject = "from cancel this waver was coming from this is a long text pat thaT differs in between two words";
            const string expected = "such and when to this the other they should and sad rhino whicH differs in between two words";

            // Act
            Action act = () => subject.Should().EndWith(expected);

            // Assert
            var exc = act.Should().Throw<XunitException>();
            var lines = exc.And.Message.Split('\n');
            lines[2].Should().HaveLength(lines[3].Length);
        }

        [Fact]
        public void diff_characters_and_arrows_have_same_column()
        {
            // Arrange
            const string subject = "from cancel this waver was coming from this is a long text pat thaT differs in between two words";
            const string expected = "such and when to this the other they should and sad rhino whicH differs in between two words";

            // Act
            Action act = () => subject.Should().EndWith(expected);

            // Assert
            var exc = act.Should().Throw<XunitException>().WithMessage($"""
                                                             **
                                                             *↓ (actual)*
                                                             *pat thaT differs*
                                                             *rhino whicH differs*
                                                             *↑ (expected)*
                                                             """);

            using var scope = new AssertionScope();
            var lines = exc.And.Message.Split('\n');
            var downIndex = lines[1].IndexOf("↓", StringComparison.Ordinal);
            var tIndex = lines[2].IndexOf("T", StringComparison.Ordinal);
            var hIndex = lines[3].IndexOf("H", StringComparison.Ordinal);
            var upIndex = lines[4].IndexOf("↑", StringComparison.Ordinal);
            new[] { downIndex, hIndex, upIndex }.Should().AllBeEquivalentTo(tIndex);
            if (scope.HasFailures())
            {
                scope.AddPreFormattedFailure("Original exception message: " + exc.And.Message);
            }
        }

        [Fact]
        public void When_one_string_is_long_they_are_right_aligned2()
        {
            string subject = new string('A',40) + new string('B', 60);
            string expected = "C";

            // Act
            Action act = () => subject.Should().EndWith(expected);

            // Assert
            act
                .Should()
                .Throw<XunitException>()
                .WithMessage("""
                             * index 99:
                                                                                  ↓ (actual)
                               "…BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB"
                                                                                 "C"
                                                                                  ↑ (expected).
                             """
                );
        }

        [Fact]
        public void When_one_string_is_long_they_are_right_aligned3()
        {
            string subject = new string('A',60) + new string('B', 40);
            string expected = 'C' + new string('A', 59) + new string('B', 40);

            // Act
            Action act = () => subject.Should().EndWith(expected);

            // Assert
            act
                .Should()
                .Throw<XunitException>()
                .WithMessage("""
                             * index 0:
                                ↓ (actual)
                               "AAAAAAAAAAA…"
                               "CAAAAAAAAAA…"
                                ↑ (expected).
                             """
                );
        }

        [Fact]
        public void When_both_strings_are_long_they_are_right_aligned()
        {
            // Act
            Action act = () => "this is a long text that differs in between two words".Should().EndWith("long text which differs in between two words");

            // Assert
            act
                .Should()
                .Throw<XunitException>()
                .WithMessage("""
                             * index 23:
                                                       ↓ (actual)
                               "this is a long text that differs in…"
                                        "long text which differs in…"
                                                       ↑ (expected).
                             """
                );
        }
    }
}
