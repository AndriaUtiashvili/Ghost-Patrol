using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private FlashlightController flashlightController;

    private bool isPaused;
    private bool movementWasEnabled;
    private bool flashlightWasEnabled;

    private void Awake()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null ||
            !Keyboard.current.pKey.wasPressedThisFrame)
        {
            return;
        }

        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameEnded)
        {
            return;
        }

        // Don't open pause while waiting on the title screen.
        if (!isPaused && Time.timeScale == 0f)
        {
            return;
        }

        if (pausePanel == null)
        {
            return;
        }

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;

        if (playerController != null)
        {
            movementWasEnabled = playerController.enabled;
            playerController.enabled = false;
        }

        if (flashlightController != null)
        {
            flashlightWasEnabled = flashlightController.enabled;
            flashlightController.enabled = false;
        }

        Time.timeScale = 0f;

        pausePanel.SetActive(true);
        pausePanel.transform.SetAsLastSibling();
    }

    private void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        if (playerController != null)
        {
            playerController.enabled = movementWasEnabled;
        }

        if (flashlightController != null)
        {
            flashlightController.enabled = flashlightWasEnabled;
        }
    }
}