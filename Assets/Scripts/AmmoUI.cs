using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image reloadBar;
    private int storedMaxAmmo;

    private void Start()
    {
        if (reloadBar != null)
        {
            reloadBar.fillAmount = 0f;
            reloadBar.gameObject.SetActive(false);
        }

        GameEvents.OnAmmoChanged += UpdateAmmoText;
        GameEvents.OnReloadStarted += ShowReloadingState;
        GameEvents.OnReloadFinished += HideReloadingState;
    }

    private void OnDestroy()
    {
        GameEvents.OnAmmoChanged -= UpdateAmmoText;
        GameEvents.OnReloadStarted -= ShowReloadingState;
        GameEvents.OnReloadFinished -= HideReloadingState;
    }

    private void UpdateAmmoText(int current, int max)
    {
        storedMaxAmmo = max;
        ammoText.text = $"{current}/{max}";
        ammoText.color = Color.white;
        
        ammoText.transform.DOComplete();
        ammoText.transform.DOPunchScale(Vector3.one * 0.3f, 0.1f, 10, 1f);
    }

    private void ShowReloadingState(float reloadTime)
    {
        ammoText.color = Color.gray;
        
        StartCoroutine(TypeTextRoutine("RELOADING...", reloadTime));

        if (reloadBar != null)
        {
            reloadBar.gameObject.SetActive(true);
            reloadBar.DOComplete();
            reloadBar.fillAmount = 0f;
            reloadBar.DOFillAmount(1f, reloadTime).SetEase(Ease.Linear);
        }
    }

    private IEnumerator TypeTextRoutine(string textToType, float duration)
    {
        ammoText.text = "";
        float delayBetweenLetters = duration / textToType.Length;
        
        foreach (char letter in textToType)
        {
            ammoText.text += letter;
            yield return new WaitForSeconds(delayBetweenLetters); 
        }
    }

    private void HideReloadingState()
    {
        if (reloadBar != null)
        {
            reloadBar.gameObject.SetActive(false);
        }
        
        ammoText.transform.DOComplete();
        ammoText.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, 10, 1f);
    }
}