using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Reward : MonoBehaviour
{
    [SerializeField]
    public float logReward, foodReward, waterReward, mineReward, campfireReward, recoverReward, benchReward, toolReward, rocketReward, launchReward;
    private TMP_InputField logNum, foodNum, waterNum, mineNum, campfireNum, recoverNum, benchNum, toolNum, rocketNum, launchNum;
    public GameObject startMenu, rewardMenu;

    private void Awake()
    {
        logNum = transform.Find("Log/LogReward").GetComponent<TMP_InputField>();
        foodNum = transform.Find("Food/FoodReward").GetComponent<TMP_InputField>();
        waterNum = transform.Find("Water/WaterReward").GetComponent<TMP_InputField>();
        mineNum = transform.Find("Mine/MineReward").GetComponent<TMP_InputField>();
        campfireNum = transform.Find("Campfire/CampfireReward").GetComponent<TMP_InputField>();
        recoverNum = transform.Find("Recover/RecoverReward").GetComponent<TMP_InputField>();
        benchNum = transform.Find("Bench/BenchReward").GetComponent<TMP_InputField>();
        toolNum = transform.Find("Tool/ToolReward").GetComponent<TMP_InputField>();
        rocketNum = transform.Find("Rocket/RocketReward").GetComponent<TMP_InputField>();
        launchNum = transform.Find("Launch/LaunchReward").GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Start()
    {
        Show();
    }

    public void Show()
    {
        logNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        foodNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        waterNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        mineNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        campfireNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        recoverNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        benchNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        toolNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        rocketNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        launchNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
    }

    private char ValidateChar(string validCharacters, char addedChar)
    {
        if (validCharacters.IndexOf(addedChar) != -1)
        {
            // Valid
            return addedChar;
        }
        else
        {
            return '\0';
        }

    }

    public void Confirm()
    {
        logReward = float.Parse(logNum.text);
        foodReward = float.Parse(foodNum.text);
        waterReward = float.Parse(waterNum.text);
        mineReward = float.Parse(mineNum.text);
        campfireReward = float.Parse(campfireNum.text);
        recoverReward = float.Parse(recoverNum.text);
        benchReward = float.Parse(benchNum.text);
        toolReward = float.Parse(toolNum.text);
        rocketReward = float.Parse(rocketNum.text);
        launchReward = float.Parse(launchNum.text);

        rewardMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void Cancel()
    {
        logNum.text = logReward.ToString();
        foodNum.text = foodReward.ToString();
        waterNum.text = waterReward.ToString();
        mineNum.text = mineReward.ToString();
        campfireNum.text = campfireReward.ToString();
        recoverNum.text = recoverReward.ToString();
        benchNum.text = benchReward.ToString();
        toolNum.text = toolReward.ToString();
        rocketNum.text = rocketReward.ToString();
        launchNum.text = launchReward.ToString();

        rewardMenu.SetActive(false);
        startMenu.SetActive(true);
    }
}
