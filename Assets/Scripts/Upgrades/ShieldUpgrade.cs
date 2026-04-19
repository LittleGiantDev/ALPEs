using UnityEngine;

[CreateAssetMenu(fileName = "ShieldUpgrade", menuName = "Shop/Upgrades/Shield")]
public class ShieldUpgrade : UpgradeData
{
    public override void ApplyUpgrade()
    {
        PlayerMovement playerMove = FindFirstObjectByType<PlayerMovement>();
        playerMove.EnableShield();
    }

    public override bool CanBeBought()
    {
        PlayerMovement playerMove = FindFirstObjectByType<PlayerMovement>();
        return !playerMove.HasShield();
    }
}