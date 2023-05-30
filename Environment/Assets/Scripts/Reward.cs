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
    // Start is called before the first frame update
    void Start()
    {
        logReward = 0.0003f;
        foodReward = 0.0002f;
        waterReward = 0.0001f;
        mineReward = 0.0005f;
        campfireReward = 0.75f;
        recoverReward = 0.05f;
        benchReward = 0.75f;
        toolReward = 0.75f;
        rocketReward = 1f;
        launchReward = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        Show();
    }

    private void Awake()
    {
        logNum = transform.Find("Menu/RewardMenu/Log/LogReward").GetComponent<TMP_InputField>();
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

        GameObject.Find("Robot").GetComponent<CharacterMoveScript>().RewardUpdate();

        Debug.Log("Log: " + logReward);
        Debug.Log("Apple: " + foodReward);
        Debug.Log("meat: " + waterReward);
        Debug.Log("Oil: " + mineReward);
        Debug.Log("Water: " + campfireReward);
        Debug.Log("Iron: " + recoverReward);
        Debug.Log("Gold: " + benchReward);
        Debug.Log("Dimond: " + toolReward);
        Debug.Log("Gold: " + rocketReward);
        Debug.Log("Dimond: " + launchReward);

        rewardMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void Cancel()
    {
        Debug.Log("Log: " + logReward);
        Debug.Log("Apple: " + foodReward);
        Debug.Log("meat: " + waterReward);
        Debug.Log("Oil: " + mineReward);
        Debug.Log("Water: " + campfireReward);
        Debug.Log("Iron: " + recoverReward);
        Debug.Log("Gold: " + benchReward);
        Debug.Log("Dimond: " + toolReward);
        Debug.Log("Gold: " + rocketReward);
        Debug.Log("Dimond: " + launchReward);

        rewardMenu.SetActive(false);
        startMenu.SetActive(true);
    }
}
