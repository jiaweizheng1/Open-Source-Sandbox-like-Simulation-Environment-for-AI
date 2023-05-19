using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Envrionment_UI : MonoBehaviour
{
    private TMP_InputField LogNum, AppleNum, MeatNum, OilNum, WaterNum, IronNum, GoldNum, DimondNum;
    public TMP_Text log_t, apple_t, meat_t, oil_t, water_t, iron_t, gold_t, diamond_t;
    public GameObject startMenu;
    public GameObject optionMenu;
    public Slider HealthSlider, HungerSlider, ThirstSlider;
    private float Health, Hunger, Thirst;
    // Start is called before the first frame update
    private void Awake()
    {
        LogNum = transform.Find("Log").transform.Find("LogNum").GetComponent<TMP_InputField>();
        AppleNum = transform.Find("Apple").transform.Find("AppleNum").GetComponent<TMP_InputField>();
        MeatNum = transform.Find("Meat").transform.Find("MeatNum").GetComponent<TMP_InputField>();
        OilNum = transform.Find("Oil").transform.Find("OilNum").GetComponent<TMP_InputField>();
        WaterNum = transform.Find("Water").transform.Find("WaterNum").GetComponent<TMP_InputField>();
        IronNum = transform.Find("Iron").transform.Find("IronNum").GetComponent<TMP_InputField>();
        GoldNum = transform.Find("Gold").transform.Find("GoldNum").GetComponent<TMP_InputField>();
        DimondNum = transform.Find("Dimond").transform.Find("DimondNum").GetComponent<TMP_InputField>();

        LogNum.text = log_t.text;
        AppleNum.text = apple_t.text;
        MeatNum.text = meat_t.text;
        OilNum.text = oil_t.text;
        WaterNum.text = water_t.text;
        IronNum.text = iron_t.text;
        GoldNum.text = gold_t.text;
        DimondNum.text = diamond_t.text;
    }

    // Update is called once per frame
    private void Update()
    {
        Show();
    }

    public void Show()
    {
        LogNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        AppleNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        MeatNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        OilNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        WaterNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        IronNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        GoldNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
        DimondNum.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar("0123456789", addedChar);
        };
    }
    
    public void SliderUpdate()
    {
        Health = HealthSlider.value;
        Hunger = HungerSlider.value;
        Thirst = ThirstSlider.value;
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

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        optionMenu.SetActive(false);
        /*SceneManager.LoadScene("MainMenu");*/
        startMenu.SetActive(true);
        GameObject.Find("Robot").GetComponent<CharacterMoveScript>().EndEpisode();
    }

    public void Confirm()
    {
        log_t.text = "x" + LogNum.text;
        apple_t.text = "x" + AppleNum.text;
        meat_t.text = "x" + MeatNum.text;
        oil_t.text = "x" + OilNum.text;
        water_t.text = "x" + WaterNum.text;
        iron_t.text = "x" + IronNum.text;
        gold_t.text = "x" + GoldNum.text;
        diamond_t.text = "x" + DimondNum.text;

        optionMenu.SetActive(false);
        startMenu.SetActive(true);

        HealthSlider.value = Health;
        HungerSlider.value = Hunger;
        ThirstSlider.value = Thirst;
    }

    public void Cancel()
    {
        optionMenu.SetActive(false);
        startMenu.SetActive(true);
    }
}
