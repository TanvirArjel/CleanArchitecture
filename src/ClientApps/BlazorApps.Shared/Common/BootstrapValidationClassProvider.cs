using System;
using System.Linq;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApps.Shared.Common;

public class BootstrapValidationClassProvider : FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        if (editContext == null)
        {
            throw new ArgumentNullException(nameof(editContext));
        }

        bool isValid = !editContext.GetValidationMessages(fieldIdentifier).Any();

        if (editContext.IsModified(fieldIdentifier))
        {
            return isValid ? "is-valid" : "is-invalid";
        }

        return isValid ? string.Empty : "is-invalid";
    }
}
