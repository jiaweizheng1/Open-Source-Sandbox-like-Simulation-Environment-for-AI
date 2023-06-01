using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject startMenu;
    public GameObject optionMenu, rewardMenu;
    public bool isPaused;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        startMenu.SetActive(true);
        optionMenu.SetActive(false);
        rewardMenu.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = FindObjectOfType<CharacterMoveScript>().timescale;
        isPaused = false;
    }

    public void BackToMenu()
    {
        GameObject.Find("Robot").GetComponent<CharacterMoveScript>().StopAllCor();
        GameObject.Find("Robot").GetComponent<CharacterMoveScript>().EndEpisode();
        pauseMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void startGame()
    {
        GameObject.Find("Robot").GetComponent<CharacterMoveScript>().StopAllCor();
        GameObject.Find("Robot").GetComponent<CharacterMoveScript>().EndEpisode();
        startMenu.SetActive(false);
        Time.timeScale = FindObjectOfType<CharacterMoveScript>().timescale;
        isPaused = false;
    }

    public void EnvrionmentEditing()
    {
        startMenu.SetActive(false);
        optionMenu.SetActive(true);
    }

    public void RewardEditing()
    {
        startMenu.SetActive(false);
        rewardMenu.SetActive(true);
    }

    public void BackToMain()
    {
        startMenu.SetActive(true);
        optionMenu.SetActive(false);
    }
}
