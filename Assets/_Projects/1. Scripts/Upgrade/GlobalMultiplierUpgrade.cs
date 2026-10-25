using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Global Multiplier")]
public class GlobalMultiplierUpgrade : UpgradeDefinition
{
    public float amountPerLevel = 0.25f;
    public override void Apply(int currentLevel) => PlayerStats.Instance.GlobalMultiplier += amountPerLevel;
}