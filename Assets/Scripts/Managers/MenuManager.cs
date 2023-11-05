using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject statsMenu;
    public GameObject player1;
    public GameObject player2;

    private PlayableCharacter playerInfo;
    private PlayableCharacter player2Info;
    public static event Action OnInvOpened;

    public List<TextMeshProUGUI> player1Stats;
    public List<TextMeshProUGUI> player2Stats;
    public Image player1Weapon;
    public Image player2Weapon;
    public Image player1Armor;
    public Image player2Armor;
    private PauseGame pauseGame;
    public bool menuScreenActive;
    public Inventory invManager;

    public GameObject uiCanvas;
    public BattleHUD battleHud;

    private void Awake()
    {
        player1 = FindObjectOfType<PlayerMovement>().gameObject;
        player2 = FindObjectOfType<Follow>().gameObject;
    }

    private void Start()
    {
        pauseGame = gameObject.GetComponent<PauseGame>();
        playerInfo = player1.GetComponent<Player>();
        player2Info = player2.GetComponent<Puck>();
    }

    private void OnEnable()
    {
        statsMenu.SetActive(false);
        playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayableCharacter>();
        player2Info = GameObject.FindGameObjectWithTag("Puck").GetComponent<PlayableCharacter>();
        playerInfo.OnStatsChanged += UpdateStats;
        player2Info.OnStatsChanged += UpdateStats;
        invManager.OnItemEquipped += UpdateStats;
        UpdateStats();
    }

    public void UpdateStats()
    {
        player1Stats[0].text = playerInfo.unitName.ToString();
        player1Stats[1].text = "Lvl. " + playerInfo.unitLevel.ToString();
        player1Stats[2].text = "EXP: " + playerInfo.experience.ToString();
        player1Stats[3].text = "HP: " + playerInfo.currentHP.ToString() + "/" + playerInfo.maxHP.ToString();


        player1Stats[4].text = "Atk: " + playerInfo.baseAttack.ToString() + (playerInfo.equippedWeapon != null ?
                      " + (" + playerInfo.equippedWeapon.attack + ")" : " + (0)");
        player1Stats[5].text = "Arc: " + playerInfo.baseArcane.ToString();
        player1Stats[6].text = "Def: " + playerInfo.baseDefense.ToString() + (playerInfo.equippedArmor != null ?
                       " + (" + playerInfo.equippedArmor.defense + ")" : " + (0)");
        player1Stats[7].text = "Spc Def: " + playerInfo.specialDefense.ToString();
        player1Stats[8].text = playerInfo.money.ToString();
        if (playerInfo.equippedWeapon != null)
        {
            player1Stats[9].text = "Weapon: " + playerInfo.equippedWeapon.itemName;
            player1Weapon.sprite = playerInfo.equippedWeapon.itemIcon;
        }
        else
            player1Stats[9].text = "Weapon: N/A";

        if (playerInfo.equippedArmor != null)
        {
            player1Stats[10].text = "Armor: " + playerInfo.equippedArmor.itemName;
            player1Armor.sprite = playerInfo.equippedArmor.itemIcon;
        }
        else
            player1Stats[10].text = "Armor: N/A";


        player2Stats[0].text = player2Info.unitName.ToString();
        player2Stats[1].text = "Lvl. " + player2Info.unitLevel.ToString();
        player2Stats[2].text = "EXP: " + player2Info.experience.ToString();
        player2Stats[3].text = "HP: " + player2Info.currentHP.ToString() + "/" + player2Info.maxHP.ToString();
        player2Stats[4].text = "Atk: " + player2Info.baseAttack.ToString() + (player2Info.equippedWeapon != null ?
                      " + (" + player2Info.equippedWeapon.attack + ")" : " + (0)");
        player2Stats[5].text = "Arc: " + player2Info.baseArcane.ToString();
        player2Stats[6].text = "Def: " + player2Info.baseDefense.ToString() + (player2Info.equippedArmor != null ?
                       " + (" + player2Info.equippedArmor.defense + ")" : " + (0)");
        player2Stats[7].text = "Spc Def: " + player2Info.specialDefense.ToString();
        if (player2Info.equippedWeapon != null)
        {
            player2Stats[8].text = "Weapon: " + player2Info.equippedWeapon.itemName;
            player2Weapon.sprite = player2Info.equippedWeapon.itemIcon;

        }
        else
        {
            player2Stats[8].text = "Weapon: N/A";
        }

        if (player2Info.equippedArmor != null)
        {
            player2Stats[9].text = "Armor: " + player2Info.equippedArmor.itemName;
            player2Armor.sprite = player2Info.equippedArmor.itemIcon;
        }
        else
            player2Stats[9].text = "Armor: N/A";
    }


    // Update is called once per frame
    void Update()
    {
        /*Inventory system is outside the scope of my MVP and will be worked on at a later time*/
        //if (!pauseGame.isPaused)
        //{
        //    if (Input.GetKeyDown(KeyCode.Tab) && SceneManager.GetActiveScene().buildIndex != 2)
        //    {
        //        UpdateStats();
        //        OnInvOpened?.Invoke();

        //        statsMenu.SetActive(!statsMenu.activeSelf);
        //        if (!statsMenu.activeSelf)
        //        {
        //            Time.timeScale = 1;
        //            menuScreenActive = false;
        //        }
        //        else
        //        {
        //            menuScreenActive = true;
        //            Time.timeScale = 0;
        //        }
        //    }
        //}
    }

    public void OnResume()
    {
        statsMenu?.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnSave()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            SaveSystem.SavePlayer(playerInfo);
            SaveSystem.SavePlayer(player2Info);
            SaveSystem.SaveSettings(pauseGame.volumeSlider, pauseGame.sfxSlider, pauseGame.isFullscreen, pauseGame.graphicsLevel);
        }
        else
        {
            pauseGame.popupAnimator.SetTrigger("in");
        }
    }

    public void OnLoad()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            LoadPlayer(playerInfo);
            LoadPlayer(player2Info);
            LoadSettings(pauseGame.volumeSlider, pauseGame.sfxSlider, pauseGame.isFullscreen, pauseGame.graphicsLevel);
        }
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            SceneManager.LoadScene(1);
            player2Info.gameObject.transform.localScale = new Vector3(2, 2, 2);
            LoadPlayer(playerInfo);
            LoadPlayer(player2Info);
            LoadSettings(pauseGame.volumeSlider, pauseGame.sfxSlider, pauseGame.isFullscreen, pauseGame.graphicsLevel);
        }
    }

    public void OnQuit()
    {
        Application.Quit();
    }
    private void LoadSettings(Slider volumeSlider, Slider sfxSlider, Toggle isFullscreen, TMP_Dropdown graphicsLevel)
    {
        SystemSettings data = SaveSystem.LoadSettings();

        volumeSlider.value = data.volume;
        sfxSlider.value = data.sfx;
        isFullscreen.enabled = data.fullscreen;
        graphicsLevel.value = data.graphics;
    }

    private void LoadPlayer(PlayableCharacter player)
    {
        PlayerData data = SaveSystem.LoadPlayer();

        player.unitName = data.unitName;
        player.unitLevel = data.unitLevel;
        player.maxHP = data.maxHP;
        player.currentHP = data.currentHP;
        player.baseAttack = data.baseAttack;
        player.baseDefense = data.baseDefense;
        player.specialDefense = data.specialDefense;
        player.experience = data.experience;
        player.money = data.money;
        player.expToNextLevel = data.expToNextLevel;

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
        Vector3 beforeBattlePos = player1.GetComponent<PlayerCollision>().beforeBattlePos;

        if (level == 1)
        {
            player1.transform.position = beforeBattlePos;
            player2.transform.position = beforeBattlePos;

            player1.GetComponent<PlayerMovement>().enabled = true;
            player2.GetComponent<Follow>().enabled = true;
            uiCanvas.SetActive(true);


            battleHud.SetHUD(player1.GetComponent<Unit>(), player2.GetComponent<Unit>());
        }

        if(level == 2) {
            uiCanvas.SetActive(false);
        }
    }

    private void OnDisable()
    {
        playerInfo.OnStatsChanged -= UpdateStats;
    }
}
