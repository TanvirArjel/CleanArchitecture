using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagement.Application.Extensions
{
    public static class ArgumentExtensions
    {
        public static T ThrowIfNull<T>(this T value, string paramName, string message = null)
            where T : class
        {
            if (value is null)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentNullException(paramName);
                }

                throw new ArgumentNullException(paramName, message);
            }

            return value;
        }

        public static T ThrowIfNull<T>(this T? value, string paramName, string message = null)
            where T : struct
        {
            if (value is null)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentNullException(paramName);
                }

                throw new ArgumentNullException(paramName, message);
            }

            return value.Value;
        }

        public static long ThrowIfNotPositive(this long value, string paramName, string message = null)
        {
            if (value <= 0)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentOutOfRangeException(paramName, "The value must be greater than 0.");
                }

                throw new ArgumentOutOfRangeException(paramName, message);
            }

            return value;
        }

        public static long ThrowIfOutOfRange(this long value, long min, long max, string paramName, string message = null)
        {
            if (value < min || value > max)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentOutOfRangeException(paramName, $"The value must be in between {min} and {max}.");
                }

                throw new ArgumentOutOfRangeException(paramName, message);
            }

            return value;
        }

        public static int ThrowIfNotPositive(this int value, string paramName, string message = null)
        {
            if (value <= 0)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentOutOfRangeException(paramName, "The value must be greater than 0.");
                }

                throw new ArgumentOutOfRangeException(paramName, message);
            }

            return value;
        }

        public static int ThrowIfOutOfRange(this int value, int min, int max, string paramName, string message = null)
        {
            if (value < min || value > max)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentOutOfRangeException(paramName, $"The value must be in between {min} and {max}.");
                }

                throw new ArgumentOutOfRangeException(paramName, message);
            }

            return value;
        }

        public static string ThrowIfNullOrEmpty(this string value, string paramName, string message = null)
        {
            value.ThrowIfNull(paramName, message);

            if (string.IsNullOrWhiteSpace(value))
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentException("The value of paramter is empty", paramName);
                }

                throw new ArgumentNullException(paramName, message);
            }

            return value;
        }

        public static Guid ThrowIfEmpty(this Guid value, string paramName, string message = null)
        {
            if (value == Guid.Empty)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentException("The value of paramter is empty", paramName);
                }

                throw new ArgumentException(message, paramName);
            }

            return value;
        }

        public static Guid ThrowIfNullOrEmpty(this Guid? value, string paramName, string message = null)
        {
            value.ThrowIfNull(paramName, message);
            ((Guid)value).ThrowIfEmpty(paramName, message);
            return value.Value;
        }

        public static IEnumerable<T> ThrowIfNull<T>(this IEnumerable<T> collection, string paramName, string message = null)
        {
            if (collection == null)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentNullException(paramName);
                }

                throw new ArgumentNullException(paramName, message);
            }

            return collection;
        }

        public static IEnumerable<T> ThrowIfNullOrEmpty<T>(this IEnumerable<T> collection, string paramName, string message = null)
        {
            collection.ThrowIfNull(paramName, message);

            if (!collection.Any())
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentException("The collection is empty.", paramName);
                }

                throw new ArgumentException(message, paramName);
            }

            return collection;
        }
    }
}
