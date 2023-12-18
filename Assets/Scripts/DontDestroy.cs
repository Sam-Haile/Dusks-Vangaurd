using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy instance { get; private set; }

    public CinemachineFreeLook cam;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    public void DestroyThis()
    {
        Debug.Log("Destroying");
        Destroy(gameObject);    
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "004 (KokaCastle)")
        {
            // Reset camera state when the target scene is loaded
            ResetCameraState();
        }
    }

    private void ResetCameraState()
    {
        Player.instance.gameObject.transform.position = new Vector3(-60, -16.8f, -23);
    }


}
