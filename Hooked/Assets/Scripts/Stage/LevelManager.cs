using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public Vector3 startPosition;
    public PlayerController player;
    public TMP_Text startText;
    public TMP_Text respawnText;

    public GameObject pauseMenu;
    public GameObject pauseButton;

    private bool pausedGame = false;


    private void Start()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        player.spawned = false;
        yield return new WaitForSeconds(1f);
        player.level = this;
        startText.text = "3";
        yield return new WaitForSeconds(1f);
        startText.text = "2";
        yield return new WaitForSeconds(1f);
        startText.text = "1";
        yield return new WaitForSeconds(1f);
        startText.text = "Start";
        player.ResumeCharacter();
        pauseButton.SetActive(true);
        yield break;
    }

    public IEnumerator Respawn()
    {
        player.spawned = false;
        respawnText.text = "3";
        yield return new WaitForSeconds(1f);
        respawnText.text = "2";
        yield return new WaitForSeconds(1f);
        respawnText.text = "1";
        yield return new WaitForSeconds(1f);
        respawnText.text = "";
        player.ResumeCharacter();
        
        yield break;
    }

    public void PauseGame()
    {
        if (pausedGame == false)
        {
            pausedGame = true;
            respawnText.text = "";
            StopAllCoroutines();
            player.PauseCharacter();
            pauseMenu.SetActive(true);
            pauseButton.SetActive(false);
        }
        else
        {
            pausedGame = false;
            //player.ResumeCharacter();
            StartCoroutine(Respawn());
            pauseMenu.SetActive(false);
            pauseButton.SetActive(true);
        }
    }

    public void RestartGame()
    {
        player.OverrideHardData();
        StartGame();
    }

    public void ExitLevel()
    {
        SceneManager.LoadScene("Menu");
    }
}
