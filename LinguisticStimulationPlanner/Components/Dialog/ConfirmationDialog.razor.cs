using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LinguisticStimulationPlanner.Components.Dialog
{
    public partial class ConfirmationDialog
    {
        [Parameter] public string Message { get; set; }
        [Parameter] public string ConfirmButton { get; set; } = "Confirm";
        [Parameter] public string CancelButton { get; set; } = "Cancel";

        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private void Confirm()
        {
            MudDialog.Close(DialogResult.Ok(true));
        }

    }
}
