using UnityEngine;

public abstract class UpgradeDefinition : ScriptableObject
{
    [Header("IDENTITY")]
    public string upgradeId;
    public string displayName;

    [Header("COST")]
    public float baseCost = 10f;
    public float costMultiplier = 1.15f;

    [Header("LEVELS")]
    public int maxLevel = 10;

    public float GetCost(int currentLevel) => baseCost * Mathf.Pow(costMultiplier, currentLevel);
    public bool IsMaxed(int currentLevel) => currentLevel >= maxLevel;
    public abstract void Apply(int currentLevel);
}