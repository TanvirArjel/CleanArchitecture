using System;
using System.Runtime.Serialization;

namespace EmployeeManagement.Domain.Exceptions
{
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
            : base()
        {
        }

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

        // A constructor is needed for serialization when an exception propagates from a remoting server to the client.
        protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
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
}
