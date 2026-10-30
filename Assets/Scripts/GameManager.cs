using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsGameEnded { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Slider bossHealthBar;

    [Header("Announcements")]
    [SerializeField] private float waveDisplayDuration = 2f;
    [SerializeField] private float fadeDuration = 0.35f;

    private int score;
    private Coroutine announcement;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;

        if (waveText != null)
        {
            waveText.gameObject.SetActive(false);
        }

        if (endText != null)
        {
            endText.gameObject.SetActive(false);
        }

        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }

        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(false);
        }

        UpdateScoreText();
    }

    public void AddScore(int points)
    {
        if (IsGameEnded)
        {
            return;
        }

        score += points;
        UpdateScoreText();
    }

    public void SetWave(int wave)
    {
        Announce(
            "WAVE " + wave +
            "\n<size=40%>PROTECT THE TOWN</size>"
        );
    }

    public void AnnounceBoss()
    {
        Announce(
            "GHOST KING" +
            "\n<size=40%>FINAL BATTLE</size>"
        );
    }

    private void Announce(string message)
    {
        if (IsGameEnded || waveText == null)
        {
            return;
        }

        HideAnnouncement();

        announcement = StartCoroutine(
            ShowAnnouncement(message)
        );
    }

    private IEnumerator ShowAnnouncement(string message)
    {
        waveText.richText = true;
        waveText.text = message;
        waveText.alpha = 0f;
        waveText.gameObject.SetActive(true);

        float duration = Mathf.Max(0.1f, waveDisplayDuration);

        float fade = Mathf.Clamp(
            fadeDuration,
            0.01f,
            duration * 0.5f
        );

        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float elapsed = Time.time - startTime;

            float fadeIn = Mathf.Clamp01(elapsed / fade);
            float fadeOut = Mathf.Clamp01(
                (duration - elapsed) / fade
            );

            waveText.alpha = Mathf.SmoothStep(
                0f,
                1f,
                Mathf.Min(fadeIn, fadeOut)
            );

            yield return null;
        }

        waveText.alpha = 0f;
        waveText.gameObject.SetActive(false);
        announcement = null;
    }

    private void HideAnnouncement()
    {
        if (announcement != null)
        {
            StopCoroutine(announcement);
            announcement = null;
        }

        if (waveText != null)
        {
            waveText.alpha = 0f;
            waveText.gameObject.SetActive(false);
        }
    }

    public void UpdateBossHealth(int current, int maximum)
    {
        if (bossHealthBar == null || IsGameEnded)
        {
            return;
        }

        bossHealthBar.gameObject.SetActive(true);
        bossHealthBar.minValue = 0f;
        bossHealthBar.maxValue = maximum;
        bossHealthBar.value = current;
    }

    public void ShowWin()
    {
        FinishGame(true);
    }

    public void ShowGameOver()
    {
        FinishGame(false);
    }

    private void FinishGame(bool won)
    {
        // Prevent duplicate ending screens and sounds.
        if (IsGameEnded)
        {
            return;
        }

        IsGameEnded = true;
        HideAnnouncement();

        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(false);
        }

        if (endPanel != null)
        {
            endPanel.SetActive(true);
            endPanel.transform.SetAsLastSibling();
        }

        if (endText != null)
        {
            endText.richText = true;

            endText.text = won
                ? "<color=#FFD166><b>TOWN SAVED!</b></color>"
                : "<color=#FF7070><b>GAME OVER</b></color>";

            endText.text +=
                "\n<size=65%><color=#FFFFFF>Final Score: " +
                score + "</color></size>";

            endText.gameObject.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(
                won ? GameSound.Victory : GameSound.GameOver
            );
        }

        // Freezes gameplay; audio continues playing.
        Time.timeScale = 0f;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            Time.timeScale = 1f;
        }
    }
}