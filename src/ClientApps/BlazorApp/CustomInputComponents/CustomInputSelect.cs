using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp.CustomInputComponents
{
    public class CustomInputSelect<TValue> : InputSelect<TValue>
    {
        protected override bool TryParseValueFromString(string value, out TValue result, out string validationErrorMessage)
        {
            if (typeof(TValue) == typeof(int) || typeof(TValue) == typeof(int?))
            {
                if (int.TryParse(value, out int resultValue))
                {
                    result = (TValue)(object)resultValue;
                    validationErrorMessage = null;
                    return true;
                }
                else
                {
                    result = default;
                    validationErrorMessage = $"The selected value {value} is not a valid number.";
                    return false;
                }
            }

            return base.TryParseValueFromString(value, out result, out validationErrorMessage);
        }
    }
}
