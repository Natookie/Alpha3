using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Golden Circle Chance")]
public class GoldenCircleChanceUpgrade : UpgradeDefinition
{
    public float amountPerLevel = 0.15f;
    public override void Apply(int currentLevel) => PlayerStats.Instance.GoldenCircleChance += amountPerLevel;
}