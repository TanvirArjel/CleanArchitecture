using System;
using System.Runtime.Serialization;

namespace EmployeeManagement.Domain.Exceptions;

[Serializable]
public class DomainValidationException : Exception
{
    public DomainValidationException()
    {
    }

    public DomainValidationException(string message)
        : base(message)
    {
    }

    public DomainValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected DomainValidationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
    }
}
