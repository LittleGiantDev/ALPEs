using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UpgradeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UpgradeData data;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;
    public Button btn;
    public CanvasGroup canvasGroup;

    public Action<UpgradeData> onSelected;

    public void Setup(UpgradeData newData)
    {
        data = newData;
        nameText.text = data.upgradeName;
        descriptionText.text = data.description;
        priceText.text = data.price.ToString();
        
        transform.localScale = Vector3.one;
        canvasGroup.alpha = 1f;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => onSelected?.Invoke(data));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(0.95f, 0.1f).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1f, 0.1f).SetUpdate(true);
    }

    public void PlaySelectionAnimation(bool isSelected)
    {
        if (isSelected)
        {
            transform.DOScale(1.1f, 0.3f).SetUpdate(true).SetEase(Ease.OutBack);
        }
        else
        {
            canvasGroup.DOFade(0f, 0.2f).SetUpdate(true);
            transform.DOScale(0f, 0.2f).SetUpdate(true).SetEase(Ease.InBack);
        }
    }
}