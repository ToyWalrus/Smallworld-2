using System;
using System.Threading.Tasks;
using Smallworld.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class ConfirmationPopup : MonoBehaviour, IConfirmation
{
    [SerializeField] private UIDocument _popup;

    private bool isOpen = false;
    private bool isAnimating = false;

    private float animateTimeElapsed = 0f;

    public float animationDuration = .7f;

    public async Task<bool> ConfirmAsync(string message, string title = "Confirm", string confirmLabel = "Yes", string cancelLabel = "No")
    {
        var tsc = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        void onConfirm() => tsc.TrySetResult(true);
        void onCancel() => tsc.TrySetResult(false);

        var confirmBtn = _popup.rootVisualElement.Q<Button>("Confirm");
        var cancelBtn = _popup.rootVisualElement.Q<Button>("Cancel");

        try
        {
            _popup.rootVisualElement.Q<Label>("Title").text = title;
            _popup.rootVisualElement.Q<Label>("Message").text = message;
            confirmBtn.text = confirmLabel;
            cancelBtn.text = cancelLabel;

            confirmBtn.clicked += onConfirm;
            cancelBtn.clicked += onCancel;

            isOpen = true;
            isAnimating = true;

            return await tsc.Task;
        }
        catch
        {
            return false;
        }
        finally
        {
            isOpen = false;
            isAnimating = true;

            confirmBtn.clicked -= onConfirm;
            cancelBtn.clicked -= onCancel;
        }
    }


    void Update()
    {
        if (isAnimating)
        {
            if (isOpen)
            {
                AnimateIn();
            }
            else
            {
                AnimateOut();
            }
        }
    }

    private void AnimateIn()
    {
        var overlay = _popup.rootVisualElement.Q("Overlay");
        overlay.style.display = DisplayStyle.Flex;

        animateTimeElapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(animateTimeElapsed / animationDuration);
        overlay.style.opacity = progress;

        if (progress >= 1f)
        {
            isAnimating = false;
            animateTimeElapsed = 0f;
        }
    }

    private void AnimateOut()
    {
        var overlay = _popup.rootVisualElement.Q("Overlay");

        animateTimeElapsed += Time.deltaTime;
        float progress = 1f - Mathf.Clamp01(animateTimeElapsed / animationDuration);
        overlay.style.opacity = progress;

        if (progress <= 0f)
        {
            isAnimating = false;
            animateTimeElapsed = 0f;
            overlay.style.display = DisplayStyle.None;
        }
    }
}