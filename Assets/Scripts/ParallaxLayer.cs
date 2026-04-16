using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Vector2 parallaxEffect;
    [SerializeField] private bool infiniteHorizontal = true;
    [SerializeField] private bool infiniteVertical = true;

    private Transform camTransform;
    private Vector3 startPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;

    private void Start()
    {
        camTransform = Camera.main.transform;
        startPosition = transform.position;
        
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        Vector3 delta = camTransform.position;
        float distX = delta.x * parallaxEffect.x;
        float distY = delta.y * parallaxEffect.y;

        transform.position = new Vector3(startPosition.x + distX, startPosition.y + distY, transform.position.z);

        if (infiniteHorizontal)
        {
            if (Mathf.Abs(camTransform.position.x - transform.position.x) >= textureUnitSizeX)
            {
                float offsetPositionX = (camTransform.position.x - transform.position.x) % textureUnitSizeX;
                transform.position = new Vector3(camTransform.position.x + offsetPositionX, transform.position.y, transform.position.z);
                startPosition.x = transform.position.x - (camTransform.position.x * parallaxEffect.x);
            }
        }

        if (infiniteVertical)
        {
            if (Mathf.Abs(camTransform.position.y - transform.position.y) >= textureUnitSizeY)
            {
                float offsetPositionY = (camTransform.position.y - transform.position.y) % textureUnitSizeY;
                transform.position = new Vector3(transform.position.x, camTransform.position.y + offsetPositionY, transform.position.z);
                startPosition.y = transform.position.y - (camTransform.position.y * parallaxEffect.y);
            }
        }
    }
}