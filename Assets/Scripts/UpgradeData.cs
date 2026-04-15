using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Shop/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public string description;
    public int price;

    public enum UpgradeType { Damage, Speed, MaxAmmo, DoubleJump, SlowMoPower, Health }
    public UpgradeType type;
    public float value;
}