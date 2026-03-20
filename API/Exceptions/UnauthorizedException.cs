using System;

namespace API.Exceptions;

public class UnauthorizedException : Exception
{
    // Default message
    public UnauthorizedException() : base("Unauthorized access.")
    {
    }

    // Custom message
    public UnauthorizedException(string message) : base(message)
    {
    }
}
