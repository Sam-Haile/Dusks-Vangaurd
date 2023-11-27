using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    public static PlayerCollision instance;

    public static string enemyTag;
    public Vector3 beforeBattlePos;

    private bool isBattleInitiated = false;

    public Animator battleTransitionAnimator;

    public bool canBattle = true;

    public MenuManager menuManager;

    public delegate void BattleTriggered();
    public static BattleTriggered OnBattleTriggered;

    [HideInInspector] public string currentSceneName;
    private void Awake()
    {
        // Ensure that there's only one instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); 
    }

    /// <summary>
    /// Save the players position in the main world
    /// Destroy the enemy and its spawner
    /// Begin loading the battle scene
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        EnemyAI enemyReference = other.GetComponent<EnemyAI>();

        if (!isBattleInitiated && enemyReference && canBattle)
        {
            currentSceneName = SceneManager.GetActiveScene().name;
            PlayerMovement.instance.enabled = false;
            beforeBattlePos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            isBattleInitiated = true;
            OnBattleTriggered?.Invoke();
            enemyTag = other.gameObject.tag;
            canBattle = false;

            enemyReference.isDefeated = true;
            enemyReference.GetComponent<Collider>().enabled = !enemyReference.isDefeated;
            StartCoroutine(LoadBattleScene());

        }
    }

    IEnumerator LoadBattleScene()
    {
        battleTransitionAnimator.SetTrigger("End");
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene("BattleScene");
    }

    public void OnLevelWasLoaded(int level)
    {
        if (level == 1 && isBattleInitiated)
        {
            battleTransitionAnimator.SetTrigger("Start");
            StartCoroutine(BattleCooldown());
            this.transform.position = beforeBattlePos;
            isBattleInitiated = false; // Reset the flag after handling the scene load
        }
    }

    IEnumerator BattleCooldown()
    {
        yield return new WaitForSeconds(1.5f);
        canBattle = true;
    }

}
