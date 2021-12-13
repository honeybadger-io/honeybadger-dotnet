using System;

namespace Honeybadger.Tests;

public class NamedException : Exception
{
    public NamedException(string? message) : base(message)
    {
    }
}