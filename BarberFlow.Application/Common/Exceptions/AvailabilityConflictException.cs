namespace BarberFlow.Application.Common.Exceptions;

public sealed class AvailabilityConflictException(string message) : Exception(message);
