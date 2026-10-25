using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Outer Circle Count")]
public class OuterCircleCountUpgrade : UpgradeDefinition
{
    public int amountPerLevel = 1;
    public override void Apply(int currentLevel) => PlayerStats.Instance.OuterCircleCount += amountPerLevel;
}