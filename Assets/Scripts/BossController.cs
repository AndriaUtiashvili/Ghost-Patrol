using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maximumHealth = 200;
    [SerializeField] private int scoreValue = 2000;

    [Header("Movement")]
    [SerializeField] private float horizontalSpeed = 2.5f;
    [SerializeField] private float horizontalLimit = 6.5f;
    [SerializeField] private float descentSpeed = 0.15f;
    [SerializeField] private float townBoundaryY = -3f;

    [Header("Hit Feedback")]
    [SerializeField] private float hitSoundCooldown = 0.12f;

    private int currentHealth;
    private float direction = 1f;
    private bool isResolved;
    private float nextHitSoundTime;

    private PlayerHealth playerHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float flashRemaining;

    private void Start()
    {
        currentHealth = Mathf.Max(1, maximumHealth);
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        UpdateHealthBar();
    }

    private void Update()
    {
        if (isResolved ||
            Time.timeScale == 0f ||
            (GameManager.Instance != null &&
             GameManager.Instance.IsGameEnded))
        {
            return;
        }

        Vector3 position = transform.position;

        position.x += direction * horizontalSpeed * Time.deltaTime;
        position.y -= descentSpeed * Time.deltaTime;

        if (position.x >= horizontalLimit)
        {
            position.x = horizontalLimit;
            direction = -1f;
        }
        else if (position.x <= -horizontalLimit)
        {
            position.x = -horizontalLimit;
            direction = 1f;
        }

        transform.position = position;

        if (flashRemaining > 0f)
        {
            flashRemaining -= Time.deltaTime;

            if (flashRemaining <= 0f && spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }

        if (position.y <= townBoundaryY)
        {
            ReachTown();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isResolved ||
            Time.timeScale == 0f ||
            (GameManager.Instance != null &&
             GameManager.Instance.IsGameEnded))
        {
            return;
        }

        ProjectileController projectile =
            other.GetComponent<ProjectileController>();

        if (projectile != null)
        {
            if (!projectile.TryConsume())
            {
                return;
            }

            currentHealth = Mathf.Max(
                0,
                currentHealth - projectile.Damage
            );

            UpdateHealthBar();

            if (currentHealth <= 0)
            {
                DefeatBoss();
                return;
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.color =
                    new Color(1f, 0.65f, 0.65f);

                flashRemaining = 0.08f;
            }

            // Both projectiles still deal damage.
            // The cooldown only prevents overlapping hit sounds.
            if (Time.time >= nextHitSoundTime)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.Play(GameSound.GhostHit);
                }

                nextHitSoundTime =
                    Time.time + hitSoundCooldown;
            }

            return;
        }

        if (other.GetComponent<PlayerHealth>() != null)
        {
            ReachTown();
        }
    }

    private void UpdateHealthBar()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateBossHealth(
                currentHealth,
                maximumHealth
            );
        }
    }

    private void DefeatBoss()
    {
        isResolved = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);

            // GameManager plays the victory sound.
            GameManager.Instance.ShowWin();
        }

        Destroy(gameObject);
    }

    private void ReachTown()
    {
        if (isResolved)
        {
            return;
        }

        isResolved = true;

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(int.MaxValue);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowGameOver();
        }

        Destroy(gameObject);
    }
}