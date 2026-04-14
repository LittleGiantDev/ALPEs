using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class AimController : MonoBehaviour
{
    [SerializeField] private RectTransform rawImageRect;
    [SerializeField] private Camera gameCamera;

    private void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImageRect, mousePos, null, out Vector2 localPoint);

        Vector2 normalizedPoint = new Vector2(
            (localPoint.x - rawImageRect.rect.x) / rawImageRect.rect.width,
            (localPoint.y - rawImageRect.rect.y) / rawImageRect.rect.height
        );

        Vector3 worldPos = gameCamera.ViewportToWorldPoint(new Vector3(normalizedPoint.x, normalizedPoint.y, gameCamera.nearClipPlane));
        worldPos.z = 0f;

        Vector3 lookDirection = worldPos - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}