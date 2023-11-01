using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public GameObject loadingScreen;
    public Slider loadingBarFill;

    private bool isLoading = false;

    public void OnStart()
    {
        if (isLoading) return;

        isLoading = true;
        Debug.Log("OnStart called");
        StartCoroutine(LoadSceneAsync(1));
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        Debug.Log("LoadingScene2");

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
