namespace Ledger.Data;

/// <summary>A storage failure that is safe to retry.</summary>
public sealed class TransientStorageException(string message) : Exception(message);
