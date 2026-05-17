using System.Runtime.CompilerServices;

namespace OA.Supplier.Domain;

public class DomainValidationException(string message) : Exception(message)
{
  public static void ThrowIfNullOrWhiteSpace(string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
  {
    if (string.IsNullOrWhiteSpace(argument))
    {
      throw new DomainValidationException($"'{paramName}' cannot be null or whitespace.");
    }
  }
};