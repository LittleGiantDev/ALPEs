using UnityEngine;

[CreateAssetMenu(fileName = "FireRateUpgrade", menuName = "Shop/Upgrades/FireRate")]
public class FireRateUpgrade : UpgradeData
{
    public float fireRateReduction;

    public override void ApplyUpgrade()
    {
        ShootingController playerShoot = FindFirstObjectByType<ShootingController>();
        playerShoot.UpgradeFireRate(fireRateReduction);
    }
}