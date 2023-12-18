using TMPro;
using UnityEngine;

public class BattleHUD : MonoBehaviour
{
    public HudInfo[] hudInfos = new HudInfo[4]; // Max 4 players in a battle


    private Unit guts;

    private void Awake()
    {
        guts = FindObjectOfType<Player>();
    }
    private void Start()
    {
        Debug.Log("Updating Try");
        UpdateHUDs();
    }

    public void UpdateHUDs()
    {
        for (int i = 0; i < PartyManager.instance.partyMembers.Count; i++)
        {
                hudInfos[i].UpdateBattleStats(PartyManager.instance.partyMembers[i]);
                hudInfos[i].gameObject.SetActive(true);
        }
    }

    public void UpdateAllStats(Unit unit)
    {

        //if(unit.tag == "Player")
        //    SetHUD();
        
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
