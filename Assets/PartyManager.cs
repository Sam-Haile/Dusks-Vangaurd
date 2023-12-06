using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager instance;

    // This script is used to keep track of party members
    public List<PlayableCharacter> partyMembers;

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
        Debug.Log("Added " +  player.name);
    }

    // Method to remove a player from the party
    public void RemovePlayerFromParty(PlayableCharacter player)
    {
        partyMembers.RemoveAll(member => member == player);
        player.uiHUD.SetActive(false);
        Debug.Log($"{player} has left the party.");
    }
}
