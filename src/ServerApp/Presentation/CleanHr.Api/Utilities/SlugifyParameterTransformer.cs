using System.Globalization;
using System.Text.RegularExpressions;

namespace CleanHr.Api.Utilities;

internal sealed partial class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string TransformOutbound(object value)
    {
        // Slugify value
        return value == null ? null : MyRegex().Replace(value.ToString(), "$1-$2").ToLower(CultureInfo.InvariantCulture);
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex MyRegex();
}
