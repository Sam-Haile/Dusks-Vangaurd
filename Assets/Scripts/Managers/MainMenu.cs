using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MainMenu : MonoBehaviour
{

    public GameObject loadingScreen;
    public GameObject saveLoadUI;
    public Slider loadingBarFill;

    private bool isLoading = false;
    private int selectedSaveOrLoadFile = -1;


    private void Start()
    {
        Debug.Log(PlayerPrefs.GetInt("FirstTime")); 
    }


    #region Load From Main Menu
    public void OnLoadSlotSelected(int saveSlotId)
    {
        selectedSaveOrLoadFile = saveSlotId;
    }

    public void OnConfirmLoad()
    {
        if (selectedSaveOrLoadFile != -1)
        {
            OnLoad(selectedSaveOrLoadFile);
            Debug.Log("Load to file #" + selectedSaveOrLoadFile);
            //foreach (PlayableCharacter player in players)
            //{
            //    battleHud.UpdateAllStats(player);
            //}

            //UpdateMoney();
            selectedSaveOrLoadFile = -1;
        }
        else
        {
            Debug.LogError("Could Not Save file. Save File: " + selectedSaveOrLoadFile);
        }
    }

    public void OnLoad(int saveSlotID)
    {

        LoadSceneAsync(1);

        foreach (PlayableCharacter player in PartyManager.instance.partyMembers)
        {
            LoadPlayer(saveSlotID, player);

        }

        Debug.Log("Starting");
        //saveLoadTransition.SetTrigger("Start");

        //StartCoroutine(BattleTranstion(2f));
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
    #endregion


    private void DestroyPersistentObjects()
    {
        // Find all objects with the tag "PersistentObject"
        GameObject[] persistentObjects = GameObject.FindGameObjectsWithTag("PersistentObject");

        // Destroy each persistent object
        foreach (GameObject persistentObject in persistentObjects)
        {
            Destroy(persistentObject);
        }
    }

    public void UICheck()
    {
        if (PlayerPrefs.GetInt("FirstTime", 1) == 2)
        {
            Debug.Log("FirstTime");
            // This is the first time the game is being opened
            PlayerPrefs.Save();
            DisableSaveLoadUI();
            StartCoroutine(LoadSceneAsync(1));
        }
        else
        {
            Debug.Log("SecondTime");
            EnableSaveLoadUI();
        }
    }

    void DisableSaveLoadUI()
    {
        saveLoadUI.SetActive(false);
    }

    void EnableSaveLoadUI()
    {
        saveLoadUI.SetActive(true);
    }

    // Start Button
    public void OnStart()
    {
        if (isLoading) return;

        isLoading = true;
        StartCoroutine(LoadSceneAsync(1));
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    // Loads Credits
    public void LoadLink(string hyperlink)
    {
        Application.OpenURL(hyperlink);
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            
            loadingBarFill.value = progressValue;

            yield return null;
        }
    }
}
