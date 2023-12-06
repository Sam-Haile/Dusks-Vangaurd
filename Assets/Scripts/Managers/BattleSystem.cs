using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

// Used for the general state of the battle
public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, FLEE }

// Used for specific enemy and player actions
public enum BattleActionType{ Start, Attack, Gaurd, StopGaurding, Arcane, Damaged, Healed, Run, Die, Won }

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
    PartyManager partyManager = PartyManager.instance;
    public Dictionary<string, GameObject> enemyDictionary = new Dictionary<string, GameObject>();
    string enemyTag = PlayerCollision.enemyTag;
    public Transform[] enemyBattleStations;
    public GameObject[] enemyPrefabList;
    [HideInInspector] public List<Unit> activeEnemies;
    private float shrinkDuration = 1f;
    Unit enemyUnit;

    // Player1 Instantiation fields
    public Transform[] playerBattleStations;
    private int[] baseDefenses;
    private int currentPlayerIndex;
    private PlayerCollision playerCollision;
    private PlayerMovement playerMovement;
    private Transform playerPos;

    // Player2 Instantiation fields
    private Follow player2FollowScript;


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


    // Event for handling player and enemy animations
    public delegate void BattleActionHandler(BattleActionType actionType, Unit unit);
    public static event BattleActionHandler OnBattleAction;

    // Event for handling general battle audio/animation
    public delegate void BattleStateHandler(BattleState actionType);
    public static event BattleStateHandler OnBattleState;

    public delegate int DamageCalculationDelegate(int damageStat, Unit selectedEnemy, Weapon equippedWeapon);

    #endregion

    #region Battle Setup

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;

        //playerPos = Player.instance.transform;
        playerMovement = PlayerMovement.instance;
        playerCollision = PlayerCollision.instance;

        foreach (GameObject enemy in enemyPrefabList)
            enemyDictionary.Add(enemy.tag, enemy);

        levelUpManager = GetComponent<LevelUpManager>();
    }

    void Start()
    {
        currentPlayerIndex = 0; // Start with the first player
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

        for (int i = 0; i < partyManager.partyMembers.Count; i++) 
        {
            PlayableCharacter member = PartyManager.instance.partyMembers[i];

            // Spawn players at specific points depending on the number of party members
            switch (partyManager.partyMembers.Count)
            {
                case 1:
                    // Position each character based on their index and party size
                    member.transform.position = playerBattleStations[3].position;
                    break;
                case 2:
                    member.transform.position = playerBattleStations[2].position;
                    member.transform.position = playerBattleStations[4].position;
                    break;
                case 3:

                    member.transform.position = playerBattleStations[1].position;
                    member.transform.position = playerBattleStations[3].position;
                    member.transform.position = playerBattleStations[5].position;
                    break;
                case 4:
                    member.transform.position = playerBattleStations[0].position;
                    member.transform.position = playerBattleStations[2].position;
                    member.transform.position = playerBattleStations[4].position;
                    member.transform.position = playerBattleStations[6].position;
                    break;
                default:
                    break;
            }

            
            member.transform.rotation = Quaternion.Euler(0, 0, 0);

            // Character specific changes
            if(member.tag == "Guts")
            {
                playerMovement.isMoving = false;
                playerMovement.enabled = false;
            }
            else if (member.tag == "Puck")
            {
                member.transform.localScale = new Vector3(3, 3, 3);
                player2FollowScript.enabled = false;
            }

        }

        battleHUD.UpdateHUDs();
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

        foreach (PlayableCharacter player in partyManager.partyMembers)
            OnBattleAction(BattleActionType.Start, player);

        // foreach enemy, play the start animation
        foreach (Unit enemy in activeEnemies)
            OnBattleAction(BattleActionType.Start, enemy);


        //if (num == 1)
        ////dialogueText.text = enemyUnit.unitName + " approaches...";
        //else
        ////dialogueText.text = num + " " + enemyUnit.unitName + "s approach...";
        foreach (PlayableCharacter player in partyManager.partyMembers)
            battleHUD.UpdateAllStats(player);

        yield return new WaitForSeconds(4f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    private void AdvanceTurn()
    {
        // Increment the current player index and check if it exceeds the party size
        currentPlayerIndex = (currentPlayerIndex + 1) % PartyManager.instance.partyMembers.Count;

        // If we've looped back to the first player, it's the enemy's turn
        if (currentPlayerIndex == 0)
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn(); // Handle the next player's turn
        }
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
        ////dialogueText.text = "";
        SetButtonsActive(true);
    }

    /// <summary>
    /// When the attack button is pressed
    /// waits for an enemy to be selected
    /// </summary>
    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        ////dialogueText.text = "Who?";

        switch (state)
        {
            case BattleState.PLAYERTURN:
                StartCoroutine(WaitForAttackSelection(partyManager.partyMembers[currentPlayerIndex].baseAttack));
                break;
        }
    }

    /// <summary>
    /// When the run button is pressed
    /// has chance of failure
    /// </summary>
    public void OnFleeButton()
    {
        if (state != BattleState.PLAYERTURN)
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
            PlayableCharacter currentPlayer = partyManager.partyMembers[currentPlayerIndex];
            for (int i = 0; i < currentPlayer.spellbook.spells.Count; i++)
            {
                Spell spell = currentPlayer.spellbook.spells[i];
                spells[i].GetComponent<Text>().text = spell.spellName;

                // Add a listener to the button click event
                spells[i].onClick.RemoveAllListeners(); // Remove existing listeners to avoid duplicates
                int index = i;
                spells[i].onClick.AddListener(() =>
                {
                    selectedSpell = currentPlayer.spellbook.spells[index];
                    spellDamage = currentPlayer.spellbook.CastSpell(selectedSpell.spellName, currentPlayer);
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
        if (state != BattleState.PLAYERTURN)
            return;

        if (state == BattleState.PLAYERTURN)
        {
            StartCoroutine(WaitForArcaneSelection(partyManager.partyMembers[currentPlayerIndex].baseArcane));
        }

    }

    /// <summary>
    /// When the gaurd button is pressed
    /// players recieve a x2 buff to defense
    /// </summary>
    public void OnGaurdButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerGaurd());
    }

    /// <summary>
    /// Reset any buffed stats at the end of a battle
    /// </summary>
    private void ResetStats()
    {
        for (int i = 0; i < partyManager.partyMembers.Count; i++)
            partyManager.partyMembers[i].baseDefense = baseDefenses[i];
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
                ////dialogueText.text = "You cannot flee this battle.";
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
            ////dialogueText.text = "Could not flee!";
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
        battleHUD.UpdateAllStats(player);

        // Determine the damage and if the enemy is dead
        damage = damageCalculation(damageStat, selectedEnemy, player.equippedWeapon);
        LastDamage = damage;
        isDead = selectedUnit.TakeDamage(damage);
        StartCoroutine(RotatePlayer(player, selectedEnemy));
        OnBattleAction(BattleActionType.Damaged, selectedEnemy);


        ////dialogueText.text = selectedEnemy.unitName + " takes " + damage + " damage.";
        yield return new WaitForSeconds(2f);

        // If an enemy dies
        // If an enemy dies
        if (isDead)
            HandleEnemyDefeat(selectedEnemy);

        AdvanceTurn();
    }

    private void HandleEnemyDefeat(Unit enemy)
    {
        OnBattleAction(BattleActionType.Die, enemy);
        levelUpManager.GainXPAndGold(enemy);

        // Slowly shrink enemy
        StartCoroutine(ShrinkAndDestroyEnemy(enemy));

        if (activeEnemies.Count == 0)
        {
            state = BattleState.WON;
            EndBattle();
        }

    }

    IEnumerator ShrinkAndDestroyEnemy(Unit enemy)
    {
        Vector3 initialScale = enemy.transform.localScale;
        Vector3 finalScale = Vector3.zero;
        float elapsedTime = 0f;

        yield return new WaitForSeconds(2.5f);

        while (elapsedTime < shrinkDuration)
        {
            enemy.transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsedTime / shrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        enemy.transform.localScale = finalScale;

        yield return new WaitForSeconds(1f);

        activeEnemies.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    /// <summary>
    /// Reduces incoming damage by half
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayerGaurd()
    {
        PlayableCharacter currentPlayer = partyManager.partyMembers[currentPlayerIndex];

        OnBattleAction(BattleActionType.Gaurd, currentPlayer);
        currentPlayer.baseDefense *= 2;
        SetButtonsActive(false);
        yield return new WaitForSeconds(2f);

        AdvanceTurn();
    }

    /// <summary>
    /// Waits for a selection before attacking
    /// </summary>
    /// <param name="damageStat"></param>
    /// <returns>The selected enemy</returns>
    IEnumerator WaitForAttackSelection(int damageStat)
    {
        while (selectedUnit == null)
            yield return null;

        PlayableCharacter currentPlayer = partyManager.partyMembers[currentPlayerIndex];

        StartCoroutine(PlayerAction(currentPlayer, selectedUnit, damageStat, DetermineDamage));
        SetHighlightable(false);
        OnBattleAction(BattleActionType.Attack, currentPlayer);

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

            PlayableCharacter currentPlayer = partyManager.partyMembers[currentPlayerIndex];
            StartCoroutine(PlayerAction(currentPlayer, selectedUnit, damageStat, DetermineDamageArcane));

        }
    }

    IEnumerator PlayerHeal(Unit selectedPlayer)
    {
        int healingAmnt = Random.Range(selectedSpell.minDamage, selectedSpell.maxDamage);

        selectedPlayer.Heal(healingAmnt);

        battleHUD.UpdateAllStats(selectedPlayer);

        PlayableCharacter currentPlayer = partyManager.partyMembers[currentPlayerIndex];
        LastDamage = healingAmnt;
        OnBattleAction(BattleActionType.Healed, currentPlayer);
        battleHUD.UpdateAllStats(currentPlayer);
        yield return new WaitForSeconds(2f);
        AdvanceTurn();
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
        UpdatePlayerStates();


        foreach (Unit enemy in activeEnemies)
        {
            // If the enemy can use magic, give them a chance of doing so
            int attackType = enemy.canUseMagic ? Random.Range(0, 2) : 0;
            yield return new WaitForSeconds(1.5f);

            ////dialogueText.text = enemyUnit.unitName + " attacks!";
            
            // Select target from active players
            PlayableCharacter target = SelectTarget();
            int dmg = attackType == 0 ? DetermineDamage(enemy.baseAttack, target, null)
                                  : DetermineDamageArcane(enemy.baseArcane, target, null);

            OnBattleAction(BattleActionType.Attack, enemy);
            DealDamage(target, dmg);


            if (AllPlayersDefeated())
            {
                state = BattleState.LOST;
                EndBattle();
                yield break; // Exit the coroutine
            }

            // Reset defense after gaurding is complete
            ResetStats();
            //OnBattleAction(BattleActionType.StopGaurding, playerUnit);
            //OnBattleAction(BattleActionType.StopGaurding, player2Unit);

            AdvanceTurn();
        }
    }


    private void UpdatePlayerStates()
    {
        foreach (var player in PartyManager.instance.partyMembers)
        {
            if (player.currentHP == 0)
                player.isDead = true;
        }
    }

    private PlayableCharacter SelectTarget()
    {
        // Ensure there is at least one alive player
        if (partyManager.partyMembers.All(player => player.isDead))
            return null; // Or handle this case appropriately

        PlayableCharacter target;
        do
        {
            target = PartyManager.instance.partyMembers[Random.Range(0, PartyManager.instance.partyMembers.Count)];
        } while (target.isDead);

        return target;
    }

    private bool AllPlayersDefeated()
    {
        // Check if all players are defeated
        return PartyManager.instance.partyMembers.All(player => player.isDead);
    }

    private void DealDamage(PlayableCharacter player, int damage)
    {
        LastDamage = damage;
        bool isDead = player.TakeDamage(damage);
        OnBattleAction(BattleActionType.Damaged, player);
        battleHUD.UpdateAllStats(player);
        ////dialogueText.text = player.unitName + " takes " + damage + " damage!";

        // if the hit kills the player, play the death animation
        if (isDead)
            OnBattleAction(BattleActionType.Die, player);
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

        foreach(var player in partyManager.partyMembers)
        {
            OnBattleAction(BattleActionType.Won, player);
         
            if(player.tag == "Puck")
                player.transform.localScale = new Vector3(2, 2, 2);
        }

        Cursor.lockState = CursorLockMode.Locked;
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
            foreach(PlayableCharacter player in partyManager.partyMembers)
            {
                if (player.isDead)
                {
                    player.currentHP = 1;
                    player.isDead = false;
                }
            }
        }

        if (state == BattleState.WON)
        {
            ////dialogueText.text = "You won the battle!";
            OnBattleState(BattleState.WON);
            GameData.battleCompleted = true;
            
            for(int i = 0; i < partyManager.partyMembers.Count; i++)
                StartCoroutine(levelUpManager.GainExp(partyManager.partyMembers[i], levelUpManager.xpSliders[i]));

            StartCoroutine(levelUpManager.GainGold(partyManager.partyMembers[0]));
        }
        else if (state == BattleState.FLEE)
        {
            ////dialogueText.text = "You fled the battle!";
            StartCoroutine(LoadWorld(1));
        }
        else if (state == BattleState.LOST)
        {
            ////dialogueText.text = "You were defeated.";
            StartCoroutine(LoadWorld(0));
        }

    }

    public void LoadWorldMethod(int sceneIndex)
    {
        StartCoroutine(LoadWorld(sceneIndex));
    }
    #endregion

}
