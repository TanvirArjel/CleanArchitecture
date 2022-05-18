using System.Globalization;
using System.Text.RegularExpressions;

namespace EmployeeManagement.Api.Utilities;

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string TransformOutbound(object value)
    {
        // Slugify value
        return value == null ? null : Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower(CultureInfo.InvariantCulture);
    }
}
