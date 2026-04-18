using System.Threading.Tasks;
using Smallworld.IO;
using UnityEngine;

public class ConfirmationPopup : MonoBehaviour, IConfirmation
{
    public Task<bool> ConfirmAsync(string message, string title = "Confirm", string confirmLabel = "Yes", string cancelLabel = "No")
    {
        throw new System.NotImplementedException();
    }
}