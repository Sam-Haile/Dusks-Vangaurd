using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public HudInfo[] hudInfos = new HudInfo[4]; // Max 4 players in a battle

    public TextMeshProUGUI gold;

    private Unit guts;

    private void Awake()
    {
        guts = FindObjectOfType<Player>();
    }

    public void UpdateHUDs()
    {
        for (int i = 0; i < hudInfos.Length; i++)
        {
            if (PartyManager.instance.partyMembers[i] != null)
                hudInfos[i].UpdateBattleStats(PartyManager.instance.partyMembers[i]);
            else
                hudInfos[i].gameObject.SetActive(false);
        }
    }

    public void SetHUD()
    {
        string amount = guts.gold.ToString();
        gold.text = amount.ToString();
    }

    public void UpdateAllStats(Unit unit)
    {

        if(unit.tag == "Player")
            SetHUD();
        
        UpdateHUDs();
        
    }


    //private void OnLevelWasLoaded(int level)
    //{
    //    if (level == 1)
    //    {
    //        UpdateAllStats(guts);
    //        UpdateAllStats(puck);
    //    }
    //}

    //IEnumerator UpdateStats()
    //{
    //    yield return new WaitForSeconds(3f);
    //    UpdateAllStats(guts);
    //}

}
