using UnityEngine;
using Nova;

public class UpgradeButton : MonoBehaviour
{
    [Header("DATA")]
    [SerializeField] private string upgradeId;

    [Header("REFERENCES")]
    [SerializeField] private CoreButton coreButton;
    [SerializeField] private TextBlock levelText;
    [SerializeField] private TextBlock costText;

    private UpgradeManager upgradeManager;

    void Start(){
        upgradeManager = UpgradeManager.Instance;
        if(coreButton != null) coreButton.OnClick.AddListener(HandleClick);

        if(upgradeManager != null) upgradeManager.OnLevelChanged += HandleLevelChanged;
        if(InventoryManager.Instance != null) InventoryManager.Instance.OnCurrencyChanged += HandleCurrencyChanged;

        Refresh();
    }

    void HandleClick(){
        if(upgradeManager == null) return;

        UpgradeDefinition def = upgradeManager.GetDefinition(upgradeId);
        if(def == null) return;

        upgradeManager.TryPurchase(upgradeId);
    }

    void HandleLevelChanged(string changedId, int newLevel){
        if(changedId == upgradeId) Refresh();
    }

    void HandleCurrencyChanged(float _) => Refresh();
    void Refresh(){
        if(upgradeManager == null) return;

        UpgradeDefinition def = upgradeManager.GetDefinition(upgradeId);
        if(def == null) return;

        int level   = upgradeManager.GetLevel(upgradeId);
        bool maxed  = def.IsMaxed(level);
        float cost  = def.GetCost(level);
        bool canAfford = InventoryManager.Instance != null && InventoryManager.Instance.GetCurrency() >= cost;

        if(levelText != null) levelText.Text = $"{level}";

        if(costText != null){
            if(maxed) costText.Text = "MAX";
            else      costText.Text = cost.ToString("F0");
        }

        if(coreButton != null) coreButton.SetInteractable(!maxed && canAfford);
    }
}