namespace Core;

public class CoreException : Exception
{
    public CoreException(string message, int code = 400) : base(message)
    {
        Code = code;
        Message = message;
    }

    public int Code { get; }

    public new string Message { get; }
    
}