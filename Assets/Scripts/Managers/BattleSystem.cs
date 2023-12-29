using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Used for the general state of the battle
public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, FLEE }

// Used for specific enemy and player actions
public enum BattleActionType{ Start, PrepareAttack, Attack, Run, RunBack, Gaurd, StopGaurding, Arcane, Damaged, Healed, Flee, Die, Won }
public enum CameraActionType{ GoToStart, GoBehindPlayer, EnemyAttacking}

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
    List<PlayableCharacter> partyManager;
    public Dictionary<string, GameObject> enemyDictionary;
    string enemyTag = PlayerCollision.enemyTag;
    public Transform[] enemyBattleStations;
    public GameObject[] enemyPrefabList;
    public List<Unit> activeEnemies;
    public int[] activeBattleStationIndex;

    private float shrinkDuration = 1f;
    Unit enemyUnit;

    // Player Instantiation fields
    public Transform[] playerBattleStations;

    private int[] baseDefenses;
    private int currentPlayerIndex;
    public Text dialogueText;
    private BattleState state;
    public Animator backButtonUI;
    public Animator combatButtonUI;
    public List<Button> spells;
    private int spellDamage;
    private Spell selectedSpell;

    private LevelUpManager levelUpManager;
    public BattleHUD battleHUD;

    public Material outlineMaterial;
    public GameObject background;

    public int LastDamage { get; private set; }

    // Event for handling player and enemy animations
    public delegate void BattleActionHandler(BattleActionType actionType, Unit unit, UnitType unitType);
    public static event BattleActionHandler OnBattleAction;

    public delegate void CameraActionHandler(CameraActionType actionType, int cameraPos);
    public static event CameraActionHandler OnCameraAction;

    // Event for handling general battle audio/animation
    public delegate void BattleStateHandler(BattleState actionType);
    public static event BattleStateHandler OnBattleState;

    public delegate int DamageCalculationDelegate(int damageStat, Unit selectedEnemy, Weapon equippedWeapon);
    #endregion

    #region Battle Setup

    private void Awake()
    {
        partyManager = PartyManager.instance.partyMembers;
        enemyDictionary = new Dictionary<string, GameObject>();
        baseDefenses = new int[partyManager.Count];


        foreach (GameObject enemy in enemyPrefabList)
            enemyDictionary.Add(enemy.tag, enemy);

        for(int i = 0; i < partyManager.Count; i++)
             baseDefenses[i] = partyManager[i].baseDefense;

            levelUpManager = GetComponent<LevelUpManager>();
    }

    void Start()
    {
        currentPlayerIndex = 0; // Start with the first player
        state = BattleState.START;
        PlayerCollision.instance.battleTransitionAnimator.SetTrigger("Start");
        StartCoroutine(SetupBattle());
        levelUpManager.totalExp = 0;
        Cursor.lockState = CursorLockMode.None;
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
        for (int i = 0; i < partyManager.Count; i++) 
        {
            PlayableCharacter member = PartyManager.instance.partyMembers[i];

            // Calculate the position index based on the number of party members
            int positionIndex = CalculatePositionIndex(partyManager.Count, i);

            // Position each character based on the calculated index
            member.transform.position = playerBattleStations[positionIndex].position;

            member.transform.rotation = Quaternion.Euler(0, 0, 0);
            member.battleIndex = positionIndex;
            // Character specific changes
            if(member.tag == "Player")
            {
                member.GetComponent<CharacterController>().enabled = false;
            }
            else if (member.tag == "Puck")
            {
                member.transform.localScale = new Vector3(3, 3, 3);
                Puck.instance.GetComponent<Follow>().enabled = false;
            }
        }

        battleHUD.UpdateHUDs();
    }

    private int CalculatePositionIndex(int partyCount, int memberIndex)
    {
        switch (partyCount)
        {
            case 1: return 3; // Center
            case 2: return memberIndex == 0 ? 2 : 4; // Left and Right
            case 3: return 1 + memberIndex * 2; // Spread out
            case 4: return memberIndex * 2; // Evenly spaced
            default: return -1; // Error case or handle as needed
        }
    }

    IEnumerator SetupBattle()
    {

        SetupPlayers();

        List<string> keys = new List<string>(enemyDictionary.Keys);

        int num = 1;

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

        foreach (PlayableCharacter player in partyManager)
            OnBattleAction(BattleActionType.Start, player, UnitType.Player);

        // foreach enemy, play the start animation
        foreach (Unit enemy in activeEnemies)
            OnBattleAction(BattleActionType.Start, enemy, UnitType.Enemy);


        //if (num == 1)
        ////dialogueText.text = enemyUnit.unitName + " approaches...";
        //else
        ////dialogueText.text = num + " " + enemyUnit.unitName + "s approach...";
        foreach (PlayableCharacter player in partyManager)
            battleHUD.UpdateAllStats(player);

        yield return new WaitForSeconds(4f);
        combatButtonUI.SetTrigger("int");
        state = BattleState.PLAYERTURN;
    }

    private void AdvanceTurn()
    {
        // If a player is dead
        UpdatePlayerStates();

        // Increment the current player index and check if it exceeds the party size
        currentPlayerIndex = (currentPlayerIndex + 1) % PartyManager.instance.partyMembers.Count;

        if (activeEnemies.Count == 0)
        {
            backButtonUI.SetTrigger("out");
            combatButtonUI.SetTrigger("out");
            state = BattleState.WON;
            EndBattle();
        }
        // If we've looped back to the first player, it's the enemy's turn
        if (currentPlayerIndex == 0)
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        else if (currentPlayerIndex != 0 && !PartyManager.instance.partyMembers[currentPlayerIndex].isDead)
        {
            state = BattleState.PLAYERTURN;
            combatButtonUI.SetTrigger("in");
        }
    }


    private void InstantiateEnemies(GameObject enemyToInstantite, int battleStationIndex)
    {
        enemyBattleStations[battleStationIndex].gameObject.SetActive(true);
        GameObject enemyGameObj = Instantiate(enemyToInstantite, enemyBattleStations[battleStationIndex]);
        enemyUnit = enemyGameObj.GetComponent<Unit>();
        enemyUnit.GetComponent<EnemyAI>().enabled = false;
        activeEnemies.Add(enemyUnit);
        enemyUnit.battleIndex = battleStationIndex;
    }
    #endregion

    #region Player Options

    /// <summary>
    /// When the attack button is pressed
    /// waits for an enemy to be selected
    /// </summary>
    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        ////dialogueText.text = "Who?";
        PlayableCharacter currentPlayer = partyManager[currentPlayerIndex];
        OnBattleAction(BattleActionType.PrepareAttack, currentPlayer, UnitType.Player);

        switch (state)
        {
            case BattleState.PLAYERTURN:
                StartCoroutine(WaitForAttackSelection(partyManager[currentPlayerIndex].baseAttack));
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
            PlayableCharacter currentPlayer = partyManager[currentPlayerIndex];
            for (int i = 0; i < currentPlayer.spellbook.spells.Count; i++)
            {
                Spell spell = currentPlayer.spellbook.spells[i];
                spells[i].GetComponent<Text>().text = spell.spellName;
                Debug.Log(spell.spellName);

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
            StartCoroutine(WaitForArcaneSelection(partyManager[currentPlayerIndex].baseArcane));
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
        for (int i = 0; i < partyManager.Count; i++)
        {
            partyManager[i].baseDefense = baseDefenses[i];
        }
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

        // Turn the HUD off
        battleHUD.UpdateAllStats(player);

        // Determine the damage and if the enemy is dead
        damage = damageCalculation(damageStat, selectedEnemy, player.equippedWeapon);
        LastDamage = damage;
        isDead = selectedUnit.TakeDamage(damage);
        // Rotate player to face enemy
        StartCoroutine(RotatePlayer(player, selectedEnemy));

        bool animationDone = false;
        bool attackApexReached = false;
        void OnAnimationFinished() => animationDone = true;
        void OnAnimationApex() => attackApexReached = true;

        PlayerMovement.instance.OnAnimationComplete += OnAnimationFinished;
        PlayerMovement.instance.OnAttackApex += OnAnimationApex;

        while (!animationDone)
        {
            if(attackApexReached)
            {
                OnBattleAction(BattleActionType.Damaged, selectedEnemy, UnitType.Enemy);
                attackApexReached = false;
            }

            yield return null;
        }

        PlayerMovement.instance.OnAnimationComplete -= OnAnimationFinished;
        PlayerMovement.instance.OnAttackApex -= OnAnimationApex;
        PlayableCharacter currentPlayer = partyManager[currentPlayerIndex];

        // Return to original positions
        OnBattleAction(BattleActionType.RunBack, currentPlayer, UnitType.Player);
        //Do not trigger the jump coroutine because Puck can fly
        //if (currentPlayer.tag == "Puck") 
          StartCoroutine(MoveOverTime(currentPlayer.gameObject, playerBattleStations[currentPlayer.battleIndex].transform.position, .75f)); // Assuming 1f is the jump height
        //else
        //  StartCoroutine(MoveOverTimeWithJump(currentPlayer.gameObject, playerBattleStations[currentPlayer.battleIndex].transform.position, .5f, 2f)); // Assuming 1f is the jump height

        if (isDead)
            HandleEnemyDefeat(selectedEnemy);

        yield return new WaitForSeconds(1f);

        currentPlayer.gameObject.transform.position = playerBattleStations[currentPlayer.battleIndex].transform.position;
        OnCameraAction(CameraActionType.GoToStart, 4);
        // If an enemy dies
         
        
        AdvanceTurn();

    }

    private void HandleEnemyDefeat(Unit enemy)
    {

        OnBattleAction(BattleActionType.Die, enemy, UnitType.Enemy);
        levelUpManager.GainXPAndGold(enemy);

        // Slowly shrink enemy
        StartCoroutine(ShrinkAndDestroyEnemy(enemy));
    }

    IEnumerator ShrinkAndDestroyEnemy(Unit enemy)
    {
        activeEnemies.Remove(enemy);
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

        yield return new WaitForSeconds(.25f);

        Destroy(enemy.gameObject);

        if (activeEnemies.Count == 0)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
            AdvanceTurn();
    }

    /// <summary>
    /// Reduces incoming damage by half
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayerGaurd()
    {
        PlayableCharacter currentPlayer = partyManager[currentPlayerIndex];
        currentPlayer.isGaurding = true;
        
        OnBattleAction(BattleActionType.Gaurd, currentPlayer, UnitType.Player);
        currentPlayer.baseDefense *= 2;
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

        backButtonUI.SetTrigger("out");

        PlayableCharacter currentPlayer = partyManager[currentPlayerIndex];

        // Move camera behind player and run towards the selected enemy
        OnCameraAction(CameraActionType.GoBehindPlayer, currentPlayer.battleIndex);
        yield return new WaitForSeconds(1f);
        OnBattleAction(BattleActionType.Run, currentPlayer, UnitType.Player);

        // In front of selected enemy
        Vector3 playerAttackPos = new Vector3(
            selectedUnit.transform.position.x,
            currentPlayer.transform.position.y,
            selectedUnit.transform.position.z - 3);

        StartCoroutine(MoveOverTime(currentPlayer.gameObject, playerAttackPos, .5f));

        StartCoroutine(PlayerAction(currentPlayer, selectedUnit, damageStat, DetermineDamage));
        SetHighlightable(false);
        OnBattleAction(BattleActionType.Attack, currentPlayer, UnitType.Player);
        selectedUnit = null;
    }

    private IEnumerator MoveOverTime(GameObject objectToMove, Vector3 targetPos, float time)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;  

        while (elapsedTime < time)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, targetPos, elapsedTime/time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        objectToMove.transform.position = targetPos;
    }

    private IEnumerator MoveOverTimeWithJump(GameObject objectToMove, Vector3 targetPos, float time, float jumpHeight)
    {
        Vector3 startPos = objectToMove.transform.position;

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            float t = elapsedTime / time;

            // Apply a sine function to the vertical movement for a more natural jump
            float yOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;

            // Calculate the new position with non-linear interpolation for Y-axis
            Vector3 newPosition = new Vector3(
                Mathf.Lerp(startPos.x, targetPos.x, t),
                startPos.y + yOffset,
                Mathf.Lerp(startPos.z, targetPos.z, t)
            );

            objectToMove.transform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the object reaches the target position
        objectToMove.transform.position = targetPos;


    }

    IEnumerator WaitForArcaneSelection(int damageStat)
    {
        while (selectedUnit == null)
            yield return null;

        Debug.Log(selectedSpell);

        if (selectedSpell.isHealingSpell)
            StartCoroutine(PlayerHeal(selectedUnit));

        if (!selectedSpell.isHealingSpell)
        {
            damageStat += spellDamage;
            selectedSpell.spellVFX.transform.position = selectedUnit.transform.position;
            selectedSpell.spellVFX.Play();

            PlayableCharacter currentPlayer = partyManager[currentPlayerIndex];
            StartCoroutine(PlayerAction(currentPlayer, selectedUnit, damageStat, DetermineDamageArcane));
        }
    }

    IEnumerator PlayerHeal(Unit selectedPlayer)
    {
        int healingAmnt = Random.Range(selectedSpell.minDamage, selectedSpell.maxDamage);

        selectedPlayer.Heal(healingAmnt);

        battleHUD.UpdateAllStats(selectedPlayer);

        PlayableCharacter currentPlayer = partyManager[currentPlayerIndex];
        LastDamage = healingAmnt;
        OnBattleAction(BattleActionType.Healed, currentPlayer, UnitType.Player);
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
        UpdatePlayerStates();
        bool doneAttacking = false;

        while (!doneAttacking)
        {
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                bool animationDone = false;
                bool attackApexReached = false;
                void OnAnimationFinished() => animationDone = true;
                void OnAnimationApex() => attackApexReached = true;

                activeEnemies[i].GetComponent<MonsterAnimController>().OnAnimationComplete += OnAnimationFinished;
                activeEnemies[i].GetComponent<MonsterAnimController>().OnAttackApex += OnAnimationApex;

                // If the enemy can use magic, give them a chance of doing so
                int attackType = activeEnemies[i].canUseMagic ? Random.Range(0, 2) : 0;
                yield return new WaitForSeconds(1.5f);

                // Select target from active players
                PlayableCharacter target = SelectTarget();
                int dmg = attackType == 0 ? DetermineDamage(activeEnemies[i].baseAttack, target, null)
                                      : DetermineDamageArcane(activeEnemies[i].baseArcane, target, null);


                Vector3 enemyAttackPos = new Vector3(
                    target.transform.position.x,
                    activeEnemies[i].transform.position.y,
                    target.transform.position.z + 3);

                OnBattleAction(BattleActionType.Run, activeEnemies[i], UnitType.Enemy);
                yield return StartCoroutine(MoveOverTime(activeEnemies[i].gameObject, enemyAttackPos, .5f));
                yield return new WaitForSeconds(.25f);

                OnCameraAction(CameraActionType.EnemyAttacking, target.battleIndex);
                OnBattleAction(BattleActionType.Attack, activeEnemies[i], UnitType.Enemy);
                
                // Play the players damaged anim at the apex of the attack
                while (!animationDone)
                {
                    if (attackApexReached)
                    {
                        OnBattleAction(BattleActionType.Damaged, target, UnitType.Player);
                        attackApexReached = false;
                    }
                    yield return null;
                }

                DealDamage(target, dmg);

                OnBattleAction(BattleActionType.RunBack, activeEnemies[i], UnitType.Enemy);
                yield return StartCoroutine(MoveOverTime(activeEnemies[i].gameObject, enemyBattleStations[activeEnemies[i].battleIndex].transform.position, .5f));
                
                
                if (AllPlayersDefeated())
                {
                    state = BattleState.LOST;
                    EndBattle();
                    yield break; // Exit the coroutine
                }
                

            }

            doneAttacking = true;

        }

        // Reset defense after gaurding is complete
        ResetStats();

        foreach (PlayableCharacter player in partyManager)
        {
            OnBattleAction(BattleActionType.StopGaurding, player, UnitType.Player);
            player.isGaurding = false;
        }

        currentPlayerIndex = 0;
        state = BattleState.PLAYERTURN;
        combatButtonUI.SetTrigger("in");
        OnCameraAction(CameraActionType.GoToStart, 4);
    }

    private void UpdatePlayerStates()
    {
        foreach (var player in partyManager)
        {
            if (player.currentHP == 0)
                player.isDead = true;
        }
    }

    private PlayableCharacter SelectTarget()
    {
        // Ensure there is at least one alive player
        if (partyManager.All(player => player.isDead))
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
        battleHUD.UpdateAllStats(player);
        ////dialogueText.text = player.unitName + " takes " + damage + " damage!";

        // if the hit kills the player, play the death animation
        if (isDead)
            OnBattleAction(BattleActionType.Die, player, UnitType.Player);
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
        PlayerCollision.instance.battleTransitionAnimator.SetTrigger("End");
        yield return new WaitForSeconds(2f);

        foreach(var player in partyManager)
        {
            OnBattleAction(BattleActionType.Won, player, UnitType.Player);
         
            if(player.tag == "Puck")
                player.transform.localScale = new Vector3(2, 2, 2);
        }

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
        //int armorDefense = (recievingDmg.equippedArmor != null) ? recievingDmg.equippedArmor.defense : 0;
        int damage = Mathf.Max(1, givingDmgStat - recievingDmg.baseDefense /*- armorDefense*/);

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

    public void SetHighlightable(bool active)
    {
        canSelect = active;
    }

    void EndBattle()
    {
        backButtonUI.SetTrigger("out");
        combatButtonUI.SetTrigger("out");
        
        if (state == BattleState.FLEE || state == BattleState.WON)
        {
            foreach(PlayableCharacter player in partyManager)
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
            OnBattleState(BattleState.WON);
            GameData.battleCompleted = true;
            
            for(int i = 0; i < partyManager.Count; i++)
                StartCoroutine(levelUpManager.GainExp(partyManager[i], levelUpManager.xpSliders[i]));

            StartCoroutine(levelUpManager.GainGold(partyManager[0]));
        }
        else if (state == BattleState.FLEE)
        {
            StartCoroutine(LoadWorld(1));
        }
        else if (state == BattleState.LOST)
        {
            StartCoroutine(LoadWorld(0));
        }

    }

    public void LoadWorldMethod(int sceneIndex)
    {
        StartCoroutine(LoadWorld(sceneIndex));
    }
    #endregion

}
