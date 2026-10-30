using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RestartButton : MonoBehaviour
{
    [SerializeField] private float clickDelay = 0.6f;

    private Button button;
    private bool restarting;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(RestartGame);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(RestartGame);
    }

    private void RestartGame()
    {
        if (restarting)
        {
            return;
        }

        restarting = true;
        button.interactable = false;

        StartCoroutine(RestartAfterClick());
    }

    private IEnumerator RestartAfterClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(GameSound.MenuClick);

            yield return new WaitForSecondsRealtime(clickDelay);
        }

        StartMenuController.SkipTitleOnce = true;
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }
}