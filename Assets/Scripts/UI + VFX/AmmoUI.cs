using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    private int storedMaxAmmo;

    private void Start()
    {

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
        
        StartCoroutine(ReloadType("reloading...", reloadTime));
    }

    private IEnumerator ReloadType(string text, float duration)
    {
        ammoText.text = "";
        float delay = duration / text.Length;
        
        foreach (char letter in text)
        {
            ammoText.text += letter;
            yield return new WaitForSeconds(delay); 
        }
    }

    private void HideReloadingState()
    {
        
        ammoText.transform.DOComplete();
        ammoText.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, 10, 1f);
    }
}