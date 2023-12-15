using Cinemachine;
using System.Collections;
using System.Collections.Generic;
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
        Debug.Log("Destoying");
        Destroy(gameObject);    
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "004 (KokaCastle)")
        {
            Debug.Log("Scene loadedf");
            // Reset camera state when the target scene is loaded
            ResetCameraState();
        }
    }

    private void ResetCameraState()
    {
        Debug.Log("MovingPlayer");
        Player.instance.gameObject.transform.position = new Vector3(-60, -16.8f, -23);

        // Implement camera state reset logic here
        //cam.Follow = Player.instance.gameObject.transform;
        //cam.LookAt = Player.instance.gameObject.transform;
    }

}
