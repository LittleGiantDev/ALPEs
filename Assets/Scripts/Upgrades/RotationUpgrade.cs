using UnityEngine;

[CreateAssetMenu(fileName = "RotationUpgrade", menuName = "Shop/Upgrades/Rotation")]
public class RotationUpgrade : UpgradeData
{
    public float rotationBoost;

    public override void ApplyUpgrade()
    {
        PlayerMovement playerMove = FindFirstObjectByType<PlayerMovement>();
        playerMove.UpgradeRotationSpeed(rotationBoost);
    }
}