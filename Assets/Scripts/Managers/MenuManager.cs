using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject statsMenu;


    public static event Action OnInvOpened;
    public PlayableCharacter[] players = new PlayableCharacter[4];
    public HudInfo[] hudInfos = new HudInfo[4]; // Max 4 players in a battle

    private PauseGame pauseGame;
    [HideInInspector] public bool menuScreenActive;
    public Inventory invManager;

    public GameObject uiCanvas;
    public BattleHUD battleHud;

    public GameObject loadingScreen;
    public Slider loadingBarFill;

    private int selectedSaveOrLoadFile = -1;
    public GameObject confirmationUI;
    public GameObject[] saveSlots;
    public LocationNameUpdater location;

    public Animator battleTransition;

    private void Start()
    {
        pauseGame = gameObject.GetComponent<PauseGame>();

        for (int i = 0; i < players.Length; i++)
            PartyManager.instance.
                
                
                (players[i]);
    }

    private void OnEnable()
    {
        foreach(PlayableCharacter player in players)
            player.OnStatsChanged += UpdateStats;

        invManager.OnItemEquipped += UpdateStats;
    }

    public void UpdateStats()
    {
        for (int i = 0; i < hudInfos.Length; i++)
        {
            if (i < players.Length)
               hudInfos[i].UpdateInventory(players[i]);
            else
               hudInfos[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UpdateStats();
            OnInvOpened?.Invoke();

            statsMenu.SetActive(!statsMenu.activeSelf);
            if (!statsMenu.activeSelf)
            {
                Time.timeScale = 1;
                menuScreenActive = false;
            }
            else
            {
                menuScreenActive = true;
                Time.timeScale = 0;
            }
        }
    }

    public void OnResume()
    {
        statsMenu?.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnSaveSlotSelected(int saveSlotId)
    {
        selectedSaveOrLoadFile = saveSlotId;
        confirmationUI.SetActive(true);
    }

    public void OnConfirmSave()
    {
        if (selectedSaveOrLoadFile != -1)
        {
            int index = selectedSaveOrLoadFile - 1;

            SaveSlot saveSlot = saveSlots[index].GetComponent<SaveSlot>();
            if (saveSlot != null)
                saveSlot.UpdateSaveSlot(location.locationNameText, players[0].isActive, players[1].isActive, players[0].gold);

            OnSave(selectedSaveOrLoadFile);
            Debug.Log("Saved to file #" + selectedSaveOrLoadFile);
            selectedSaveOrLoadFile = -1;
        }
        else
        {
            Debug.LogError("Could Not Save file. Save File: " + selectedSaveOrLoadFile);
        }

    }

    public void OnLoadSlotSelected(int saveSlotId)
    {
        selectedSaveOrLoadFile = saveSlotId;
        confirmationUI.SetActive(true);
    }

    public void OnConfirmLoad()
    {
        if (selectedSaveOrLoadFile != -1)
        {
            OnLoad(selectedSaveOrLoadFile);
            Debug.Log("Load to file #" + selectedSaveOrLoadFile);
            foreach (PlayableCharacter player in players)
            {
                battleHud.UpdateAllStats(player);
            }
            selectedSaveOrLoadFile = -1;
        }
        else
        {
            Debug.LogError("Could Not Save file. Save File: " + selectedSaveOrLoadFile);
        }
    }

    public void OnSave(int saveSlotID)
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            foreach (PlayableCharacter player in players)
            {
                SaveSystem.SavePlayer(saveSlotID, player);
            }

            foreach (var enemy in GameData.enemies)
            {
                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();

                SaveSystem.SaveGameData(saveSlotID, enemyAI.enemyID, enemyAI.isDefeated);
            }
        }
        else
        {
            pauseGame.popupAnimator.SetTrigger("in");
        }
    }

    public void OnLoad(int saveSlotID)
    {
        battleTransition.SetTrigger("End");
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            foreach(PlayableCharacter player in players)
            {
                LoadPlayer(saveSlotID, player);
            }

            LoadGameData(saveSlotID);
            Time.timeScale = 1;
        }
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            SceneManager.LoadScene(1);

            foreach (PlayableCharacter player in players)
            {
                LoadPlayer(saveSlotID, player);

                if(player.tag == "Puck")
                {
                    player.gameObject.transform.localScale = new Vector3(2, 2, 2);
                }
            }

        }

        StartCoroutine(BattleTranstion(2f));
    }

    public void LoadGameData(int saveSlotID, bool loadPositionAndRotationOnly = false)
    {
        EnemyData data = SaveSystem.LoadGameData(saveSlotID);

        if (data != null)
        {
            foreach (var enemy in GameData.enemies)
            {
                enemy.gameObject.SetActive(true);

                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    if (loadPositionAndRotationOnly)
                    {
                        // Load only position and rotation
                        if (data.enemyPositions.TryGetValue(enemyAI.enemyID, out float[] posArray) &&
                            data.enemyRotations.TryGetValue(enemyAI.enemyID, out float[] rotArray))
                        {
                            enemy.transform.position = new Vector3(posArray[0], posArray[1], posArray[2]);
                            enemy.transform.rotation = new Quaternion(rotArray[0], rotArray[1], rotArray[2], rotArray[3]);
                        }
                    }
                    else
                    {
                        if (data.enemies.TryGetValue(enemyAI.enemyID, out bool isDefeated))
                        {
                            enemyAI.isDefeated = isDefeated;
                            float[] posArray = data.enemyPositions[enemyAI.enemyID];
                            float[] rotArray = data.enemyRotations[enemyAI.enemyID];
                            enemy.transform.position = new Vector3(posArray[0], posArray[1], posArray[2]);
                            enemy.transform.rotation = new Quaternion(rotArray[0], rotArray[1], rotArray[2], rotArray[3]);
                        }
                        else
                        {
                            Debug.LogError("Enemy not found in saved data: " + enemyAI.enemyID);
                        }
                    }

                }
            }
        }
    }

    private void LoadPlayer(int saveSlotID, PlayableCharacter player)
    {
        PlayerData data = SaveSystem.LoadPlayer(saveSlotID, player.name);

        player.unitName = data.unitName;
        player.unitLevel = data.unitLevel;
        player.maxHP = data.maxHP;
        player.currentHP = data.currentHP;
        player.maxMP = data.maxMP;
        player.currentMP = data.currentMP;
        player.baseAttack = data.baseAttack;
        player.baseArcane = data.baseArcane;
        player.baseDefense = data.baseDefense;
        player.specialDefense = data.specialDefense;
        player.experience = data.experience;
        player.gold = data.gold;
        player.expToNextLevel = data.expToNextLevel;
        player.isActive = data.active;

        Vector3 position;
        Vector3 rotation;

        if (player.GetComponent<CharacterController>() != null)
            player.GetComponent<CharacterController>().enabled = false;

        //Load the position
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        player.gameObject.transform.position = position;

        //Load the rotation
        rotation.x = data.rotation[0];
        rotation.y = data.rotation[1];
        rotation.z = data.rotation[2];
        player.gameObject.transform.rotation = Quaternion.Euler(rotation);
        if (player.GetComponent<CharacterController>() != null)
            player.GetComponent<CharacterController>().enabled = true;
    }

    public void OnLevelWasLoaded(int level)
    {
        Vector3 beforeBattlePos = players[0].GetComponent<PlayerCollision>().beforeBattlePos;

        if (level == 1)
        {
            foreach (PlayableCharacter player in players)
            {
                player.transform.position = beforeBattlePos;
            }

            players[0].GetComponent<PlayerMovement>().enabled = true;
            players[1].GetComponent<Follow>().enabled = true;
            uiCanvas.SetActive(true);

            battleHud.SetHUD();
        }

        if (level == 2)
        {
            uiCanvas.SetActive(false);
        }
    }

    IEnumerator BattleTranstion(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        battleTransition.SetTrigger("Start");
    }

    public void QuitToMenu(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        Time.timeScale = 1.0f;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBarFill.value = progressValue;

            yield return null;
        }
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        foreach (PlayableCharacter player in players)
        {
            player.OnStatsChanged -= UpdateStats;
        }
    }
}
