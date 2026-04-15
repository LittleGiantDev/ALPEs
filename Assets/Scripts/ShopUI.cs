using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TavernManager tavernManager;
    [SerializeField] private TextMeshProUGUI playerCoinsText;
    [SerializeField] private Button continueButton;
    
    [Header("Upgrade Slots")]
    [SerializeField] private UpgradeSlot[] slots; // Crea 3 slots en tu UI
    [SerializeField] private UpgradeData[] allPossibleUpgrades; // Base de datos de todas las mejoras

    private UpgradeData selectedUpgrade;
    private EconomyManager economy;

    private void Start()
    {
        economy = FindFirstObjectByType<EconomyManager>();
        continueButton.onClick.AddListener(OnContinuePressed);
        
        // Al empezar, cada slot necesita saber qué pasa si lo clickas
        foreach (var slot in slots)
        {
            slot.onSelected += HandleUpgradeSelection;
        }
    }

    private void OnEnable()
    {
        RefreshCoinsUI();
        GenerateRandomDeals();
    }

    private void GenerateRandomDeals()
    {
        // Elegimos 3 mejoras al azar de la lista
        List<UpgradeData> pool = new List<UpgradeData>(allPossibleUpgrades);
        for (int i = 0; i < slots.Length; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            slots[i].Setup(pool[randomIndex]);
            pool.RemoveAt(randomIndex); // Para que no se repitan
        }
    }

    private void HandleUpgradeSelection(UpgradeData data)
    {
        selectedUpgrade = data;
        // Aquí podrías iluminar el botón para que se vea que está elegido
    }

    private void RefreshCoinsUI()
    {
        if (economy != null) playerCoinsText.text = "MONEDAS: " + economy.GetCoins();
    }

    private void OnContinuePressed()
    {
        if (selectedUpgrade != null)
        {
            if (economy.SpendCoins(selectedUpgrade.price))
            {
                ApplyUpgradeEffect(selectedUpgrade);
            }
        }
        
        selectedUpgrade = null;
        tavernManager.ResumeGame();
    }

    private void ApplyUpgradeEffect(UpgradeData data)
    {
        // GameObject player = GameObject.FindGameObjectWithTag("Player");
        //
        // switch (data.type)
        // {
        //     case UpgradeData.UpgradeType.Speed:
        //         player.GetComponent<PlayerMovement>().UpgradeBaseSpeed(data.value);
        //         break;
        //     case UpgradeData.UpgradeType.MaxAmmo:
        //         player.GetComponent<ShootingController>().UpgradeMaxAmmo((int)data.value);
        //         break;
        //     case UpgradeData.UpgradeType.DoubleJump:
        //         player.GetComponent<PlayerMovement>().EnableDoubleJump();
        //         break;
        //     case UpgradeData.UpgradeType.SlowMoPower:
        //         // Buscamos al TimeManager en la escena
        //         FindFirstObjectByType<TimeManager>().UpgradeSlowMo(data.value);
        //         break;
        // }
    }
}