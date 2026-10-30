using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
public class StartMenuController : MonoBehaviour
{
    public static bool SkipTitleOnce { get; set; }

    [Header("Menu")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private Button startButton;

    [Header("Gameplay")]
    [SerializeField] private GameObject ghostSpawner;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private FlashlightController flashlightController;

    private bool started;

    private void Awake()
    {
        if (startPanel == null ||
            startButton == null ||
            ghostSpawner == null ||
            playerController == null ||
            flashlightController == null)
        {
            Debug.LogError(
                "Assign all StartMenuController fields."
            );

            SkipTitleOnce = false;
            return;
        }

        startButton.onClick.AddListener(StartGame);

        // Play Again bypasses the title screen once.
        if (SkipTitleOnce)
        {
            SkipTitleOnce = false;
            StartGame();
            return;
        }

        ghostSpawner.SetActive(false);
        playerController.enabled = false;
        flashlightController.enabled = false;

        startPanel.SetActive(true);
        startPanel.transform.SetAsLastSibling();

        Time.timeScale = 0f;
    }

    private void StartGame()
    {
        if (started)
        {
            return;
        }

        started = true;

        startPanel.SetActive(false);
        Time.timeScale = 1f;

        playerController.enabled = true;
        flashlightController.enabled = true;
        ghostSpawner.SetActive(true);
    }

    private void OnDestroy()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(StartGame);
        }
    }
}