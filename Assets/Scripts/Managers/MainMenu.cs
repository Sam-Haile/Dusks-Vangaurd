using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public GameObject loadingScreen;
    public GameObject saveLoadUI;
    public Slider loadingBarFill;

    private bool isLoading = false;

    private void Start()
    {
        Debug.Log(PlayerPrefs.GetInt("FirstTime"));
    }

    public void UICheck()
    {

        if (PlayerPrefs.GetInt("FirstTime", 1) == 1)
        {
            // This is the first time the game is being opened
            PlayerPrefs.SetInt("FirstTime", 1); // Set the flag to indicate the game has been opened before
            PlayerPrefs.Save();
            DisableSaveLoadUI();
            Debug.Log("Disabled first");
            StartCoroutine(LoadSceneAsync(1));
        }
        else
        {
            EnableSaveLoadUI();
            Debug.Log("Enabling now first");
        }
    }

    void DisableSaveLoadUI()
    {
        saveLoadUI.SetActive(false);
    }

    void EnableSaveLoadUI()
    {
        saveLoadUI.SetActive(true);
    }

    // Start Button
    public void OnStart()
    {
        if (isLoading) return;

        isLoading = true;
        StartCoroutine(LoadSceneAsync(1));
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    // Loads Credits
    public void LoadLink(string hyperlink)
    {
        Application.OpenURL(hyperlink);
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            
            loadingBarFill.value = progressValue;

            yield return null;
        }
    }
}
