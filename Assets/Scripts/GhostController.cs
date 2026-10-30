using UnityEngine;

public class GhostController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private int health = 1;
    [SerializeField] private float escapeY = -5.5f;
    [SerializeField] private int scoreValue = 100;

    private PlayerHealth playerHealth;
    private InvisibleGhostController invisibleGhost;
    private bool isResolved;

    public void SetHealth(int amount)
    {
        health = Mathf.Max(1, amount);
    }

    private void Awake()
    {
        invisibleGhost = GetComponent<InvisibleGhostController>();
    }

    private void Start()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
    }

    private void Update()
    {
        if (isResolved ||
            (GameManager.Instance != null &&
             GameManager.Instance.IsGameEnded))
        {
            return;
        }

        transform.Translate(
            Vector3.down * speed * Time.deltaTime,
            Space.World
        );

        if (transform.position.y < escapeY)
        {
            DamagePlayerAndDisappear(playerHealth, true);
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
            if (invisibleGhost != null &&
                !FlashlightController.IsFlashlightOn)
            {
                return;
            }

            if (!projectile.TryConsume())
            {
                return;
            }

            health -= projectile.Damage;

            if (health <= 0)
            {
                isResolved = true;

                PlaySound(GameSound.GhostDefeated);

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore(scoreValue);
                }

                Destroy(gameObject);
            }
            else
            {
                // Only play the hit sound if the ghost survives.
                PlaySound(GameSound.GhostHit);
            }

            return;
        }

        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            DamagePlayerAndDisappear(player, false);
        }
    }

    private void DamagePlayerAndDisappear(
        PlayerHealth player,
        bool escaped)
    {
        if (isResolved)
        {
            return;
        }

        isResolved = true;

        if (escaped)
        {
            PlaySound(GameSound.GhostEscaped);
        }

        if (player != null)
        {
            player.TakeDamage(1);
        }

        Destroy(gameObject);
    }

    private void PlaySound(GameSound sound)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(sound);
        }
    }
}