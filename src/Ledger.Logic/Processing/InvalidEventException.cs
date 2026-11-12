namespace Ledger.Logic.Processing;

/// <summary>The message can never succeed and should not be retried.</summary>
public sealed class InvalidEventException(string message) : Exception(message);
