using System.Collections;
using Nova;
using UnityEngine;

public class DisplayModeConfirmUI : MonoBehaviour
{
    public static DisplayModeConfirmUI Instance { get; private set; }

    [Header("PANEL")]
    [SerializeField] private UIBlock2D panel;
    [SerializeField] private UIBlock2D dialogBox;

    [Header("TEXT")]
    [SerializeField] private TextBlock countdownText;
    [SerializeField] private TextBlock messageText;

    [Header("BUTTONS")]
    [SerializeField] private CoreButton confirmButton;
    [SerializeField] private CoreButton revertButton;

    [Header("ANIMATION")]
    [SerializeField] private UIBlockAnimator dialogAnimator;
    [SerializeField] private float slideDuration = 0.25f;
    [SerializeField] private Vector2 slideOffset = new Vector2(0f, -400f);

    [Header("TIMER")]
    [SerializeField] private float confirmDuration = 15f;

    private DisplayMode pendingMode;
    private Coroutine animateCoroutine;
    private Coroutine timerCoroutine;
    private bool confirmed;
    private Vector2 dialogOrigin;

    void Awake(){
        if(Instance != null && Instance != this){ Destroy(gameObject); return; }
        Instance = this;

        if(dialogAnimator != null && dialogBox != null) dialogAnimator.Assign(dialogBox, null, null);

        if(dialogBox != null){
            dialogOrigin = dialogBox.Position.XY.Value;
            dialogBox.Position.XY.Value = dialogOrigin + slideOffset;
        }

        if(confirmButton != null) confirmButton.OnClick.AddListener(HandleConfirm);
        if(revertButton != null) revertButton.OnClick.AddListener(HandleRevert);
    }

    void OnDisable(){
        if(confirmButton != null) confirmButton.OnClick.RemoveListener(HandleConfirm);
        if(revertButton != null) revertButton.OnClick.RemoveListener(HandleRevert);
    }

    public void RequestModeChange(DisplayMode newMode){
        if(DisplayModeManager.Instance == null) return;
        if(DisplayModeManager.Instance.CurrentMode == newMode) return;

        pendingMode = newMode;
        confirmed = false;
        if(messageText != null) messageText.Text = "Change to " + newMode + "?";
        if(countdownText != null) countdownText.Text = "";

        if(animateCoroutine != null) StopCoroutine(animateCoroutine);
        animateCoroutine = StartCoroutine(SlideIn());

        if(timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(CountdownRoutine());
    }

    IEnumerator SlideIn(){
        if(dialogBox != null) dialogBox.Position.XY.Value = dialogOrigin + slideOffset;

        if(dialogAnimator != null) dialogAnimator.AnimateMove(dialogOrigin + slideOffset, dialogOrigin, slideDuration);
        else if(dialogBox != null) dialogBox.Position.XY.Value = dialogOrigin;

        yield return null;
    }

    IEnumerator CountdownRoutine(){
        float remaining = confirmDuration;

        while(remaining > 0f && !confirmed){
            remaining -= Time.unscaledDeltaTime;
            if(countdownText != null) countdownText.Text = "Countdown: " + Mathf.CeilToInt(remaining).ToString() + "s";
            yield return null;
        }

        if(!confirmed) HandleRevert();
    }

    void HandleConfirm(){
        if(confirmed) return;
        confirmed = true;

        if(timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = null;

        DisplayModeManager.Instance.ApplyImmediate(pendingMode);
        DisplayModeManager.Instance.SaveMode(pendingMode);
        HideDialog();
    }

    void HandleRevert(){
        if(confirmed) return;
        confirmed = true;

        if(timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = null;

        HideDialog();
    }

    public void Revert() => HandleRevert();

    void HideDialog(){
        if(animateCoroutine != null) StopCoroutine(animateCoroutine);
        animateCoroutine = StartCoroutine(SlideOut());
    }

    IEnumerator SlideOut(){
        if(dialogAnimator != null) dialogAnimator.AnimateMove(dialogOrigin, dialogOrigin + slideOffset, slideDuration);
        else if(dialogBox != null) dialogBox.Position.XY.Value = dialogOrigin + slideOffset;

        if(dialogAnimator != null) yield return new WaitForSecondsRealtime(slideDuration);
    }
}