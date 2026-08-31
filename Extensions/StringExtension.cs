
using System.Diagnostics.CodeAnalysis;

namespace RabbitMQMonitor.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Formats a <see cref="Guid"/> as a client-facing UUID token, used as the
        /// API key issued to an Ams Client for authenticating requests.
        /// </summary>
        /// <remarks>
        /// Call as an extension method on any <see cref="Guid"/>, e.g.
        /// <c>Guid.NewGuid().ToClientUuid()</c>.
        /// </remarks>
        /// <param name="guid">The identifier to format. Typically a freshly generated <see cref="Guid"/>.</param>
        /// <returns>
        /// A 40-character string consisting of the current UTC date (<c>yyyyMMdd</c>)
        /// followed by the 32-character, no-hyphen ("N") representation of <paramref name="guid"/>.
        /// </returns>
        public static string ToClientUuid(this Guid guid) => $"{DateTime.UtcNow:yyyyMMdd}{guid:N}";

        /// <summary>
        /// Determines whether two strings are equal after trimming leading and trailing
        /// whitespace from both, ignoring case and culture
        /// (<see cref="StringComparison.OrdinalIgnoreCase"/>).
        /// </summary>
        /// <remarks>
        /// Safe to call on a <see langword="null"/> receiver: two <see langword="null"/>
        /// values are considered equal, and a <see langword="null"/> never equals a non-null value
        /// (including an empty or whitespace-only one).
        /// </remarks>
        /// <param name="source">The string to compare. May be <see langword="null"/>.</param>
        /// <param name="value">The string to compare against. May be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if both trimmed strings are equal ignoring case; otherwise <see langword="false"/>.</returns>
        public static bool IgnoreEquals(this string? source, string? value)
            => string.Equals(source?.Trim(), value?.Trim(), StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Determines whether two strings differ after trimming leading and trailing
        /// whitespace from both, ignoring case and culture
        /// (<see cref="StringComparison.OrdinalIgnoreCase"/>). The negation of
        /// <see cref="IgnoreEquals(string?, string?)"/>.
        /// </summary>
        /// <remarks>
        /// Safe to call on a <see langword="null"/> receiver, which keeps the call readable when the
        /// source is produced by a null-conditional expression, e.g. <c>(model?.Applicant).NotIgnoreEquals(userId)</c>.
        /// </remarks>
        /// <param name="source">The string to compare. May be <see langword="null"/>.</param>
        /// <param name="value">The string to compare against. May be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the strings differ ignoring case; otherwise <see langword="false"/>.</returns>
        public static bool NotIgnoreEquals(this string? source, string? value)
            => !source.IgnoreEquals(value);

        /// <summary>
        /// Determines whether a string carries any non-whitespace content, i.e. it is neither
        /// <see langword="null"/>, empty, nor made up only of whitespace.
        /// The negation of <see cref="string.IsNullOrWhiteSpace(string?)"/>.
        /// </summary>
        /// <remarks>
        /// Annotated with <see cref="NotNullWhenAttribute"/>, so the compiler treats
        /// <paramref name="source"/> as non-null inside a block guarded by this check.
        /// </remarks>
        /// <param name="source">The string to test. May be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the string holds non-whitespace content; otherwise <see langword="false"/>.</returns>
        public static bool IsNotBlank([NotNullWhen(true)] this string? source)
            => !string.IsNullOrWhiteSpace(source);

        /// <summary>
        /// Determines whether a string carries no usable content, i.e. it is
        /// <see langword="null"/>, empty, or made up only of whitespace.
        /// Equivalent to <see cref="string.IsNullOrWhiteSpace(string?)"/>.
        /// </summary>
        /// <remarks>
        /// Annotated with <see cref="NotNullWhenAttribute"/>, so the compiler treats
        /// <paramref name="source"/> as non-null on the <see langword="false"/> branch of this check.
        /// </remarks>
        /// <param name="source">The string to test. May be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the string is null, empty, or whitespace only; otherwise <see langword="false"/>.</returns>
        public static bool IsBlank([NotNullWhen(false)] this string? source)
            => string.IsNullOrWhiteSpace(source);

        /// <summary>
        /// Determines whether a string holds at least one character.
        /// The negation of <see cref="string.IsNullOrEmpty(string?)"/>.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="IsNotBlank(string?)"/>, a string made up only of whitespace
        /// counts as non-empty. Annotated with <see cref="NotNullWhenAttribute"/>, so the compiler
        /// treats <paramref name="source"/> as non-null inside a block guarded by this check.
        /// </remarks>
        /// <param name="source">The string to test. May be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the string holds at least one character; otherwise <see langword="false"/>.</returns>
        public static bool IsNotEmpty([NotNullWhen(true)] this string? source)
            => !string.IsNullOrEmpty(source);

        /// <summary>
        /// Determines whether a string is <see langword="null"/> or has no characters.
        /// Equivalent to <see cref="string.IsNullOrEmpty(string?)"/>.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="IsBlank(string?)"/>, a string made up only of whitespace
        /// counts as non-empty. Annotated with <see cref="NotNullWhenAttribute"/>, so the compiler
        /// treats <paramref name="source"/> as non-null on the <see langword="false"/> branch of this check.
        /// </remarks>
        /// <param name="source">The string to test. May be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the string is null or empty; otherwise <see langword="false"/>.</returns>
        public static bool IsEmpty([NotNullWhen(false)] this string? source)
            => string.IsNullOrEmpty(source);
    }
}
