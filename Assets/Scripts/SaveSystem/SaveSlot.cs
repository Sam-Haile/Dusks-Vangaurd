using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public TextMeshProUGUI currentChapter;
    public Image[] partyIcons;
    public TextMeshProUGUI goldAmount;
    public TextMeshProUGUI timePlayed;

    // Method to update the save slot details
    public void UpdateSaveSlot(string currentChapterName, int gold)
    {
        currentChapter.text = currentChapterName;

        for(int i = 0; i < PartyManager.instance.partyMembers.Count; i++)
        {
            partyIcons[i].color = new Color(255, 255, 255, 255); 
        }
        
        goldAmount.text = gold.ToString();
    }

}
