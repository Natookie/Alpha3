using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance {get; private set;}

    public event Action<string, int> OnLevelChanged;

    [SerializeField] private List<UpgradeDefinition> upgrades;
    [SerializeField] private InventoryManager inventory;

    private Dictionary<string, int> levels = new Dictionary<string, int>();
    private Dictionary<string, UpgradeDefinition> lookup = new Dictionary<string, UpgradeDefinition>();

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }

        foreach(var u in upgrades){
            if(u == null || string.IsNullOrEmpty(u.upgradeId)) continue;
            levels[u.upgradeId] = 0;
            lookup[u.upgradeId] = u;
        }
    }

    public UpgradeDefinition GetDefinition(string upgradeId) =>
        lookup.TryGetValue(upgradeId, out var u) ? u : null;

    public int GetLevel(string upgradeId) => levels.TryGetValue(upgradeId, out var l) ? l : 0;
    public float GetCost(UpgradeDefinition u) => u.GetCost(GetLevel(u.upgradeId));
    public bool IsMaxed(UpgradeDefinition u) => u.IsMaxed(GetLevel(u.upgradeId));

    public bool TryPurchase(string upgradeId){
        UpgradeDefinition u = GetDefinition(upgradeId);
        if(u == null) {
            Debug.LogWarning($"Unknown upgrade: {upgradeId}"); 
            return false; 
        }

        int level = GetLevel(upgradeId);
        if(u.IsMaxed(level)) return false;

        float cost = u.GetCost(level);
        if(!inventory.TrySpend(cost)) return false;

        u.Apply(level);
        levels[upgradeId] = level + 1;
        OnLevelChanged?.Invoke(upgradeId, level + 1);
        return true;
    }

    public void ResetLevels(){
        var ids = new List<string>(levels.Keys);
        foreach(var id in ids){
            levels[id] = 0;
            OnLevelChanged?.Invoke(id, 0);
        }
    }

    public List<UpgradeLevelEntry> ExportLevels(){
        var list = new List<UpgradeLevelEntry>();
        foreach(var kvp in levels){
            list.Add(new UpgradeLevelEntry { upgradeId = kvp.Key, level = kvp.Value });
        }
        return list;
    }

    public void ImportLevels(List<UpgradeLevelEntry> entries){
        foreach(var e in entries){
            if(!levels.ContainsKey(e.upgradeId)) continue;
            levels[e.upgradeId] = e.level;
            OnLevelChanged?.Invoke(e.upgradeId, e.level);
        }
    }

    public bool UpgradePointOnHit()          => TryPurchase("point_on_hit");
    public bool UpgradeCircleSpeed()         => TryPurchase("circle_speed");
    public bool UpgradeGoldenCircleChance()  => TryPurchase("golden_circle_chance");
    public bool UpgradeOuterCircleCount()    => TryPurchase("outer_circle_count");
    public bool UpgradeGlobalMultiplier()    => TryPurchase("global_multiplier");
}