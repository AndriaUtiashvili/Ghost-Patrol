using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;

    public int Damage { get; private set; } = 1;

    private bool consumed;
    private static Material sharedTrailMaterial;

    public void Configure(int wave)
    {
        Color color = new Color(0.15f, 0.7f, 1f);
        Vector3 size = new Vector3(0.18f, 0.35f, 1f);
        bool useTrail = false;

        Damage = 1;

        switch (wave)
        {
            case 2:
                color = new Color(0.75f, 0.3f, 1f);
                useTrail = true;
                break;

            case 3:
                color = new Color(0.2f, 1f, 0.35f);
                break;

            case 4:
                color = new Color(1f, 0.2f, 0.15f);
                size = new Vector3(0.32f, 0.55f, 1f);
                Damage = 2;
                break;

            case 5:
                color = new Color(1f, 0.2f, 0.15f);
                size = new Vector3(0.32f, 0.55f, 1f);
                Damage = 2;
                useTrail = true;
                break;
        }

        transform.localScale = size;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        if (sprite != null)
        {
            sprite.color = color;
            sprite.sortingOrder = 3;
        }

        TrailRenderer trail = GetComponent<TrailRenderer>();

        if (!useTrail)
        {
            if (trail != null)
            {
                trail.emitting = false;
                trail.Clear();
            }

            return;
        }

        if (trail == null)
        {
            trail = gameObject.AddComponent<TrailRenderer>();
        }

        if (sharedTrailMaterial == null)
        {
            Shader shader = Shader.Find("Sprites/Default");

            if (shader != null)
            {
                sharedTrailMaterial = new Material(shader);
            }
        }

        if (sharedTrailMaterial != null)
        {
            trail.sharedMaterial = sharedTrailMaterial;
        }

        trail.time = 0.15f;
        trail.minVertexDistance = 0.03f;
        trail.startWidth = wave == 5 ? 0.2f : 0.1f;
        trail.endWidth = 0f;
        trail.startColor = color;
        trail.endColor = new Color(color.r, color.g, color.b, 0f);
        trail.sortingOrder = 2;
        trail.emitting = true;
        trail.Clear();
    }

    public bool TryConsume()
    {
        if (consumed)
        {
            return false;
        }

        consumed = true;

        Collider2D projectileCollider = GetComponent<Collider2D>();

        if (projectileCollider != null)
        {
            projectileCollider.enabled = false;
        }

        Destroy(gameObject);
        return true;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (!consumed)
        {
            transform.Translate(
                Vector3.up * speed * Time.deltaTime,
                Space.World
            );
        }
    }
}