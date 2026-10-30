using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FlashlightController : MonoBehaviour
{
    public static bool IsFlashlightOn { get; private set; }

    [Header("Flashlight")]
    [SerializeField] private GameObject flashlightGlow;
    [SerializeField] private float maximumEnergy = 3f;
    [SerializeField] private float drainRate = 1f;
    [SerializeField] private float rechargeRate = 0.5f;

    [Header("Energy Meter")]
    [SerializeField] private Slider energyBar;

    private float currentEnergy;
    private bool mustReleaseKey;

    private void Awake()
    {
        maximumEnergy = Mathf.Max(0.1f, maximumEnergy);
        currentEnergy = maximumEnergy;

        SetFlashlight(false, false);
        UpdateEnergyBar();
    }

    private void Update()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameEnded)
        {
            SetFlashlight(false, false);
            return;
        }

        if (Time.timeScale == 0f)
        {
            return;
        }

        bool flashlightPressed =
            Keyboard.current != null &&
            Keyboard.current.fKey.isPressed;

        if (!flashlightPressed)
        {
            mustReleaseKey = false;

            currentEnergy += rechargeRate * Time.deltaTime;
            SetFlashlight(false, true);
        }
        else if (!mustReleaseKey && currentEnergy > 0f)
        {
            currentEnergy -= drainRate * Time.deltaTime;

            if (currentEnergy <= 0f)
            {
                currentEnergy = 0f;
                mustReleaseKey = true;
                SetFlashlight(false, true);
            }
            else
            {
                SetFlashlight(true, true);
            }
        }
        else
        {
            SetFlashlight(false, true);
        }

        currentEnergy = Mathf.Clamp(
            currentEnergy,
            0f,
            maximumEnergy
        );

        UpdateEnergyBar();
    }

    private void SetFlashlight(bool isOn, bool playSound)
    {
        bool changed = IsFlashlightOn != isOn;

        IsFlashlightOn = isOn;

        if (flashlightGlow != null &&
            flashlightGlow.activeSelf != isOn)
        {
            flashlightGlow.SetActive(isOn);
        }

        // Only click when the state changes, not every frame.
        if (changed && playSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(GameSound.Flashlight);
        }
    }

    private void UpdateEnergyBar()
    {
        if (energyBar == null)
        {
            return;
        }

        energyBar.minValue = 0f;
        energyBar.maxValue = maximumEnergy;
        energyBar.value = currentEnergy;
    }

    private void OnDisable()
    {
        // Pausing or opening a menu turns it off silently.
        SetFlashlight(false, false);
    }
}