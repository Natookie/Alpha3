using System;
using UnityEngine;
using NaughtyAttributes;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance {get; private set;}
    public event Action<float> OnCurrencyChanged;

    private float currency;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    public void SetCurrency(float value){
        currency = value;
        OnCurrencyChanged?.Invoke(currency);
    }
    public void AddCurrency(float value){
        currency += value;
        OnCurrencyChanged?.Invoke(currency);

        // AudioManager.Instance.PlaySfx("Point Gained");
    }

    [Button("Debug Add Currency")]
    public void DebugAddCurrency() => AddCurrency(100000f);

    public bool TrySpend(float value){
        if(currency < value) return false;
        currency -= value;
        OnCurrencyChanged?.Invoke(currency);
        AudioManager.Instance.PlaySfx("Purchase");
        return true;
    }

    public float GetCurrency() => currency;
}