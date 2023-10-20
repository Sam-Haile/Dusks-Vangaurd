using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{

    public GameObject pauseMenu;

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
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            if (!pauseMenu.activeSelf)
            {
                isPaused = false;
                if (!menuManager.menuScreenActive)
                    Time.timeScale = 1;
            }
            else
            {
                isPaused = true;
                Time.timeScale = 0;
            }
        }
    }


    public void OnResume()
    {
        pauseMenu?.SetActive(false);
        Time.timeScale = 1;
    }

    // Use this to save game and quit out of application
    public void OnApplicationQuit()
    {

    }

    public void OnMenu()
    {
        pauseMenu?.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public void OnLoad()
    {
        Debug.Log("Loading Game... Please vait beta");
    }
}
