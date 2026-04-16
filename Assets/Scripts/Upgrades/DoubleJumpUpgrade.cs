using UnityEngine;

[CreateAssetMenu(fileName = "DoubleJumpUpgrade", menuName = "Shop/Upgrades/DoubleJump")]
public class DoubleJumpUpgrade : UpgradeData
{
    public override void ApplyUpgrade()
    {
        PlayerMovement playerMove = FindFirstObjectByType<PlayerMovement>();
        if (playerMove != null)
        {
            playerMove.EnableDoubleJump();
        }
    }

    public override bool CanBeBought()
    {
        PlayerMovement playerMove = FindFirstObjectByType<PlayerMovement>();
        if (playerMove != null)
        {
            return !playerMove.HasDoubleJump();
        }
        return true;
    }
}