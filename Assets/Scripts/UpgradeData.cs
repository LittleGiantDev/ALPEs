using UnityEngine;

public abstract class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public string description;
    public int price;

    public abstract void ApplyUpgrade();

    public virtual bool CanBeBought()
    {
        return true;
    }
}