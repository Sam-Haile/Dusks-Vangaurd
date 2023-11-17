using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public TextMeshProUGUI currentChapter;
    public Image gutsIcon;
    public Image puckIcon;
    public TextMeshProUGUI goldAmount;
    public TextMeshProUGUI timePlayed;

    // Method to update the save slot details
    public void UpdateSaveSlot(string currentChapterName, bool gutsSprite, bool puckSprite, int gold)
    {
        currentChapter.text = currentChapterName;

        if(gutsSprite)
        {
            gutsIcon.color = new Color(255, 255, 255);
        }

        if (puckSprite)
        {
            puckIcon.color = new Color(255, 255, 255);
        }

        goldAmount.text = gold.ToString();
    }

}
