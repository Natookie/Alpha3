using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Circle Shrink Duration")]
public class CircleShrinkDurationUpgrade : UpgradeDefinition
{
    [Tooltip("Fraction faster per level, e.g. 0.1 = 10% faster")]
    public float percentPerLevel = 0.05f;
    public override void Apply(int currentLevel) => PlayerStats.Instance.CircleShrinkDuration /= 1f + percentPerLevel;
}