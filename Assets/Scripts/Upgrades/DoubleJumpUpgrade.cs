using UnityEngine;

[CreateAssetMenu(fileName = "DoubleJumpUpgrade", menuName = "Shop/Upgrades/DoubleJump")]
public class DoubleJumpUpgrade : UpgradeData
{
    public override void ApplyUpgrade()
    {
        PlayerMovement playerMove = FindFirstObjectByType<PlayerMovement>();
        playerMove.EnableDoubleJump();
    }

    public override bool CanBeBought()
    {
        PlayerMovement playerMove = FindFirstObjectByType<PlayerMovement>();
        return !playerMove.HasDoubleJump();
    }
}