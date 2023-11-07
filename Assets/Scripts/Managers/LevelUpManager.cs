using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    //EXP Screen Fields
    [HideInInspector] public int totalExp;
    public Slider p1XpSlider;
    public Slider p2XpSlider;
    public GameObject levelScreen;
    public TextMeshProUGUI moneyFromBattle;
    public TextMeshProUGUI currentMoney;
    [HideInInspector] public int totalMoney = 0;


    public bool expDone = false;
    public bool moneyDone = false;
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

    public int MoneyToGain(Unit enemiesMoney)
    {
        return enemiesMoney.money + (int)Random.Range(enemiesMoney.money * -.1f, enemiesMoney.money * .1f);
    }
    public void GainXPAndMoney(Unit selectedEnemy)
    {
        totalExp += ExpToGain(selectedEnemy);
        totalMoney += MoneyToGain(selectedEnemy);
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
        levelScreen.SetActive(true);
        yield return new WaitForSeconds(2f);

        float targetValue = (float)(player.experience + totalExp) / (float)player.expToNextLevel;

        float overflowExp;
        float elapsedTime = 0f;
        float duration = 2f; // Time (in seconds) you want the interpolation to take

        while (elapsedTime < duration)
        {
            float sliderValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);

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

    public IEnumerator GainMoney(PlayableCharacter player)
    {
        yield return new WaitForSeconds(2f);

        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            int moneyInterpolation = (int)Mathf.Lerp(totalMoney, 0f, elapsedTime / duration);
            moneyFromBattle.text = moneyInterpolation.ToString();

            int totalMoneyInterpolation = (int)Mathf.Lerp(player.money, player.money + totalMoney, elapsedTime / duration);
            currentMoney.text = totalMoneyInterpolation.ToString();

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the values reach the exact target values in case of rounding errors
        moneyFromBattle.text = "0";
        currentMoney.text = (player.money + totalMoney).ToString();

        // Update the player's money to the new total
        player.money += totalMoney;

        moneyDone = true;
    }

}
