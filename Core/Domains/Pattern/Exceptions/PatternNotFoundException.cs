namespace Core.Domains.Pattern.Exceptions;

public class PatternNotFoundException : ApplicationException

{
    public PatternNotFoundException() : base($"Pattern Not found")
    {
    }
}