using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeSlot : MonoBehaviour
{
    public UpgradeData data;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button btn;

    public Action<UpgradeData> onSelected;

    public void Setup(UpgradeData newData)
    {
        data = newData;
        nameText.text = data.upgradeName;
        priceText.text = data.price.ToString();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => onSelected?.Invoke(data));
    }
}