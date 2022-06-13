using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject options;
    public GameObject credits;
    public void StartTestLevel()
    {
        SceneManager.LoadScene("Level1");
    }

    public void StartEndlessLevel()
    {
        SceneManager.LoadScene("Endless");
    }

    public void ShowOptions()
    {
        options.SetActive(true);
        credits.SetActive(false);
        mainMenu.SetActive(false);
    }

    public void ShowMenu()
    {
        options.SetActive(false);
        credits.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ShowCredits()
    {
        options.SetActive(false);
        credits.SetActive(true);
        mainMenu.SetActive(false);
    }

}
