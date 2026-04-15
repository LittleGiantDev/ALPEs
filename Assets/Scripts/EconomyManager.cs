using UnityEngine;
using TMPro;
using DG.Tweening;

public class EconomyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    private int totalCoins;

    private void Start()
    {
        UpdateUI();
        GameEvents.OnCoinCollected += AddCoins;
    }

    private void OnDestroy()
    {
        GameEvents.OnCoinCollected -= AddCoins;
    }

    private void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateUI();
        
        coinsText.transform.DOComplete();
        coinsText.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 10, 1f);
    }
    
    private void UpdateUI()
    {
        if (coinsText != null) coinsText.text = totalCoins.ToString();
    }

    public int GetCoins() => totalCoins;
    
    public bool SpendCoins(int amount)
    {
        if (totalCoins >= amount)
        {
            totalCoins -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }
}