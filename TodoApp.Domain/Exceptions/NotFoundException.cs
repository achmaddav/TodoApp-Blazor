namespace TodoApp.Domain.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string name, object key)
            : base($"{name} with id '{key}' was not found.") { }
    }
}
