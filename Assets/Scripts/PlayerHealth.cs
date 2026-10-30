using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maximumHealth = 3;

    [Header("Heart Images — Left to Right")]
    [SerializeField] private Image[] hearts;

    [Header("Heart Colors")]
    [SerializeField] private Color fullHeartColor = Color.white;

    [SerializeField] private Color emptyHeartColor =
        new Color(0.5f, 0.5f, 0.5f, 0.8f);

    private int currentHealth;
    private PlayerController playerController;
    private bool isDead;

    private void Start()
    {
        maximumHealth = Mathf.Max(1, maximumHealth);
        currentHealth = maximumHealth;

        playerController = GetComponent<PlayerController>();

        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
        {
            return;
        }

        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameEnded)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);

        UpdateHearts();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(GameSound.PlayerDamage);
        }

        if (currentHealth <= 0)
        {
            isDead = true;

            if (playerController != null)
            {
                playerController.enabled = false;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ShowGameOver();
            }
        }
    }

    private void UpdateHearts()
    {
        if (hearts == null)
        {
            return;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null)
            {
                continue;
            }

            hearts[i].gameObject.SetActive(i < maximumHealth);

            hearts[i].color = i < currentHealth
                ? fullHeartColor
                : emptyHeartColor;
        }
    }
}