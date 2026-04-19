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
        if (newData == null)
        {
            return;
        }

        data = newData;

        if (nameText != null)
        {
            nameText.text = data.upgradeName;
        }

        if (descriptionText != null)
        {
            descriptionText.text = data.description;
        }

        if (priceText != null)
        {
            priceText.text = data.price.ToString();
        }
        
        transform.localScale = Vector3.one;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(HandleClick);
        }
    }

    private void HandleClick()
    {
        if (onSelected != null)
        {
            onSelected.Invoke(data);
        }
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
            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0f, 0.2f).SetUpdate(true);
            }
            transform.DOScale(0f, 0.2f).SetUpdate(true).SetEase(Ease.InBack);
        }
    }
}