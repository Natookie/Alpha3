using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Point On Hit")]
public class PointOnHitUpgrade : UpgradeDefinition
{
    public float amountPerLevel = 2f;
    public override void Apply(int currentLevel) => PlayerStats.Instance.PointOnHit += amountPerLevel;
}