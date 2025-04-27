namespace BookBoostApi.Models;

public class ConflictException : Exception
{
    public ConflictException()
    {
    }

    public ConflictException(string message)
        : base(message)
    {
    }

    public ConflictException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class ProductException : Exception
{
    public ProductException()
    {
    }

    public ProductException(string message)
        : base(message)
    {
    }

    public ProductException(string message, Exception inner)
        : base(message, inner)
    {
    }
}