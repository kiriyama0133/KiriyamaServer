using System.Text.RegularExpressions;

namespace KiriyamaServer.Domain.Users;

/// <summary>
/// 用户的邮箱地址（值对象）。负责格式校验。
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex Regex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Email cannot be empty.");
        }

        string normalized = value.Trim().ToLowerInvariant();

        if (!Regex.IsMatch(normalized))
        {
            throw new DomainException("Email format is invalid.");
        }

        Value = normalized;
    }

    public bool Equals(Email? other) => other is not null && Value == other.Value;

    public override bool Equals(object? obj) => obj is Email other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
