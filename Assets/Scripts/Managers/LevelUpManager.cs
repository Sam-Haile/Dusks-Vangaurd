using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager instance;
    private Queue<PlayableCharacter> characterLvUpQueue = new Queue<PlayableCharacter>();
    //EXP Screen Fields
    [HideInInspector] public int totalExp;
    public GameObject[] xpHUDS = new GameObject[4]; // Max 4 players in a battle
    public Animator[] levelUpText = new Animator[4];
    public GameObject levelScreen;
    public TextMeshProUGUI goldFromBattle;
    public TextMeshProUGUI currentGold;
    public TextMeshProUGUI expFromBattle;
    [HideInInspector] public int totalGold = 0;

    public Button button;
    public LevelUpStats stats;
    public GameObject continueButton;
    [HideInInspector] public bool advance = false;

    public bool expDone = false;
    public bool goldDone = false;

    List<PlayableCharacter> partyManager;
    public LevelUpStats lvlUpStats;
    public PlayableCharacter currentPlayer;

    private void Awake()
    {
        // Ensure that there's only one instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        partyManager = PartyManager.instance.partyMembers;
    }

    private void Update()
    {
        if (expDone && goldDone)
        {
            expDone = false;
            goldDone = false;
            continueButton.SetActive(true);
        }
    }

    public int ExpToGain(Unit enemiesXP)
    {
        return enemiesXP.experience + (int)Random.Range(enemiesXP.experience * -.1f, enemiesXP.experience * .1f);
    }

    private void SetXP(float currentXp, Slider slider)
    {
        slider.value = currentXp;
    }

    public int GoldToGain(Unit enemiesGold)
    {
        return enemiesGold.gold + (int)Random.Range(enemiesGold.gold * -.1f, enemiesGold.gold * .1f);
    }
    public void GainXPAndGold(Unit selectedEnemy)
    {
        totalExp += ExpToGain(selectedEnemy);
        totalGold += GoldToGain(selectedEnemy);
    }

    private void UpdateStats(PlayableCharacter player)
    {
        //Set gold fields
        goldFromBattle.text = totalGold.ToString();

        currentGold.text = player.gold.ToString();

        //Set EXP fields
        expFromBattle.text = totalExp.ToString();

        levelScreen.SetActive(true);
    }

    /// <summary>
    /// Apply the XP to players after a battle ends in victory
    /// </summary>
    /// <returns></returns>
    public IEnumerator GainExp(PlayableCharacter player, int i, System.Action onComplete)
    {
        // Give totalExp a degree of variabliity
        int variableExp = (int)(totalExp * Random.Range(.8f, 1.2f));

        float startValue = (float)player.experience / (float)player.expToNextLevel;
        xpHUDS[i].SetActive(true);
        XPScreen xpHUD = xpHUDS[i].GetComponent<XPScreen>();

        xpHUD.xpSlider.value = startValue;
        xpHUD.playerIcon.sprite = partyManager[i].playerSpriteHead;

        yield return new WaitForSeconds(2f);

        float targetValue = (float)(partyManager[i].experience + variableExp) / (float)partyManager[i].expToNextLevel;

        float overflowExp;
        float duration = 1.5f; // Time (in seconds) you want the interpolation to take
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float sliderValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            int expInterpolation = (int)Mathf.Lerp(variableExp, 0, elapsedTime / duration);

            if (xpHUD.xpSlider.value == 1)
            {
                characterLvUpQueue.Enqueue(partyManager[i]);
                levelUpText[i].SetTrigger("in");

                overflowExp = (targetValue * partyManager[i].expToNextLevel) - partyManager[i].expToNextLevel;

                // Reset the start and target values to correctly interpolate the overflow experience:
                startValue = 0;
                targetValue = overflowExp / partyManager[i].expToNextLevel;

                // Reset the elapsed time and duration to interpolate the overflow experience:
                elapsedTime = 0;
                duration = 2f; // Reset the interpolation duration

                sliderValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            }

            SetXP(sliderValue, xpHUD.xpSlider);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the slider value reaches the exact target value in case of rounding errors
        SetXP(targetValue, xpHUD.xpSlider);
        partyManager[i].AddExperience(variableExp);

        expDone = true;
        onComplete?.Invoke();

    }

    public IEnumerator GainGold(PlayableCharacter player)
    {
        UpdateStats(player);

        yield return new WaitForSeconds(2f);

        float duration = 1.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            int goldInterpolation = (int)Mathf.Lerp(totalGold, 0f, elapsedTime / duration);
            goldFromBattle.text = goldInterpolation.ToString();

            int totalGoldInterpolation = (int)Mathf.Lerp(player.gold, player.gold + totalGold, elapsedTime / duration);
            currentGold.text = totalGoldInterpolation.ToString();

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the values reach the exact target values in case of rounding errors
        goldFromBattle.text = "0";
        currentGold.text = (player.gold + totalGold).ToString();

        // Update the player's gold to the new total
        player.gold += totalGold;

        goldDone = true;
    }


    public IEnumerator LevelUp()
    {
        while (characterLvUpQueue.Count > 0)
        {
            PlayableCharacter player = (PlayableCharacter)characterLvUpQueue.Dequeue();

            //stats.UpdateStats(player);

            currentPlayer = player;

            stats.animator.SetTrigger("in");

            // Wait for the onClick event to hide the level-up screen
            // Attach the method to the button's onClick event
            button.onClick.AddListener(HideLevelUpScreen);

            // Wait for the onClick event to hide the level-up screen
            yield return new WaitUntil(() => advance);

        }
    }

    public void HideLevelUpScreen()
    {
        stats.animator.SetTrigger("out");
        // Set the flag to signal that the screen is hidden
        advance = true;

        // Remove the listener to avoid multiple calls
        button.onClick.RemoveListener(HideLevelUpScreen);
    }

}
