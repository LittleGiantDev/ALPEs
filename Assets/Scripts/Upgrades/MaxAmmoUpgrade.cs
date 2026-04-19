using UnityEngine;

[CreateAssetMenu(fileName = "MaxAmmoUpgrade", menuName = "Shop/Upgrades/MaxAmmo")]
public class MaxAmmoUpgrade : UpgradeData
{
    public int extraAmmo;

    public override void ApplyUpgrade()
    {
        ShootingController playerShoot = FindFirstObjectByType<ShootingController>();
        playerShoot.UpgradeMaxAmmo(extraAmmo);
    }
}