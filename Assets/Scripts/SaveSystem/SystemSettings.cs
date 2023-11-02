using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SystemSettings
{
    //Settings fields
    public int volume;
    public int sfx;
    public bool fullscreen;
    public int graphics;

    public SystemSettings(Slider volumeSlider, Slider sfxSlider, Toggle isFullscreen, TMP_Dropdown graphicsLevel)
    {
        volume = (int)volumeSlider.value;
        sfx = (int)sfxSlider.value;
        fullscreen = isFullscreen.enabled;
        graphics = graphicsLevel.value;
    }
}
