using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Options_ResolutionDropdown : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown fpsLimitDropdown;
    [SerializeField] TMP_Dropdown fullscreenDropdown;

    [SerializeField] Toggle vSyncCheckmark;

    int[] FPSLimits = { -1, 30, 60, 90, 120, 144, -10 };
    FullScreenMode[] fullScreenModes = { FullScreenMode.Windowed, FullScreenMode.MaximizedWindow, FullScreenMode.FullScreenWindow, FullScreenMode.ExclusiveFullScreen };
    List<string> fullScreenStrings = new();
    List<Resolution> resolutions = new();

    void Awake()
    {
        SetDefaults();

        //Restore prev settings
        int width = PlayerPrefs.GetInt("ResolutionX");
        int height = PlayerPrefs.GetInt("ResolutionY");
        string fullScreen = PlayerPrefs.GetString("FullscreenMode");

        Screen.SetResolution(width, height, (FullScreenMode)Enum.Parse(typeof(FullScreenMode), fullScreen));
    }

    void SetDefaults()
    {
        int width = PlayerPrefs.GetInt("ResolutionX");
        int height = PlayerPrefs.GetInt("ResolutionY");
        string fullScreen = PlayerPrefs.GetString("FullscreenMode");

        if (width == 0)
        {
            PlayerPrefs.SetInt("ResolutionX", Screen.width);
        }
        if (height == 0)
        {
            PlayerPrefs.SetInt("ResolutionY", Screen.height);
        }
        if (fullScreen == "")
        {
            PlayerPrefs.SetString("FullscreenMode", Screen.fullScreenMode.ToString());
        }
    }

    void OnEnable() //We update this info every onenable to ensure the settings are accurate and in the event the user switches monitors
    {
        InitResolutionDropdown();
        InitFPSLimitDropDown();
        InitWindowModeDropDown();
        UpdateDropdowns();
    }

    void InitResolutionDropdown()
    {
        Resolution[] supportedResolutions = Screen.resolutions;
        List<string> options = new();
        resolutions = new();

        for (int i = supportedResolutions.Length - 1; i >= 0; i--)
        {
            if (!options.Contains(supportedResolutions[i].width + "x" + supportedResolutions[i].height)) //we do it this way because there are duplicate resolutions, varying only in refresh rate which is irrelevant to us
            {
                options.Add(supportedResolutions[i].width + "x" + supportedResolutions[i].height);
                resolutions.Add(supportedResolutions[i]);
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
    }

    void InitFPSLimitDropDown()
    {
        fullScreenStrings = new();

        foreach (int limit in FPSLimits)
        {
            if (limit == -1)
            {
                fullScreenStrings.Add("Unlimited");

            }
            else if (limit == -10)
            {
                fullScreenStrings.Add("VSync (Monitor Refresh Rate)");
            }
            else fullScreenStrings.Add(limit.ToString());
        }


        fpsLimitDropdown.ClearOptions();
        fpsLimitDropdown.AddOptions(fullScreenStrings);
    }

    void InitWindowModeDropDown()
    {
        List<string> options = new();

        for (int i = 0; i < fullScreenModes.Length; i++)
        {
            switch (fullScreenModes[i])
            {
                case FullScreenMode.Windowed:
                    options.Add("Windowed");
                    break;

                case FullScreenMode.MaximizedWindow:
                    options.Add("Maximized");
                    break;

                case FullScreenMode.FullScreenWindow:
                    options.Add("Borderless Fullscreen");
                    break;

                case FullScreenMode.ExclusiveFullScreen:
                    options.Add("Exclusive Fullscreen");
                    break;
            }
        }

        fullscreenDropdown.ClearOptions();
        fullscreenDropdown.AddOptions(options);
    }

    public void UpdateResolution(int index = 999)
    {
        if (index > resolutions.Count) index = 0;

        int width = resolutions[index].width;
        int height = resolutions[index].height;

        PlayerPrefs.SetInt("ResolutionX", width);
        PlayerPrefs.SetInt("ResolutionY", height);

        Debug.Log(resolutions[index]);

        Screen.SetResolution(width, height, Screen.fullScreenMode);

    }

    // public void UpdateRefreshRate(int index)
    // {
    //     Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreenMode, refreshRates[index]);
    //     Debug.Log(refreshRates[index]);
    // }

    public void UpdateFPSLimit(int index)
    {
        int limit = FPSLimits[index];
        Debug.Log(limit);

        Application.targetFrameRate = limit;

        if (limit == -10)
        {
            UpdateVsync(true);
        }
        else
        {
            UpdateVsync(false, false);
        }
        // Debug.Log(int.Parse(limit));

    }

    public void UpdateFullScreen()
    {
        // Screen.SetResolution(Screen.width, Screen.height, fullScreen);
        // Screen.fullScreen = fullScreen;


    }

    void UpdateDropdowns()
    {
        //Get current resolution selected

        if (!resolutionDropdown.enabled || !fpsLimitDropdown.enabled) return;

        var listAvailableStrings = resolutionDropdown.options.Select(option => option.text).ToList();

        int curIndex = listAvailableStrings.IndexOf(Screen.width + "x" + Screen.height);

        // if (resolutionDropdown.value != curIndex)
        // {
        //     resolutionDropdown.value = curIndex;
        // }

        listAvailableStrings.Clear();


        //Get current target fps

        listAvailableStrings = fpsLimitDropdown.options.Select(option => option.text).ToList();

        string currentLimit = Application.targetFrameRate.ToString();

        if (currentLimit == (-1).ToString())
        {
            currentLimit = "Unlimited";
        }

        vSyncCheckmark.isOn = QualitySettings.vSyncCount > 0;

        if (vSyncCheckmark.isOn)
        {
            curIndex = listAvailableStrings.IndexOf("VSync (Monitor Refresh Rate)");
        }
        else
        {
            curIndex = listAvailableStrings.IndexOf(currentLimit);
        }

        if (fpsLimitDropdown.value != curIndex)
        {
            fpsLimitDropdown.value = curIndex;
        }


        //get current windowed mode
        // listAvailableStrings = fullscreenDropdown.options.Select(option => option.text).ToList();
        // curIndex = Array.IndexOf(fullScreenModes, Screen.fullScreenMode);

        // if (resolutionDropdown.value != curIndex)
        // {
        //     resolutionDropdown.value = curIndex;
        // }
    }

    public void UpdateVsyncUI(bool on)
    {
        UpdateVsync(on);
    }

    void UpdateVsync(bool on, bool UpdateFPSLimit = true)
    {
        vSyncCheckmark.isOn = on;

        QualitySettings.vSyncCount = on ? 1 : 0;

        fpsLimitDropdown.interactable = !on;


        if (on)
        {
            var listAvailableStrings = fpsLimitDropdown.options.Select(option => option.text).ToList();
            fpsLimitDropdown.value = listAvailableStrings.IndexOf("VSync (Monitor Refresh Rate)");
        }
        else if (UpdateFPSLimit)
        {
            var listAvailableStrings = fpsLimitDropdown.options.Select(option => option.text).ToList();
            fpsLimitDropdown.value = listAvailableStrings.IndexOf("Unlimited");
        }
    }

    public void UpdateFullscreenMode(int index)
    {
        Screen.fullScreenMode = fullScreenModes[index];
        PlayerPrefs.SetString("FullscreenMode", Screen.fullScreenMode.ToString());

    }
}
