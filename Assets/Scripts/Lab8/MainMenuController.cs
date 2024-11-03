using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject mainMenuPanel;

    [Header("Options")]
    public TMP_Dropdown maxFpsDropdown;
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fpsCounterToggle;
    public TextMeshProUGUI fpsCounterText;

    private Resolution[] resolutions;

    private int[] availableFpsOptions = { 30, 60, 120, 144, 240, -1 };
    private void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        int defaultResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution res = resolutions[i];
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(res.width + " x " + res.height));
            if (res.width == 1920 && res.height == 1080)
            {
                defaultResolutionIndex = i;
            }
        }

        resolutionDropdown.value = defaultResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        fpsCounterToggle.onValueChanged.AddListener(SetFPSCounter);
        fpsCounterText.gameObject.SetActive(fpsCounterToggle.isOn);

        maxFpsDropdown.ClearOptions();
        foreach (int fps in availableFpsOptions)
        {
            string label = fps == -1 ? "Unlimited" : fps + " FPS";
            maxFpsDropdown.options.Add(new TMP_Dropdown.OptionData(label));
        }
        maxFpsDropdown.onValueChanged.AddListener(SetMaxFPS);
        maxFpsDropdown.value = GetCurrentFpsIndex();
        maxFpsDropdown.RefreshShownValue();
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void OpenMainMenu()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                return i;
            }
        }
        return 0;
    }

    private void SetFPSCounter(bool isEnabled)
    {
        fpsCounterText.gameObject.SetActive(isEnabled);
    }

    private void SetMaxFPS(int index)
    {
        int fps = availableFpsOptions[index];
        Application.targetFrameRate = fps;
    }

    private int GetCurrentFpsIndex()
    {
        int currentFps = Application.targetFrameRate;
        for (int i = 0; i < availableFpsOptions.Length; i++)
        {
            if (availableFpsOptions[i] == currentFps)
            {
                return i;
            }
        }
        return availableFpsOptions.Length - 1;
    }
}
