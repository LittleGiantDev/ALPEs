using UnityEngine;

[CreateAssetMenu(fileName = "ReloadSpeedUpgrade", menuName = "Shop/Upgrades/ReloadSpeed")]
public class ReloadSpeedUpgrade : UpgradeData
{
    public float reloadTimeReduction;

    public override void ApplyUpgrade()
    {
        ShootingController playerShoot = FindFirstObjectByType<ShootingController>();
        if (playerShoot != null)
        {
            playerShoot.UpgradeReloadSpeed(reloadTimeReduction);
        }
    }
}