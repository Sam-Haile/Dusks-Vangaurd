using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleSystem;

public class BattleCameraMover : MonoBehaviour
{
    // Camera behind the player
    public GameObject[] cameraPositions;

    // Camera for player attacks
    public GameObject[] cameraEnemyPositions;
    private Animator animator;

    bool doneMoving = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

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
                    StartCoroutine(MoveCamera(cameraPositions[cameraPos].transform.position, cameraPositions[cameraPos].transform.rotation, .01f, 60));
                break;
            case CameraActionType.GoBehindPlayer:
                StartCoroutine(GoBehindAndFollowAttack(cameraPos));
                break;
                case CameraActionType.EnemyAttacking:
                StartCoroutine(EnemyAttackingPlayer(cameraPos));
                break;
                default:
                    break;
            }
    }


    private IEnumerator GoBehindAndFollowAttack(int cameraPos)
    {
        yield return StartCoroutine(MoveCamera(cameraPositions[cameraPos].transform.position, cameraPositions[cameraPos].transform.rotation, .5f, 100));

        yield return new WaitForSeconds(.45f);

        yield return StartCoroutine(MoveCamera(
            new Vector3(cameraPositions[3].transform.position.x,
            cameraPositions[3].transform.position.y,
            cameraPositions[3].transform.position.z + 3.75f), cameraPositions[cameraPos].transform.rotation, .5f, 100));

        yield return new WaitForSeconds(.70f);

        this.GetComponent<Camera>().fieldOfView = 80;
        // After GoBehindPlayer finishes, start GoToPos
        yield return StartCoroutine(MoveCamera(
            new Vector3(cameraPositions[3].transform.position.x - 5.17f,
            cameraPositions[3].transform.position.y + 1.94f,
            cameraPositions[3].transform.position.z + 4f), Quaternion.Euler(new Vector3(22.9f, 57.7f, 0f)), .01f, 80));
    }

    private IEnumerator EnemyAttackingPlayer(int cameraPos)
    {

        yield return StartCoroutine(MoveCamera(cameraEnemyPositions[cameraPos].transform.position, cameraEnemyPositions[cameraPos].transform.rotation, .01f, 80));

    }

    private IEnumerator MoveCamera(Vector3 targetPos, Quaternion targetRot, float time, int camFOV)
    {
        doneMoving = false;

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

        doneMoving = true;
    }
}
