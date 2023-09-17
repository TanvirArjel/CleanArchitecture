using System;
using System.Runtime.Serialization;

namespace CleanHr.Domain.Exceptions;

[Serializable]
public sealed class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message)
        : base(message)
    {
    }

    public EntityNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public EntityNotFoundException(Type entityType)
       : base(GetExceptionMessage(entityType, null))
    {
    }

    public EntityNotFoundException(Type entityType, object key)
       : base(GetExceptionMessage(entityType, key))
    {
    }

    private EntityNotFoundException()
       : base()
    {
    }

    private EntityNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
    {
        throw new NotImplementedException();
    }

    private static string GetExceptionMessage(Type entityType, object key)
    {
        if (key == null)
        {
            return $"Entity \'{entityType.Name}\' was not found.";
        }

        return $"Entity \'{entityType.Name}\' with the id/key value \'{key}\' was not found.";
    }
}
