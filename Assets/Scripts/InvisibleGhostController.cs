using UnityEngine;

public class InvisibleGhostController : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float hiddenAlpha = 0f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisibility();
    }

    private void Update()
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        Color color = spriteRenderer.color;
        color.a = FlashlightController.IsFlashlightOn
            ? 1f
            : hiddenAlpha;

        spriteRenderer.color = color;
    }
}