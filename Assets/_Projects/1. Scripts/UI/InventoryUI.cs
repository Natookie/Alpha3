using UnityEngine;
using Nova;

public class InventoryUI : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private TextBlock[] digitTexts;

    [Header("SMOOTHING")]
    [SerializeField] private float countSpeed = 8f;
    [SerializeField] private float snapThreshold = 0.5f;

    private float displayedValue;
    private bool initialized;
    private int[] lastDigits;

    void Start(){
        lastDigits = new int[digitTexts.Length];
        for(int i = 0; i < lastDigits.Length; i++) lastDigits[i] = -1;

        if(InventoryManager.Instance != null){
            InventoryManager.Instance.OnCurrencyChanged += HandleCurrencyChanged;
            displayedValue = InventoryManager.Instance.GetCurrency();
            initialized = true;
            RefreshText();
        }
    }

    void OnDisable(){
        if(InventoryManager.Instance != null) InventoryManager.Instance.OnCurrencyChanged -= HandleCurrencyChanged;
    }

    void Update(){
        if(!initialized || InventoryManager.Instance == null) return;

        float target = InventoryManager.Instance.GetCurrency();
        if(Mathf.Approximately(displayedValue, target)) return;

        displayedValue = Mathf.Lerp(displayedValue, target, 1f - Mathf.Exp(-countSpeed * Time.deltaTime));
        if(Mathf.Abs(displayedValue - target) < snapThreshold) displayedValue = target;

        RefreshText();
    }

    void HandleCurrencyChanged(float newValue){}

    void RefreshText(){
        int value = Mathf.Max(0, Mathf.FloorToInt(displayedValue));
        int length = digitTexts.Length;

        for(int i = 0; i < length; i++){
            int divisor = (int)Mathf.Pow(10, length - 1 - i);
            int digit = (value / divisor) % 10;

            if(digit == lastDigits[i]) continue;

            lastDigits[i] = digit;

            if(digitTexts[i] == null) continue;
            digitTexts[i].Text = digit.ToString();
            OnDigitChanged(i, digit);
        }
    }

    protected virtual void OnDigitChanged(int index, int newDigit){}
}