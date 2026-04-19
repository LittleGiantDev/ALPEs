using UnityEngine;

[CreateAssetMenu(fileName = "ExplosiveBulletsUpgrade", menuName = "Shop/Upgrades/ExplosiveBullets")]
public class ExplosiveBulletsUpgrade : UpgradeData
{
    public override void ApplyUpgrade()
    {
        ShootingController playerShoot = FindFirstObjectByType<ShootingController>();
        playerShoot.EnableExplosiveBullets();
    }

    public override bool CanBeBought()
    {
        ShootingController playerShoot = FindFirstObjectByType<ShootingController>();
        return !playerShoot.HasExplosiveBullets();
        return true;
    }
}