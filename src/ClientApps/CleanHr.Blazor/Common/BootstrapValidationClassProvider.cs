using System;
using System.Linq;
using Microsoft.AspNetCore.Components.Forms;

namespace CleanHr.Blazor.Common;

internal sealed class BootstrapValidationClassProvider : FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        ArgumentNullException.ThrowIfNull(editContext);

        bool isValid = !editContext.GetValidationMessages(fieldIdentifier).Any();

        if (editContext.IsModified(fieldIdentifier))
        {
            return isValid ? "is-valid" : "is-invalid";
        }

        return isValid ? string.Empty : "is-invalid";
    }
}
