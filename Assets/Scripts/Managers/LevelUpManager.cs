using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager instance;

    //EXP Screen Fields
    [HideInInspector] public int totalExp;
    public Slider p1XpSlider;
    public Slider p2XpSlider;
    public GameObject levelScreen;
    public TextMeshProUGUI goldFromBattle;
    public TextMeshProUGUI currentGold;
    public TextMeshProUGUI expFromBattle;
    [HideInInspector] public int totalGold = 0;

    public GameObject continueButton;

    public bool expDone = false;
    public bool goldDone = false;
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

    private void Update()
    {
        if (expDone && goldDone)
        {
            expDone = false;
            goldDone = false;
            continueButton.SetActive(true);
        }
    }

    public void LevelUp()
    {
        //Do level up stuff here
        Debug.Log("LEVEL UP TIME");
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
    public IEnumerator GainExp(PlayableCharacter player, Slider xpSlider, Text dialogueText)
    {
        //expFromBattle.text = totalExp.ToString();
        float startValue = (float)player.experience / (float)player.expToNextLevel;
        xpSlider.value = startValue;
        yield return new WaitForSeconds(2f);

        float targetValue = (float)(player.experience + totalExp) / (float)player.expToNextLevel;

        float overflowExp;
        float duration = 1.5f; // Time (in seconds) you want the interpolation to take
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float sliderValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            int expInterpolation = (int)Mathf.Lerp(totalExp, 0, elapsedTime/ duration);

            if (xpSlider.value == 1)
            {
                LevelUp();

                overflowExp = (targetValue * player.expToNextLevel) - player.expToNextLevel;

                // Reset the start and target values to correctly interpolate the overflow experience:
                startValue = 0;
                targetValue = overflowExp / player.expToNextLevel;

                // Reset the elapsed time and duration to interpolate the overflow experience:
                elapsedTime = 0;
                duration = 2f; // Reset the interpolation duration

                sliderValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            }

            SetXP(sliderValue, xpSlider);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the slider value reaches the exact target value in case of rounding errors
        SetXP(targetValue, xpSlider);
        player.AddExperience(totalExp, dialogueText.text);

        expDone = true;

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

}
