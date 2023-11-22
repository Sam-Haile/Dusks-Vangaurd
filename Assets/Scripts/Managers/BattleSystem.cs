using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, SECONDPLAYERTURN, ENEMYTURN, WON, LOST, FLEE }

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
    [HideInInspector] public List<Unit> activeEnemies;
    //public List<BattleHUD> enemyHUD;
    private float shrinkDuration = 1f;
    Unit enemyUnit;

    // Player1 Instantiation fields
    public Transform playerBattleStation;
    private PlayableCharacter playerUnit;
    private CharacterController playerController;
    private PlayerCollision playerCollision;
    private PlayerMovement playerMovement;
    private int initialDefenseP1;
    private bool isPlayerDead;
    private Transform playerPos;

    // Player2 Instantiation fields
    public Transform player2BattleStation;
    PlayableCharacter player2Unit;
    Follow player2FollowScript;
    private int initialDefenseP2;
    private bool isPlayer2Dead;

    public Text dialogueText;
    public List<Button> buttons;
    private BattleState state;
    public List<Button> backButtons;
    public List<Button> spells;
    private int spellDamage;
    private Spell selectedSpell;

    private LevelUpManager levelUpManager;
    public BattleHUD battleHUD;

    public Material outlineMaterial;

    public int LastDamage { get; private set; }

    public enum BattleActionType
    {
        Start,
        Attack,
        Gaurd,
        StopGaurding,
        Arcane,
        Damaged,
        Healed,
        Run,
        Die,
        Won
    }


    // Event for handling battle animations
    public delegate void PlayerActionHandler(BattleActionType actionType, PlayableCharacter player);
    public static event PlayerActionHandler OnPlayerAction;

    public delegate void EnemyActionHandler(BattleActionType actionType, Unit enemy);
    public static event EnemyActionHandler OnEnemyAction;

    public delegate void BattleStateHandler(BattleState actionType);
    public static event BattleStateHandler OnBattleState;


    public delegate int DamageCalculationDelegate(int damageStat, Unit selectedEnemy, Weapon equippedWeapon);

    #endregion

    #region Battle Setup

    private void Awake()
    {
        playerPos = FindObjectOfType<Player>().transform;
        playerPos.position = playerBattleStation.position;

        playerUnit = playerPos.GetComponent<PlayableCharacter>();
        playerMovement = playerPos.GetComponent<PlayerMovement>();
        playerController = playerPos.GetComponent<CharacterController>();
        playerCollision = playerPos.GetComponent<PlayerCollision>();

        player2Unit = Puck.instance;
        player2FollowScript = player2Unit.GetComponent<Follow>();

        foreach (GameObject enemy in enemyPrefabList)
        {
            enemyDictionary.Add(enemy.tag, enemy);
        }

        levelUpManager = GetComponent<LevelUpManager>();
        
    }

    void Start()
    {
        state = BattleState.START;
        playerCollision.battleTransitionAnimator.SetTrigger("Start");
        StartCoroutine(SetupBattle());
        levelUpManager.totalExp = 0;
    }

    /// <summary>
    /// Determines the number of enemeis to spawn in a given battle
    /// 50% chance for 2 enemies, 35% for 3, 15% for 1
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

    /// <summary>
    /// Stops player 1 and 2 from moving
    /// Set the hud to their current stats
    /// </summary>
    private void SetupPlayers()
    {
        playerController.enabled = false;
        playerMovement.isMoving = false;
        playerMovement.enabled = false;
        playerPos.rotation = Quaternion.Euler(0, 0, 0);
        initialDefenseP1 = playerUnit.baseDefense;

        player2FollowScript.fairyDust.enabled = false;
        player2FollowScript.enabled = false;
        player2Unit.gameObject.transform.position = player2BattleStation.transform.position;
        player2Unit.transform.rotation = Quaternion.Euler(0, 0, 0);
        player2Unit.transform.localScale = new Vector3(5, 5, 5);
        initialDefenseP2 = player2Unit.baseDefense;

        battleHUD.SetHP(playerUnit);
        battleHUD.SetHP(player2Unit);
    }

    IEnumerator SetupBattle()
    {
        SetButtonsActive(false);

        SetupPlayers();

        List<string> keys = new List<string>(enemyDictionary.Keys);

        int num = NumOfEnemies();

        if (num == 1)
        {
            InstantiateEnemies(enemyDictionary[enemyTag], 2);
        }
        else if (num == 2)
        {
            InstantiateEnemies(enemyDictionary[enemyTag], 3);
            InstantiateEnemies(enemyDictionary[keys[Random.Range(0, keys.Count)]], 1);
        }
        else if (num == 3)
        {
            InstantiateEnemies(enemyDictionary[keys[Random.Range(0, keys.Count)]], 4);
            InstantiateEnemies(enemyDictionary[enemyTag], 2);
            InstantiateEnemies(enemyDictionary[keys[Random.Range(0, keys.Count)]], 0);
        }

        OnPlayerAction(BattleActionType.Start, playerUnit); //Start battle animations

        // foreach enemy, play the start animation
        foreach (Unit enemy in activeEnemies)
        {
            OnEnemyAction(BattleActionType.Start, enemy);
        }

        if (num == 1)
            dialogueText.text = enemyUnit.unitName + " approaches...";
        else
            dialogueText.text = num + " " + enemyUnit.unitName + "s approach...";

        battleHUD.UpdateAllStats(playerUnit);
        battleHUD.UpdateAllStats(player2Unit);

        yield return new WaitForSeconds(4f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }


    private void InstantiateEnemies(GameObject enemyToInstantite, int battleStationIndex)
    {
        enemyBattleStations[battleStationIndex].gameObject.SetActive(true);
        GameObject enemyGameObj = Instantiate(enemyToInstantite, enemyBattleStations[battleStationIndex]);
        enemyUnit = enemyGameObj.GetComponent<Unit>();
        enemyUnit.GetComponent<EnemyAI>().enabled = false;
        activeEnemies.Add(enemyUnit);
    }
    #endregion

    #region Player Options

    /// <summary>
    /// Display buttons and waits for an input from player
    /// </summary>
    void PlayerTurn()
    {
        dialogueText.text = "";
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

        switch (state)
        {
            case BattleState.PLAYERTURN:
                StartCoroutine(WaitForAttackSelection(playerUnit.baseAttack));
                break;
            case BattleState.SECONDPLAYERTURN:
                StartCoroutine(WaitForAttackSelection(player2Unit.baseAttack));
                break;
        }
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
    /// Display the currently equipped spells
    /// for each player on their turn
    /// </summary>
    public void DisplaySpells()
    {
        if (state == BattleState.PLAYERTURN)
        {
            for (int i = 0; i < playerUnit.spellbook.spells.Count; i++)
            {
                Spell spell = playerUnit.spellbook.spells[i];
                spells[i].GetComponent<Text>().text = spell.spellName;

                // Add a listener to the button click event
                spells[i].onClick.RemoveAllListeners(); // Remove existing listeners to avoid duplicates
                int index = i;
                spells[i].onClick.AddListener(() =>
                {
                    selectedSpell = playerUnit.spellbook.spells[index];
                    spellDamage = playerUnit.spellbook.CastSpell(selectedSpell.spellName, playerUnit);
                });
            }
        }
        else if (state == BattleState.SECONDPLAYERTURN)
        {
            for (int i = 0; i < player2Unit.spellbook.spells.Count; i++)
            {
                Spell spell = player2Unit.spellbook.spells[i];
                spells[i].GetComponent<Text>().text = spell.spellName;

                // Add a listener to the button click event
                spells[i].onClick.RemoveAllListeners(); // Remove existing listeners to avoid duplicates
                int index = i;
                spells[i].onClick.AddListener(() =>
                {
                    selectedSpell = player2Unit.spellbook.spells[index];
                    spellDamage = player2Unit.spellbook.CastSpell(selectedSpell.spellName, player2Unit);
                });
            }
        }
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
        {
            StartCoroutine(WaitForArcaneSelection(playerUnit.baseArcane));
        }
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
        // If there is a boss present in the battle
        // you cannot leave
        foreach (Unit enemy in activeEnemies)
        {
            if (enemy.tag == "Boss")
            {
                dialogueText.text = "You cannot flee this battle.";
                yield return new WaitForSeconds(2f);
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }

        int escape = Random.Range(0, 100);

        if (escape < 80)
        {
            state = BattleState.FLEE;
            EndBattle();
        }
        else
        {
            dialogueText.text = "Could not flee!";
            yield return new WaitForSeconds(2f);
            state = BattleState.ENEMYTURN;

            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator RotatePlayer(PlayableCharacter player, Unit enemy)
    {
        //Rotate towards to enemy your attacking
        Vector3 direction = (enemy.transform.position - player.transform.position).normalized;
        Vector3 flatDirection = new Vector3(direction.x, 0, direction.z).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(flatDirection);

        Quaternion startingRotation = player.transform.rotation;

        int rotationSpeed = 5;
        float t = 0f;

        while (t <= 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            Quaternion interpolatedRotation = Quaternion.Slerp(startingRotation, targetRotation, t);

            player.transform.rotation = Quaternion.Euler(0, interpolatedRotation.eulerAngles.y, 0);

            yield return null;
        }

        yield return new WaitForSeconds(5f);

        t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            Quaternion interpolatedRotation = Quaternion.Slerp(targetRotation, startingRotation, t);

            player.transform.rotation = Quaternion.Euler(0, interpolatedRotation.eulerAngles.y, 0);

            yield return null;
        }
    }

    IEnumerator PlayerAction(PlayableCharacter player, Unit selectedEnemy, int damageStat, DamageCalculationDelegate damageCalculation)
    {
        bool isDead;
        int damage;

        // Set the HUD
        SetButtonsActive(false);
        backButtons[0].gameObject.SetActive(false);
        backButtons[1].gameObject.SetActive(false);
        battleHUD.SetMP(player);

        // Determine the damage and if the enemy is dead
        damage = damageCalculation(damageStat, selectedEnemy, player.equippedWeapon);
        LastDamage = damage;
        isDead = selectedUnit.TakeDamage(damage);
        StartCoroutine(RotatePlayer(player, selectedEnemy));
        OnEnemyAction(BattleActionType.Damaged, selectedEnemy);


        dialogueText.text = selectedEnemy.unitName + " takes " + damage + " damage.";
        yield return new WaitForSeconds(2f);

        // If an enemy dies
        // If an enemy dies
        if (isDead)
        {
            OnEnemyAction(BattleActionType.Die, selectedEnemy);
            dialogueText.text = selectedEnemy.unitName + " has been defeated!";

            // Gain xp/gold 
            levelUpManager.GainXPAndGold(selectedEnemy);

            // Slowly shrink enemy
            Vector3 initialScale = selectedEnemy.transform.localScale;
            Vector3 finalScale = Vector3.zero;
            float elapsedTime = 0f;

            yield return new WaitForSeconds(2.5f);

            while (elapsedTime < shrinkDuration)
            {
                selectedEnemy.transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsedTime / shrinkDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            selectedEnemy.transform.localScale = finalScale;

            yield return new WaitForSeconds(1f);

            activeEnemies.Remove(selectedEnemy);
            Destroy(selectedEnemy.gameObject);

            // If all the enemies are defeated, end encounter
            if (activeEnemies.Count == 0)
            {
                state = BattleState.WON;
                EndBattle();
            }
        }
        // if the battle is not over,
        // and players turn is over,
        // and player 2 is not dead
        if (state == BattleState.PLAYERTURN && !isPlayer2Dead)
        {
            state = BattleState.SECONDPLAYERTURN;
            PlayerTurn();
        }
        // if the second player has moved
        else if (state == BattleState.SECONDPLAYERTURN)
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
        switch (state)
        {
            case BattleState.PLAYERTURN:
                OnPlayerAction(BattleActionType.Gaurd, playerUnit);
                playerUnit.baseDefense *= 2;
                SetButtonsActive(false);
                dialogueText.text = "Guts brace himself";
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
                break;
            case BattleState.SECONDPLAYERTURN:
                OnPlayerAction(BattleActionType.Gaurd, player2Unit);
                player2Unit.baseDefense *= 2;
                SetButtonsActive(false);
                dialogueText.text = "Puck braces himself";
                yield return new WaitForSeconds(2f);
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
                break;
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

        switch (state)
        {
            case BattleState.PLAYERTURN:
                StartCoroutine(PlayerAction(playerUnit, selectedUnit, damageStat, DetermineDamage));
                SetHighlightable(false);
                OnPlayerAction(BattleActionType.Attack, playerUnit);
                break;
            case BattleState.SECONDPLAYERTURN:
                StartCoroutine(PlayerAction(player2Unit, selectedUnit, damageStat, DetermineDamage));
                SetHighlightable(false);
                OnPlayerAction(BattleActionType.Attack, player2Unit);
                break;
            default:
                break;
        }
    }

    IEnumerator WaitForArcaneSelection(int damageStat)
    {
        while (selectedUnit == null)
            yield return null;

        if (selectedSpell.isHealingSpell)
            StartCoroutine(PlayerHeal(selectedUnit));

        if (!selectedSpell.isHealingSpell)
        {
            damageStat += spellDamage;
            selectedSpell.spellVFX.transform.position = selectedUnit.transform.position;
            selectedSpell.spellVFX.Play();

            switch (state)
            {
                case BattleState.PLAYERTURN:
                    StartCoroutine(PlayerAction(playerUnit, selectedUnit, damageStat, DetermineDamageArcane));
                    OnPlayerAction(BattleActionType.Arcane, playerUnit);
                    break;
                case BattleState.SECONDPLAYERTURN:
                    StartCoroutine(PlayerAction(player2Unit, selectedUnit, damageStat, DetermineDamageArcane));
                    OnPlayerAction(BattleActionType.Arcane, player2Unit);
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator PlayerHeal(Unit selectedPlayer)
    {
        int healingAmnt = Random.Range(selectedSpell.minDamage, selectedSpell.maxDamage);

        selectedPlayer.Heal(healingAmnt);

        battleHUD.SetHP(selectedPlayer);

        switch (state)
        {
            case BattleState.PLAYERTURN:
                LastDamage = healingAmnt;
                Debug.Log("Healiong for " + healingAmnt);
                OnPlayerAction(BattleActionType.Healed, playerUnit);
                battleHUD.SetMP(playerUnit);
                dialogueText.text = "You feel renewed strength!";
                yield return new WaitForSeconds(2f);
                state = BattleState.SECONDPLAYERTURN;
                PlayerTurn();
                break;
            case BattleState.SECONDPLAYERTURN:
                LastDamage = healingAmnt;
                Debug.Log("Healiong for " + healingAmnt);
                OnPlayerAction(BattleActionType.Healed, playerUnit);
                battleHUD.SetMP(player2Unit);
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
                break;
        }


    }
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
            isPlayerDead = true;
        else if (player2Unit.currentHP == 0)
            isPlayer2Dead = true;

        foreach (Unit enemy in activeEnemies)
        {
            int attackType = 0;
            yield return new WaitForSeconds(1.5f);

            // If the enemy can use magic, give them a chance of doing so
            if (enemy.canUseMagic)
                attackType = Random.Range(0, 2);
            //dialogueText.text = enemyUnit.unitName + " attacks!";

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
            if (attackType == 0)
                dmg = DetermineDamage(enemy.baseAttack, target, null);
            else
                dmg = DetermineDamageArcane(enemy.baseArcane, target, null);

            OnEnemyAction(BattleActionType.Attack, enemy);

            if (target == playerUnit)
                DealDamage(playerUnit, dmg); // Apply damage to P1          
            else if (target == player2Unit)
                DealDamage(player2Unit, dmg); // Apply damage to P2

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
        OnPlayerAction(BattleActionType.StopGaurding, playerUnit);
        OnPlayerAction(BattleActionType.StopGaurding, player2Unit);

        if (!isPlayerDead)
            state = BattleState.PLAYERTURN;
        else
            state = BattleState.SECONDPLAYERTURN;

        PlayerTurn();
    }
    private void DealDamage(PlayableCharacter player, int damage)
    {
        LastDamage = damage;
        isPlayerDead = player.TakeDamage(damage);
        OnPlayerAction(BattleActionType.Damaged, player);
        battleHUD.SetHP(player);
        dialogueText.text = player.unitName + " takes " + damage + " damage!";

        // if the hit kills the player, play the death animation
        if (player.currentHP <= 0)
        {
            OnPlayerAction(BattleActionType.Die, player);
        }
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
        yield return new WaitForSeconds(.5f);
        playerCollision.battleTransitionAnimator.SetTrigger("End");
        yield return new WaitForSeconds(2f);
        player2Unit.transform.localScale = new Vector3(2, 2, 2);
        OnPlayerAction(BattleActionType.Won, playerUnit);
        OnPlayerAction(BattleActionType.Won, player2Unit);
        playerController.enabled = true;
        SceneManager.LoadScene(sceneIndex);
    }

    #endregion

    #region Misc. Methods

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
                Outline outlineComponent = highlight.gameObject.GetComponent<Outline>();
                if (outlineComponent != null)
                {
                    outlineComponent.enabled = false;
                }
                highlight = null;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
            {

                Transform hitTransform = raycastHit.transform;
                if (hitTransform != null)
                {
                    highlight = hitTransform;

                    EnemyAI enemyAI = highlight.gameObject.GetComponent<EnemyAI>();
                    Player playerComponent = highlight.gameObject.GetComponent<Player>();

                    // if selected object is an enemy
                    if (enemyAI != null && (selectedSpell == null || !selectedSpell.isHealingSpell))
                        SelectEnemy("EnemyMat");
                    else if (playerComponent != null && selectedSpell != null && selectedSpell.isHealingSpell)
                        SelectEnemy("PlayerMat");
                    else
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
        else
        {
            // Highlight
            if (highlight != null)
            {
                Outline outlineComponent = highlight.gameObject.GetComponent<Outline>();
                if (outlineComponent != null)
                {
                    outlineComponent.enabled = false;
                }
                highlight = null;
            }
        }


        if (levelUpManager.expDone && levelUpManager.goldDone)
        {
            levelUpManager.expDone = false;
            levelUpManager.goldDone = false;
            levelUpManager.continueButton.SetActive(true);
        }
    }

    private void SelectEnemy(string materialName)
    {
        Outline outline;

        if (highlight.gameObject.GetComponent<Outline>() != null)
        {
            outline = highlight.gameObject.GetComponent<Outline>();
        }
        else
        {
            outline = highlight.gameObject.AddComponent<Outline>();
        }

        outline.SetMaterialName(materialName);
        outline.enabled = true;
    }


    private int DetermineDamage(int givingDmgStat, Unit recievingDmg, Weapon playersWeapon)
    {
        // Add equipped weapons power to the damage
        Weapon weapon = null;
        if (playersWeapon != null)
            weapon = playersWeapon.GetComponent<Weapon>();

        // Determine the defensive stat of the unit
        int armorDefense = (recievingDmg.equippedArmor != null) ? recievingDmg.equippedArmor.defense : 0;
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
        int damage = Mathf.Max(1, givingDmgStat - recievingDmg.baseDefense - armorSpecDefense);

        if (weapon != null)
            damage += weapon.arcane;
        return damage;
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
        if (state == BattleState.FLEE || state == BattleState.WON)
        {
            if (isPlayerDead)
                playerUnit.currentHP = 1;
            if (isPlayer2Dead)
                player2Unit.currentHP = 1;
        }

        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
            OnBattleState(BattleState.WON);
            GameData.battleCompleted = true;

            StartCoroutine(levelUpManager.GainExp(playerUnit, levelUpManager.p1XpSlider, dialogueText));
            StartCoroutine(levelUpManager.GainExp(player2Unit, levelUpManager.p2XpSlider, dialogueText));
            StartCoroutine(levelUpManager.GainGold(playerUnit));
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

    }

    public void LoadWorldMethod(int sceneIndex)
    {
        StartCoroutine(LoadWorld(sceneIndex));
    }
    #endregion

}
