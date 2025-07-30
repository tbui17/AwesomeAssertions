using System;
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
                "Expected string to end with \"AB\" because it should, but \"ABC\" differs near \"ABC\" (index 0).");
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

        [Fact]
        public void When_long_string_does_not_end_with_single_char_it_should_show_aligned_diff()
        {
            // Act
            Action act = () => "ABCDEFGHI".Should().EndWith("A");

            // Assert
            act.Should().Throw<XunitException>().WithMessage("""
                                                             Expected string to have the expected end, but they differ at index 3:
                                                                       ↓ (actual)
                                                               "ABCEFGHI"
                                                                      "A"
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
                    Expected string to end with the same string, but they differ at index 8:
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
                             Expected string to end with the same string, but they differ at index 8:
                                     ↓ (actual)
                               "ABCDEFGHI"
                                  "DEXGHI"
                                     ↑ (expected).
                             """);
        }

        [Fact]
        public void When_very_long_string_does_not_end_with_string_pointer_adjusts_for_ellipses()
        {
            // Act
            Action act = () => "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Should().EndWith("1YZ");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("""
                    Expected string to end with the same string, but they differ at index 23:
                                ↓ (actual)
                      "...RSTUVWXYZ"
                               "1YZ"
                                ↑ (expected).
                    """);
        }

        [Fact]
        public void When_string_with_newlines_does_not_end_with_string_it_should_show_escaped_diff()
        {
            // Act
            Action act = () => "ABC\nDEF\nGHI".Should().EndWith("HI");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("""
                    Expected string to end with the same string, but they differ at index 9:
                       ↓ (actual)
                      "ABC\nDEF\nGHI"
                                "HI"
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
                    Expected string to end with the same string, but they differ at index 1:
                       ↓ (actual)
                      "ABC"
                       "XY"
                        ↑ (expected).
                    """);
        }
    }
}
