namespace Core.Providers.Exceptions;

public class ProviderNameExistsException : ApplicationException
{
    public ProviderNameExistsException() : base($"Provider name exists")
    {
    }
}