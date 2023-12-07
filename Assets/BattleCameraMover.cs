using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleSystem;

public class BattleCameraMover : MonoBehaviour
{

    public GameObject[] cameraPositions;

    private void OnEnable()
    {
        OnCameraAction += HandleCameraAction;
    }

    private void OnDisable()
    {
        OnCameraAction -= HandleCameraAction;
    }


    private void HandleCameraAction(CameraActionType actionType, int cameraPos)
    {

            switch (actionType)
            {
                case CameraActionType.GoToStart:
                    StartCoroutine(MoveCamera(cameraPositions[cameraPos].transform.position, cameraPositions[cameraPos].transform.rotation, .5f, 60));
                break;
                case CameraActionType.GoToPos:
                    StartCoroutine(MoveCamera(cameraPositions[cameraPos].transform.position, cameraPositions[cameraPos].transform.rotation, .5f, 105));
                        break;
                default:
                    break;
            }
    }


    private IEnumerator MoveCamera(Vector3 targetPos, Quaternion targetRot, float time, int camFOV)
    {
        float elapsedTime = 0;
        Vector3 startingPos = this.transform.position;
        Quaternion startingRot = this.transform.rotation;
        Camera camera = this.GetComponent<Camera>();    

        while (elapsedTime < time)
        {
            this.transform.position = Vector3.Lerp(startingPos, targetPos, elapsedTime/time);
            this.transform.rotation = Quaternion.Lerp(startingRot, targetRot, elapsedTime / time);
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, camFOV, elapsedTime/time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        this.transform.position = targetPos;
        this.transform.rotation = targetRot;

    }
}
