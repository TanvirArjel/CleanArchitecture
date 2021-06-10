using System;

namespace BlazorApp.Helpers
{
    public static class Requires
    {
        public static T IsNotNull<T>(T instance, string paramName)
            where T : class
        {
            // Use ReferenceEquals in case T overrides equals.
            if (object.ReferenceEquals(null, instance))
            {
                // Call a method that throws instead of throwing directly. This allows
                // this IsNotNull method to be inlined.
                throw new ArgumentNullException(paramName);
            }

            return instance;
        }

        public static long IsPositiveLong(long value, string paramName)
        {
            // Check if positive long number.
            if (value <= 0)
            {
                // Call a method that throws instead of throwing directly. This allows
                // this IsNotNull method to be inlined.
                throw new ArgumentOutOfRangeException(paramName, "The value must be greater than 0.");
            }

            return value;
        }

        public static int IsPositiveInt(int value, string paramName)
        {
            // Check if positive long number.
            if (value <= 0)
            {
                // Call a method that throws instead of throwing directly. This allows
                // this IsNotNull method to be inlined.
                throw new ArgumentOutOfRangeException(paramName, "The value must be greater than 0.");
            }

            return value;
        }

        public static string IsNotNull(string value, string paramName)
        {
            // Use ReferenceEquals in case T overrides equals.
            if (string.IsNullOrWhiteSpace(value))
            {
                // Call a method that throws instead of throwing directly. This allows
                // this IsNotNull method to be inlined.
                throw new ArgumentNullException(paramName);
            }

            return value;
        }
    }
}
