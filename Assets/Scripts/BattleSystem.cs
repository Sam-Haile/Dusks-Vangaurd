using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, SECONDPLAYERTURN, ENEMYTURN, WON, GAINEXP, LOST, FLEE }

public class BattleSystem : MonoBehaviour
{

    #region Fields
    // Highlight/Selection fields
    private RaycastHit raycastHit;
    private Transform highlight;
    private Transform selection;
    private Unit selectedUnit;
    private bool canSelect;

    // Enemy Instantiation fields
    public Dictionary<string, GameObject> enemyDictionary = new Dictionary<string, GameObject>();
    string enemyTag = PlayerCollision.enemyTag;
    public Transform[] enemyBattleStations;
    public GameObject[] enemyPrefabList;
    public List<Unit> activeEnemies;
    public List<BattleHUD> enemyHUD;
    Unit enemyUnit;

    // Player1 Instantiation fields
    public Transform playerBattleStation;
    public BattleHUD playerHUD;
    PlayableCharacter playerUnit;
    private int initialDefenseP1;
    private bool isPlayerDead;
    public Transform playerPos;

    // Player2 Instantiation fields
    public Transform player2BattleStation;
    public BattleHUD player2HUD;
    PlayableCharacter player2Unit;
    private int initialDefenseP2;
    private bool isPlayer2Dead;

    public Text dialogueText;
    public List<Button> buttons;
    public BattleState state;
    public List<Button> backButtons;
    //EXP Screen
    private int totalExp;
    public Slider p1XpSlider;
    public Slider p2XpSlider;
    public GameObject levelScreen;
    public UnityEvent levelUpEvent;
    private int totalMoney = 0;


    // Event for handling battle animations
    public delegate void BattleActionHandler();
    public static event BattleActionHandler OnBattleAction;

    #endregion

    #region Battle Setup

    private void Awake()
    {
        playerPos = FindObjectOfType<PlayerMovement>().transform;
        playerUnit = playerPos.GetComponent<PlayableCharacter>();

        foreach (GameObject enemy in enemyPrefabList)
        {
            enemyDictionary.Add(enemy.tag, enemy);
        }
    }

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        totalExp = 0;
    }

    /// <summary>
    /// Determines the number of enemeis to spawn in a given battle
    /// </summary>
    /// <returns>Number of enemies to spawn</returns>
    private int NumOfEnemies()
    {
        int numOfEnemies = Random.Range(0, 101);
        int chanceOfDiffEnemy = Random.Range(0, 2);

        if (numOfEnemies <= 50) // 50% Chance
        {
            return 2;
        }
        else if (numOfEnemies > 50 && numOfEnemies <= 85) //35% Chance
        {
            return 3;
        }
        else
            return 1;//15% Chance
    }

    IEnumerator SetupBattle()
    {
        SetButtonsActive(false);

        playerPos.position = playerBattleStation.position;

        //Stop animation and movement
        playerUnit.GetComponent<PlayerMovement>().isMoving = false;
        playerUnit.GetComponent<PlayerMovement>().enabled = false;
        playerUnit.transform.rotation = Quaternion.Euler(0, 0, 0);
        initialDefenseP1 = playerUnit.baseDefense;

        player2Unit = Puck.instance;
        player2Unit.GetComponent<Follow>().enabled = false;
        player2Unit.gameObject.transform.position = player2BattleStation.transform.position;
        player2Unit.transform.rotation = Quaternion.Euler(0, 0, 0);
        initialDefenseP2 = player2Unit.baseDefense;

        List<string> keys = new List<string>(enemyDictionary.Keys);

        int num = NumOfEnemies();

        if (num == 1)
        {
            GameObject enemyGameObj = Instantiate(enemyDictionary[enemyTag], enemyBattleStations[1]);
            enemyUnit = enemyGameObj.GetComponent<Unit>();
            enemyUnit.GetComponent<EnemyAI>().enabled = false;
            activeEnemies.Add(enemyUnit);

            enemyBattleStations[2].gameObject.SetActive(false);
            enemyBattleStations[0].gameObject.SetActive(false);
        }
        else if (num == 2)
        {
            GameObject enemyGameObj1 = Instantiate(enemyDictionary[keys[Random.Range(0, keys.Count)]], enemyBattleStations[0]);
            enemyUnit = enemyGameObj1.GetComponent<Unit>();
            enemyUnit.GetComponent<EnemyAI>().enabled = false;
            activeEnemies.Add(enemyUnit);

            GameObject enemyGameObj2 = Instantiate(enemyDictionary[enemyTag], enemyBattleStations[1]);
            enemyUnit = enemyGameObj2.GetComponent<Unit>();
            enemyUnit.GetComponent<EnemyAI>().enabled = false;
            activeEnemies.Add(enemyUnit);

            enemyBattleStations[2].gameObject.SetActive(false);
        }
        else if (num == 3)
        {
            GameObject enemyGameObj1 = Instantiate(enemyDictionary[keys[Random.Range(0, keys.Count)]], enemyBattleStations[0]);
            enemyUnit = enemyGameObj1.GetComponent<Unit>();
            enemyUnit.GetComponent<EnemyAI>().enabled = false;
            activeEnemies.Add(enemyUnit);

            GameObject enemyGameObj2 = Instantiate(enemyDictionary[enemyTag], enemyBattleStations[1]);
            enemyUnit = enemyGameObj2.GetComponent<Unit>();
            enemyUnit.GetComponent<EnemyAI>().enabled = false;
            activeEnemies.Add(enemyUnit);

            GameObject enemyGameObj3 = Instantiate(enemyDictionary[keys[Random.Range(0, keys.Count)]], enemyBattleStations[2]);
            enemyUnit = enemyGameObj3.GetComponent<Unit>();
            enemyUnit.GetComponent<EnemyAI>().enabled = false;
            activeEnemies.Add(enemyUnit);

        }

        dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

        playerHUD.SetHUD(playerUnit);
        player2HUD.SetHUD(player2Unit);

        //for (int i = 0; i < activeEnemies.Count; i++)
        //{
        //    enemyHUD[i].gameObject.SetActive(true);
        //    enemyHUD[i].SetHUD(enemyUnit);
        //}

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    #endregion

    #region Player Options
    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
        SetButtonsActive(true);
    }

    /// <summary>
    /// When the attack button is pressed
    /// waits for an enemy to be selected
    /// </summary>
    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN && state != BattleState.SECONDPLAYERTURN)
            return;

        dialogueText.text = "Who?";

        if (state == BattleState.PLAYERTURN)
            StartCoroutine(WaitForAttackSelection(playerUnit.baseAttack));
        else
            StartCoroutine(WaitForAttackSelection(player2Unit.baseAttack));

    }

    /// <summary>
    /// When the run button is pressed
    /// has chance of failure
    /// </summary>
    public void OnFleeButton()
    {
        if (state != BattleState.PLAYERTURN && state != BattleState.SECONDPLAYERTURN)
            return;

        StartCoroutine(PlayerFlee());
    }

    /// <summary>
    /// When the arcane button is pressed
    /// depends on arcane stat
    /// </summary>
    public void OnArcaneButton()
    {
        if (state != BattleState.PLAYERTURN && state != BattleState.SECONDPLAYERTURN)
            return;

        if (state == BattleState.PLAYERTURN)
            StartCoroutine(WaitForArcaneSelection(playerUnit.baseArcane));
        else if (state == BattleState.SECONDPLAYERTURN)
            StartCoroutine(WaitForArcaneSelection(player2Unit.baseArcane));
    }

    /// <summary>
    /// When the gaurd button is pressed
    /// players recieve a x2 buff to defense
    /// </summary>
    public void OnGaurdButton()
    {
        if (state != BattleState.PLAYERTURN && state != BattleState.SECONDPLAYERTURN)
            return;

        StartCoroutine(PlayerGaurd());
    }

    /// <summary>
    /// Reset any buffed stats at the end of a battle
    /// </summary>
    private void ResetStats()
    {
        playerUnit.baseDefense = initialDefenseP1;
        player2Unit.baseDefense = initialDefenseP2;
    }

    /// <summary>
    /// Trigger the flee state if successful
    /// players do not recieve experience from fled battles
    /// </summary>
    /// <returns>If successful, ends battle. If not, enemies turn</returns>
    IEnumerator PlayerFlee()
    {
        int escape = Random.Range(0, 100);

        if (escape < 80)
        {
            state = BattleState.FLEE;
            EndBattle();
        }
        else
        {
            dialogueText.text = "Could not flee!.";
            yield return new WaitForSeconds(2f);
            state = BattleState.ENEMYTURN;

            StartCoroutine(EnemyTurn());
        }
    }

    /// <summary>
    /// Players may attack any selected enemy
    /// depends on attack stat and that of weapon
    /// </summary>
    /// <param name="selectedEnemy"></param>
    /// <param name="damageStat"></param>
    /// <returns>Deals damage to selected enemy</returns>
    IEnumerator PlayerAttack(Unit selectedEnemy, int damageStat)
    {
        
        // Call the event to cue an animation
        OnBattleAction?.Invoke();

        bool isDead = false;
        int damage;
        SetButtonsActive(false);
        backButtons[0].gameObject.SetActive(false);
        backButtons[1].gameObject.SetActive(false);
        
        if (state == BattleState.PLAYERTURN)
        {
            damage = DetermineDamage(damageStat, selectedEnemy, playerUnit.equippedWeapon);
            isDead = selectedUnit.TakeDamage(damage);
        }
        else
        {
            damage = DetermineDamage(damageStat, selectedEnemy, player2Unit.equippedWeapon);
            isDead = selectedUnit.TakeDamage(damage);
        }

        enemyHUD[activeEnemies.IndexOf(selectedEnemy)].SetHP(selectedEnemy.currentHP);
        dialogueText.text = selectedEnemy.unitName + " takes " + damage + " damage.";

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            dialogueText.text = selectedEnemy.unitName + " has been defeated!";
            // Add random volatility to XP and Money
            totalExp += ExpToGain(selectedEnemy);
            totalMoney += MoneyToGain(selectedEnemy);
            yield return new WaitForSeconds(2f);
            activeEnemies.Remove(selectedEnemy);
            Destroy(selectedEnemy.gameObject);

            // If all the enemies are defeated, end encounter
            if (activeEnemies.Count == 0)
            {
                state = BattleState.WON;
                EndBattle();
            }
            // If player 1 went, players 2s turn
            else if (state == BattleState.PLAYERTURN)
            {
                PlayerTurn();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
        else if (state == BattleState.PLAYERTURN && !isPlayer2Dead)
        {
            state = BattleState.SECONDPLAYERTURN;
            PlayerTurn();
        }
        else
        {
            state = BattleState.ENEMYTURN;


            StartCoroutine(EnemyTurn());
        }
        selectedUnit = null;
    }

    IEnumerator PlayerArcane(Unit selectedEnemy, int damageStat)
    {
        bool isDead = false;
        int damage;
        SetButtonsActive(false);
        backButtons[0].gameObject.SetActive(false);
        backButtons[1].gameObject.SetActive(false);

        if (state == BattleState.PLAYERTURN)
        {
            damage = DetermineDamageArcane(damageStat, selectedEnemy, playerUnit.equippedWeapon);
            isDead = selectedUnit.TakeDamage(damage);
        }
        else
        {
            damage = DetermineDamageArcane(damageStat, selectedEnemy, playerUnit.equippedWeapon);
            isDead = selectedUnit.TakeDamage(damage);
        }

        enemyHUD[activeEnemies.IndexOf(selectedEnemy)].SetHP(selectedEnemy.currentHP);
        dialogueText.text = selectedEnemy.unitName + " takes " + damage + " arcane damage.";

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            dialogueText.text = selectedEnemy.unitName + " has been defeated!";
            // Add random volatility to XP and Money
            totalExp += ExpToGain(selectedEnemy);
            totalMoney += MoneyToGain(selectedEnemy);
            yield return new WaitForSeconds(2f);
            activeEnemies.Remove(selectedEnemy);
            Destroy(selectedEnemy.gameObject);

            // If all the enemies are defeated, end encounter
            if (activeEnemies.Count == 0)
            {
                state = BattleState.WON;
                EndBattle();
            }
            // If player 1 went, players 2s turn
            else if (state == BattleState.PLAYERTURN)
            {
                PlayerTurn();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
        else if (state == BattleState.PLAYERTURN && !isPlayer2Dead)
        {
            state = BattleState.SECONDPLAYERTURN;
            PlayerTurn();
        }
        else
        {
            state = BattleState.ENEMYTURN;


            StartCoroutine(EnemyTurn());
        }
        selectedUnit = null;
    }

    /// <summary>
    /// Reduces incoming damage by half
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayerGaurd()
    {
        if (state == BattleState.PLAYERTURN)
        {
            playerUnit.baseDefense *= 2;
            dialogueText.text = "You brace yourself";

            SetButtonsActive(false);

            yield return new WaitForSeconds(2f);

            if (!isPlayer2Dead)
            {
                state = BattleState.SECONDPLAYERTURN;
                PlayerTurn();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());

            }
        }
        else if (state == BattleState.SECONDPLAYERTURN)
        {
            player2Unit.baseDefense *= 2;
            SetButtonsActive(false);
            yield return new WaitForSeconds(2f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    /// <summary>
    /// Waits for a selection before attacking
    /// </summary>
    /// <param name="damageStat"></param>
    /// <returns>The selected enemy</returns>
    IEnumerator WaitForAttackSelection(int damageStat)
    {
        while (selectedUnit == null)
        {
            yield return null;
        }

        StartCoroutine(PlayerAttack(selectedUnit, damageStat));
    }

    IEnumerator WaitForArcaneSelection(int damageStat)
    {
        while (selectedUnit == null)
        {
            yield return null;
        }

        StartCoroutine(PlayerArcane(selectedUnit, damageStat));
    }

    /*
    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(5);     // CALCULATE HEALING HERE

        playerHUD.SetHP(playerUnit.currentHP);
        dialogueText.text = "You feel renewed strength!";

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }
    */ // HealCode

    #endregion

    #region Enemy Options

    /// <summary>
    /// Enemies turn will attack the player
    /// prioritizes player if health < 15% of max
    /// </summary>
    /// <returns></returns>
    IEnumerator EnemyTurn()
    {
        SetButtonsActive(false);

        if (playerUnit.currentHP == 0)
        {
            isPlayerDead = true;
        }
        else if (player2Unit.currentHP == 0)
        {
            isPlayer2Dead = true;
        }

        foreach (Unit enemy in activeEnemies)
        {
            // Enemies randomly decide wether to use arcane or physical attacks
            int attackType = Random.Range(0, 1);
            dialogueText.text = enemyUnit.unitName + " attacks!";
            yield return new WaitForSeconds(1f);

            int dmg;
            Unit target;   //The target the enemy will attack


            if (isPlayerDead)                                           //If P1 is dead attack P2
                target = player2Unit;
            else if (isPlayer2Dead)                                     //If P2 is dead attack P1
                target = playerUnit;
            else if (playerUnit.currentHP < .15 * playerUnit.maxHP)     //If P1s health is <15% attack P1
                target = playerUnit;
            else if (player2Unit.currentHP < .15 * player2Unit.maxHP)   //If P2s health is <15% attack P2
                target = player2Unit;
            else                                                        //If none of the above cases apply, choose randomly
                target = Random.Range(0, 2) == 0 ? playerUnit : player2Unit;

            // Use a formula to determine the amount of damage to deal
            Debug.Log(attackType);
            if (attackType == 0)
            {
                dmg = DetermineDamage(enemy.baseAttack, target, null);

            }
            else
            {
                dmg = DetermineDamageArcane(enemy.baseArcane, target, null);
            }

            // Apply damage to P1 and display to textBox
            if (target == playerUnit)
            {
                isPlayerDead = playerUnit.TakeDamage(dmg);
                playerHUD.SetHP(target.currentHP);
                dialogueText.text = playerUnit.unitName + " takes " + dmg + " damage!";
            }
            // Apply damage to P2 and display to textBox
            else if (target == player2Unit)
            {
                isPlayer2Dead = player2Unit.TakeDamage(dmg);
                player2HUD.SetHP(target.currentHP);
                dialogueText.text = player2Unit.unitName + " takes " + dmg + " damage!";
            }


            yield return new WaitForSeconds(2f);

            // If both players die, end battle
            if (isPlayerDead && isPlayer2Dead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
        }

        // Reset defense after gaurding is complete
        ResetStats();
        if (!isPlayerDead)
            state = BattleState.PLAYERTURN;
        else
            state = BattleState.SECONDPLAYERTURN;
        PlayerTurn();
    }
    #endregion

    #region Misc. Coroutines

    /// <summary>
    /// Loads a selected scene
    /// </summary>
    /// <param name="sceneIndex"></param>
    /// <returns></returns>
    IEnumerator LoadWorld(int sceneIndex)
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Apply the XP to players after a battle ends in victory
    /// </summary>
    /// <returns></returns>
    IEnumerator GainExp(PlayableCharacter player, Slider xpSlider)
    {
        float startValue = (float)player.experience / (float)player.expToNextLevel;
        xpSlider.value = startValue;
        levelScreen.SetActive(true);
        yield return new WaitForSeconds(2f);

        float targetValue = (float)(player.experience + totalExp) / (float)player.expToNextLevel;

        float overflowExp = 0;
        float elapsedTime = 0f;
        float duration = 2f; // Time (in seconds) you want the interpolation to take

        while (elapsedTime < duration)
        {
            float sliderValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);

            if (xpSlider.value == 1)
            {
                //TRIGGER LEVEL UP LOGIC HERE
                levelUpEvent?.Invoke();

                overflowExp = (targetValue * player.expToNextLevel) - player.expToNextLevel;

                // Reset the start and target values to correctly interpolate the overflow experience:
                startValue = 0;
                targetValue = overflowExp / player.expToNextLevel;

                // Reset the elapsed time and duration to interpolate the overflow experience:
                elapsedTime = 0;
                duration = 2f; // Reset the interpolation duration

                sliderValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            }

            SetXP(sliderValue, xpSlider);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the slider value reaches the exact target value in case of rounding errors
        SetXP(targetValue, xpSlider);
        player.AddExperience(totalExp, dialogueText.text);

        yield return new WaitForSeconds(2f);

        EndBattle();
    }

    #endregion

    #region Misc. Methods
    private void SetXP(float currentXp, Slider slider)
    {
        slider.value = currentXp;
    }

    /// <summary>
    /// Handles selecting an enemy during the player phase
    /// </summary>
    void Update()
    {

        if (canSelect)
        {
            // Highlight
            if (highlight != null)
            {
                highlight.gameObject.GetComponent<Outline>().enabled = false;
                highlight = null;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
            {
                highlight = raycastHit.transform;
                if (highlight.gameObject.GetComponent<EnemyAI>() != null && highlight != selection)
                {
                    Outline outline;

                    if (highlight.gameObject.GetComponent<Outline>() != null)
                    {
                        outline = highlight.gameObject.GetComponent<Outline>();
                        outline.OutlineWidth = 30f;
                        outline.enabled = true;
                    }
                    else
                    {
                        outline = highlight.gameObject.AddComponent<Outline>();
                        outline.OutlineWidth = 30f;
                        outline.enabled = true;
                    }
                }
                else
                {
                    highlight = null;
                }
            }

            // Selection
            if (Input.GetMouseButtonDown(0))
            {
                if (highlight)
                {
                    if (selection != null)
                    {
                        selection.gameObject.GetComponent<Outline>().enabled = false;
                    }

                    selection = raycastHit.transform;
                    selectedUnit = selection.gameObject.GetComponent<Unit>();
                }
                else
                {
                    if (selection)
                    {
                        selection.gameObject.GetComponent<Outline>().enabled = false;
                        selection = null;
                    }
                }
            }

        }

    }

    private int DetermineDamage(int givingDmgStat, Unit recievingDmg, Weapon playersWeapon)
    {
        Weapon weapon = null;
        if (playersWeapon != null)
            weapon = playersWeapon.GetComponent<Weapon>();
        // Determine the defensive stat of the unit
        int armorDefense = (recievingDmg.equippedArmor != null) ? recievingDmg.equippedArmor.defense : 0;
        //Debug.Log(recievingDmg + "Defense: " + armorDefense);
        int damage = Mathf.Max(1, givingDmgStat - recievingDmg.baseDefense - armorDefense);
        if (weapon != null)
            damage += weapon.attack;
        return damage;
    }

    private int DetermineDamageArcane(int givingDmgStat, Unit recievingDmg, Weapon playersWeapon)
    {
        Weapon weapon = null;
        if (playersWeapon != null)
            weapon = playersWeapon.GetComponent<Weapon>();
        // Determine the special defensive stat of the unit
        int armorSpecDefense = (recievingDmg.equippedArmor != null) ? recievingDmg.equippedArmor.specialDefense : 0;
        //Debug.Log(recievingDmg + "Special Defense: " + armorSpecDefense);
        int damage = Mathf.Max(1, givingDmgStat - recievingDmg.baseDefense - armorSpecDefense);
        if (weapon != null)
            damage += weapon.arcane;
        return damage;
    }

    /// <summary>
    /// Returns the enemies XP will a little random increase/decrease
    /// </summary>
    /// <param name="enemiesXP"></param>
    /// <returns></returns>
    private int ExpToGain(Unit enemiesXP)
    {
        return enemiesXP.experience + Random.Range(-10, 11);
    }

    /// <summary>
    /// Returns the enemies XP will a little random increase/decrease
    /// </summary>
    /// <param name="enemiesMoney"></param>
    /// <returns></returns>
    private int MoneyToGain(Unit enemiesMoney)
    {
        return enemiesMoney.money + Random.Range(-50, 51);
    }

    public void SetButtonsActive(bool active)
    {
        foreach (Button button in buttons)
            button.gameObject.SetActive(active);
    }

    public void SetHighlightable(bool active)
    {
        canSelect = active;
    }

    void EndBattle()
    {
        if (state == BattleState.GAINEXP || state == BattleState.FLEE)
        {
            if (isPlayerDead)
                playerUnit.currentHP = 1;
            if (isPlayer2Dead)
                player2Unit.currentHP = 1;
        }

        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";

            state = BattleState.GAINEXP;
            StartCoroutine(GainExp(playerUnit, p1XpSlider));
            StartCoroutine(GainExp(player2Unit, p2XpSlider));
            playerUnit.money += totalMoney;
        }
        else if (state == BattleState.FLEE)
        {
            dialogueText.text = "You fled the battle!";
            StartCoroutine(LoadWorld(1));
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated.";
            StartCoroutine(LoadWorld(0));
        }
        else if (state == BattleState.GAINEXP)
        {
            StartCoroutine(LoadWorld(1));
        }
    }
    #endregion

}
