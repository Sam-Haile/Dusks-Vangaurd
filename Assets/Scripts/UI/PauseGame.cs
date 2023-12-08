using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{

    public GameObject pauseMenu;
    public GameObject pausePanel;
    public GameObject panelBackground;
    public GameObject statsMenu;
    public GameObject saveLoadMenu;
    public GameObject pointer;
    public GameObject inventoryMenu;

    public GameObject settingsMenu;
    public GameObject[] pauseOptions;

    public Slider volumeSlider;
    public Slider sfxSlider;
    public Toggle isFullscreen;
    public TMP_Dropdown graphicsLevel;

    public Animator popupAnimator;

    [HideInInspector]
    public bool isPaused;

    private MenuManager menuManager;



    private void Start()
    {
        menuManager = GetComponent<MenuManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetMenu();
            pauseMenu.SetActive(!pauseMenu.activeSelf);

            if (!pauseMenu.activeSelf) // if pause menu was off
            {
                saveLoadMenu.SetActive(false);
                pointer.SetActive(false);
                pausePanel.SetActive(true);
                isPaused = false;
                if (!menuManager.menuScreenActive)
                    Time.timeScale = 1;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                isPaused = true;
                //reset the pause menu if left on the settings
                pointer.SetActive(false);
                settingsMenu.SetActive(false);
                saveLoadMenu.SetActive(false);
                foreach (GameObject button in pauseOptions)
                {
                    button.SetActive(true);
                }
                
                Time.timeScale = 0;
            }
        }

        if (pauseMenu.activeSelf)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

    }

    public void OnResume()
    {
        pauseMenu?.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnMenu()
    {
        pauseMenu?.SetActive(false);
        SceneManager.LoadScene(0);
    }


    public void ResetMenu()
    {
        panelBackground.SetActive(true);
        statsMenu.SetActive(true);
    }

}
