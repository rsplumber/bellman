namespace Core.Domains.Pattern.Exceptions;

public class PatternInvalidParametersException : CoreException

{
    public PatternInvalidParametersException() : base($"Pattern Invalid Parameters")
    {
    }
}