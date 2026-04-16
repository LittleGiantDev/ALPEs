using UnityEngine;

[CreateAssetMenu(fileName = "SpeedUpgrade", menuName = "Shop/Upgrades/Speed")]
public class SpeedUpgrade : UpgradeData
{
    public float speedIncrease;

    public override void ApplyUpgrade()
    {
        PlayerMovement playerMove = FindFirstObjectByType<PlayerMovement>();
        if (playerMove != null)
        {
            playerMove.UpgradeBaseSpeed(speedIncrease);
        }
    }
}