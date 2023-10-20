using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject statsMenu;
    public PlayableCharacter playerInfo;
    public PlayableCharacter player2Info;
    public static event Action OnInvOpened;

    public List<TextMeshProUGUI> player1Stats;
    public List<TextMeshProUGUI> player2Stats;
    public Image player1Weapon;
    public Image player2Weapon;
    public Image player1Armor;
    public Image player2Armor;
    private PauseGame pauseGame;
    [HideInInspector]
    public bool menuScreenActive;
    public Inventory invManager;
    private void Start()
    {
        pauseGame = gameObject.GetComponent<PauseGame>();
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
        player1Stats[8].text = "Money: " + playerInfo.money.ToString();
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
        if (!pauseGame.isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Tab) && SceneManager.GetActiveScene().buildIndex != 1)
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
    }

    public void OnResume()
    {
        statsMenu?.SetActive(false);
        Time.timeScale = 1;
    }

    // Use this to save game and quit out of application
    public void OnApplicationQuit()
    {

    }

    public void OnMenu()
    {
        statsMenu?.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public void OnSave()
    {
        Debug.Log("Saving Game... Please bait");
    }

    public void OnLoad()
    {
        Debug.Log("Loading Game... Please vait beta");
    }


    public void OnLevelWasLoaded(int level)
    {
        Debug.Log("LEVEL LOADED");
    }

    private void OnDisable()
    {
        playerInfo.OnStatsChanged -= UpdateStats;
    }
}
