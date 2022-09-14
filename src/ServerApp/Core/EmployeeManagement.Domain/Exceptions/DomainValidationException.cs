using System;
using System.Runtime.Serialization;

namespace EmployeeManagement.Domain.Exceptions;

[Serializable]
public sealed class DomainValidationException : Exception
{
    public DomainValidationException(string message)
        : base(message)
    {
    }

    public DomainValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    private DomainValidationException()
    {
    }

    private DomainValidationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
    }
}
