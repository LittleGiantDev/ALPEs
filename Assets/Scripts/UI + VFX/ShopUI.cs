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
    private float resumeTimer;
    private bool isResuming;

    private void Start()
    {
        economy = FindFirstObjectByType<EconomyManager>();
        
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinuePressed);
        }

        if (slots != null)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                {
                    slots[i].onSelected += TryBuyUpgrade;
                }
            }
        }
    }

    private void OnEnable()
    {
        isTransitioning = false;
        isResuming = false;
        GenerateRandomDeals();
    }

    private void Update()
    {
        if (isResuming)
        {
            resumeTimer -= Time.unscaledDeltaTime;
            if (resumeTimer <= 0)
            {
                isResuming = false;
                if (tavernManager != null)
                {
                    tavernManager.ResumeGame();
                }
            }
        }
    }

    private void GenerateRandomDeals()
    {
        if (allPossibleUpgrades == null || slots == null)
        {
            return;
        }

        List<UpgradeData> pool = new List<UpgradeData>();
        
        for (int i = 0; i < allPossibleUpgrades.Length; i++)
        {
            if (allPossibleUpgrades[i] != null && allPossibleUpgrades[i].CanBeBought())
            {
                pool.Add(allPossibleUpgrades[i]);
            }
        }
        
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                continue;
            }

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
        if (economy == null || data == null)
        {
            return;
        }

        if (economy.SpendCoins(data.price))
        {
            data.ApplyUpgrade();
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBuySound();
            }
        
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null && slots[i].data == data)
                {
                    slots[i].transform.DOPunchScale(Vector3.one * 0.2f, 0.2f).SetUpdate(true);
                    
                    if (data.CanBeBought() == false)
                    {
                        if (slots[i].canvasGroup != null)
                        {
                            slots[i].canvasGroup.DOFade(0.5f, 0.3f).SetUpdate(true);
                        }
                        slots[i].btn.interactable = false;
                    }
                }
            }
        }
        else
        {
            transform.DOShakePosition(0.2f, 10f).SetUpdate(true);
        }
    }

    private void OnContinuePressed()
    {
        if (isTransitioning || tavernManager == null)
        {
            return;
        }
        
        tavernManager.ResumeGame();
    }
}