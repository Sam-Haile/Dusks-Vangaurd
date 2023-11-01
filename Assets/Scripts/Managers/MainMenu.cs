using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public GameObject loadingScreen;
    public Slider loadingBarFill;

    public void OnStart()
    {
        StartCoroutine(LoadSceneAsync(1));
        //SceneManager.LoadScene(1);
    }

    public void OnQuit()
    {
        Application.Quit();
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

        // After the scene has loaded, introduce an artificial delay
        float artificialDelay = 1.0f; // 1 second delay, for example
        float startTime = Time.time;

        while (Time.time - startTime < artificialDelay)
        {
            loadingBarFill.value = Mathf.Lerp(loadingBarFill.value, 1, (Time.time - startTime) / artificialDelay);
            yield return null;
        }

        loadingBarFill.value = 1; // Ensure the slider reaches the end
    }
}
