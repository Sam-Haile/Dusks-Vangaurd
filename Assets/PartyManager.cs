using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyManager : MonoBehaviour
{
    public static PartyManager instance;

    // This script is used to keep track of party members
    public List<PlayableCharacter> partyMembers;

    public HudInfo[] partyMenuStats;

    public HudInfo[] equipMenuStats;

    public Image[] playerIcons;

    int index = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddPlayerToList(PlayableCharacter player)
    {
        partyMembers.Add(player);
        UpdatePartyStats();
    }

    // Method to remove a player from the party
    public void RemovePlayerFromParty(PlayableCharacter player)
    {
        partyMembers.RemoveAll(member => member == player);
        player.uiHUD.SetActive(false);
        Debug.Log($"{player} has left the party.");
    }

    /// <summary>
    /// Activates party menu based on number of
    /// active party members
    /// </summary>
    public void UpdatePartyStats()
    {
        for(int i = 0; i < partyMembers.Count; i++)
        {
            partyMenuStats[i].UpdatePartyMenu(partyMembers[i]);
            partyMenuStats[i].gameObject.SetActive(true);
        }
    }

    public void UpdateEquipStats()
    {
        for (int i = 0; i < partyMembers.Count; i++)
        {
            equipMenuStats[i].UpdateInventory(partyMembers[i]);
        }
        
        equipMenuStats[0].gameObject.SetActive(true);
    }

    public void DisplayPlayerEquipStats(int playerIndex)
    {
        for (int i = 0; i < partyMembers.Count; i++)
        {
            if(i == playerIndex)
            {
                equipMenuStats[i].gameObject.SetActive(true);
            }
            else
                equipMenuStats[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the small icons at the top to reflect the current party
    /// </summary>
    public void ModifyEquipMenuIcons()
    {
        for (int i = 0; i < partyMembers.Count; i++)
        {
            playerIcons[i].gameObject.SetActive(true);
            playerIcons[i].sprite = partyMembers[i].playerSprite;
        }
    }

    /// <summary>
    /// This method uses the arrows to display different player stats
    /// </summary>
    public void CycleThroughPlayers(string arrow)
    {

        if(arrow == "left")
        {
            if(index == 0)
            {
                index = partyMembers.Count - 1;
            }
            else
                index--;

        }
        else if(arrow == "right")
        {
            if (index == partyMembers.Count - 1)
                index = 0;
            else
                index++;

        }
            
        DisplayPlayerEquipStats(index);

    }
}
