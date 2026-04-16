using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private TavernManager tavernManager;
    [SerializeField] private Button continueButton;
    [SerializeField] private UpgradeSlot[] slots;
    [SerializeField] private UpgradeData[] allPossibleUpgrades;

    private EconomyManager economy;
    private bool isTransitioning;

    private void Start()
    {
        economy = FindFirstObjectByType<EconomyManager>();
        continueButton.onClick.AddListener(OnContinuePressed);

        foreach (UpgradeSlot slot in slots)
        {
            slot.onSelected += TryBuyUpgrade;
        }
    }

    private void OnEnable()
    {
        isTransitioning = false;
        GenerateRandomDeals();
    }

    private void GenerateRandomDeals()
    {
        List<UpgradeData> pool = new List<UpgradeData>();
        
        foreach (UpgradeData data in allPossibleUpgrades)
        {
            if (data.CanBeBought())
            {
                pool.Add(data);
            }
        }
        
        for (int i = 0; i < slots.Length; i++)
        {
            if (pool.Count == 0)
            {
                slots[i].gameObject.SetActive(false);
                continue;
            }
            
            slots[i].gameObject.SetActive(true);
            int randomIndex = Random.Range(0, pool.Count);
            slots[i].Setup(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }
    }

    private void TryBuyUpgrade(UpgradeData data)
    {
        if (isTransitioning) return;

        if (economy.SpendCoins(data.price))
        {
            isTransitioning = true;
            data.ApplyUpgrade();
            ExecutePurchaseFeedback(data);
        }
    }

    private void ExecutePurchaseFeedback(UpgradeData selectedData)
    {
        foreach (UpgradeSlot slot in slots)
        {
            bool isSelected = (slot.data == selectedData);
            slot.PlaySelectionAnimation(isSelected);
        }

        DOVirtual.DelayedCall(0.5f, () => tavernManager.ResumeGame()).SetUpdate(true);
    }

    private void OnContinuePressed()
    {
        if (isTransitioning) return;
        tavernManager.ResumeGame();
    }
}