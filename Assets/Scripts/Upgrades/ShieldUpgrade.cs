using UnityEngine;

[CreateAssetMenu(fileName = "ShieldUpgrade", menuName = "Shop/Upgrades/Shield")]
public class ShieldUpgrade : UpgradeData
{
    public override void ApplyUpgrade()
    {
        PlayerMovement playerMove = FindFirstObjectByType<PlayerMovement>();
        if (playerMove != null)
        {
            playerMove.EnableShield();
        }
    }

    public override bool CanBeBought()
    {
        PlayerMovement playerMove = FindFirstObjectByType<PlayerMovement>();
        if (playerMove != null)
        {
            return !playerMove.HasShield();
        }
        return true;
    }
}