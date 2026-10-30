using System.Collections;
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    [Header("Ghost Prefabs")]
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private GameObject invisibleGhostPrefab;
    [SerializeField] private GameObject fastGhostPrefab;

    [Header("Boss")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private float bossSpawnY = 3f;

    [Header("Waves")]
    [SerializeField] private int totalWaves = 5;
    [SerializeField] private float waveAnnouncementDuration = 2f;
    [SerializeField] private float delayBetweenWaves = 4f;

    [Header("Spawn Area")]
    [SerializeField] private float spawnY = 4.5f;
    [SerializeField] private float minimumX = -7.5f;
    [SerializeField] private float maximumX = 7.5f;

    private PlayerController playerController;

    private bool GameEnded =>
        GameManager.Instance != null &&
        GameManager.Instance.IsGameEnded;

    private void Start()
    {
        if (ghostPrefab == null)
        {
            Debug.LogError("Assign the regular Ghost prefab.");
            return;
        }

        if (bossPrefab == null ||
            bossPrefab.GetComponent<BossController>() == null)
        {
            Debug.LogError(
                "Assign a GhostKing prefab with BossController."
            );

            return;
        }

        playerController = FindAnyObjectByType<PlayerController>();
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        for (int wave = 1; wave <= totalWaves; wave++)
        {
            if (GameEnded)
            {
                yield break;
            }

            if (playerController != null)
            {
                playerController.SetWeaponWave(wave);
            }

            // Wave 1 uses the starting weapon.
            if (wave > 1)
            {
                PlaySound(GameSound.WeaponUpgrade);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetWave(wave);
            }

            yield return new WaitForSeconds(
                waveAnnouncementDuration
            );

            int ghostsThisWave = 3 + wave * 2;

            float spawnInterval = Mathf.Max(
                0.4f,
                1.3f - (wave - 1) * 0.15f
            );

            for (int i = 0; i < ghostsThisWave; i++)
            {
                if (GameEnded)
                {
                    yield break;
                }

                SpawnGhost(wave, i);

                yield return new WaitForSeconds(spawnInterval);
            }

            yield return new WaitUntil(() =>
                GameEnded ||
                FindObjectsByType<GhostController>().Length == 0
            );

            if (GameEnded)
            {
                yield break;
            }

            PlaySound(GameSound.WaveCompleted);

            yield return new WaitForSeconds(delayBetweenWaves);
        }

        if (GameEnded)
        {
            yield break;
        }

        PlaySound(GameSound.BossArrival);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AnnounceBoss();
        }

        yield return new WaitForSeconds(
            waveAnnouncementDuration
        );

        if (!GameEnded)
        {
            Instantiate(
                bossPrefab,
                new Vector3(0f, bossSpawnY, 0f),
                Quaternion.identity
            );
        }
    }

    private void SpawnGhost(int wave, int ghostIndex)
    {
        GameObject prefabToSpawn = ghostPrefab;
        bool isRegularGhost = true;

        if (wave >= 3 &&
            ghostIndex % 3 == 2 &&
            invisibleGhostPrefab != null)
        {
            prefabToSpawn = invisibleGhostPrefab;
            isRegularGhost = false;
        }
        else if (wave >= 2 &&
                 ghostIndex % 3 == 1 &&
                 fastGhostPrefab != null)
        {
            prefabToSpawn = fastGhostPrefab;
            isRegularGhost = false;
        }

        Vector3 position = new Vector3(
            Random.Range(minimumX, maximumX),
            spawnY,
            0f
        );

        GameObject ghost = Instantiate(
            prefabToSpawn,
            position,
            Quaternion.identity
        );

        GhostController controller =
            ghost.GetComponent<GhostController>();

        if (controller != null)
        {
            int ghostHealth = isRegularGhost
                ? Mathf.Max(1, wave - 1)
                : 1;

            controller.SetHealth(ghostHealth);
        }
    }

    private void PlaySound(GameSound sound)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(sound);
        }
    }
}