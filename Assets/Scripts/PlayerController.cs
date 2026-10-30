using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float horizontalLimit = 7.5f;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootCooldown = 0.25f;
    [SerializeField] private float projectileSpawnOffset = 1.3f;
    [SerializeField] private float doubleShotSpacing = 0.3f;

    private float nextShootTime;
    private int weaponWave = 1;

    public void SetWeaponWave(int wave)
    {
        weaponWave = Mathf.Clamp(wave, 1, 5);
        nextShootTime = Time.time;
    }

    private void Update()
    {
        if (Time.timeScale == 0f ||
            (GameManager.Instance != null &&
             GameManager.Instance.IsGameEnded))
        {
            return;
        }

        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        float direction = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed ||
                Keyboard.current.leftArrowKey.isPressed)
            {
                direction -= 1f;
            }

            if (Keyboard.current.dKey.isPressed ||
                Keyboard.current.rightArrowKey.isPressed)
            {
                direction += 1f;
            }
        }

        Vector3 position = transform.position;

        position.x += direction * moveSpeed * Time.deltaTime;
        position.x = Mathf.Clamp(
            position.x,
            -horizontalLimit,
            horizontalLimit
        );

        transform.position = position;
    }

    private void HandleShooting()
    {
        if (projectilePrefab == null ||
            Keyboard.current == null ||
            !Keyboard.current.spaceKey.isPressed ||
            Time.time < nextShootTime)
        {
            return;
        }

        Vector3 spawnPosition =
            transform.position + Vector3.up * projectileSpawnOffset;

        bool doubleShot = weaponWave == 3 || weaponWave == 5;

        if (doubleShot)
        {
            float offset = doubleShotSpacing * 0.5f;

            FireProjectile(spawnPosition + Vector3.left * offset);
            FireProjectile(spawnPosition + Vector3.right * offset);
        }
        else
        {
            FireProjectile(spawnPosition);
        }

        // One sound per firing action, including double shots.
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(GameSound.Blaster);
        }

        float cooldown = shootCooldown;

        switch (weaponWave)
        {
            case 2:
                cooldown *= 0.65f;
                break;

            case 3:
                cooldown *= 0.85f;
                break;

            case 4:
                cooldown *= 0.9f;
                break;

            case 5:
                cooldown *= 0.7f;
                break;
        }

        nextShootTime = Time.time + Mathf.Max(0.05f, cooldown);
    }

    private void FireProjectile(Vector3 position)
    {
        GameObject shot = Instantiate(
            projectilePrefab,
            position,
            Quaternion.identity
        );

        ProjectileController projectile =
            shot.GetComponent<ProjectileController>();

        if (projectile != null)
        {
            projectile.Configure(weaponWave);
        }
    }
}