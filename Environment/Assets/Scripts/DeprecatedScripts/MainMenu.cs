using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Scene");
        GameObject.Find("Robot").GetComponent<CharacterMoveScript>().EndEpisode();
    }
}

