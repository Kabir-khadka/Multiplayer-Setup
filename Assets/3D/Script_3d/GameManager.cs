using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject CompleteMenuUI;
    [SerializeField] private Button restartButton;
    [SerializeField]  private static bool GameIsPaused = false;

    [SerializeField] GameObject PauseMenuUI;

    // Called to display the completion text
    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(GameIsPaused){
                Resume();
            }else{
                Pause();
            }
        }
    }

    public void Resume(){
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        GameIsPaused = false;
    }

    public void Pause(){
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Complete()
    {
        if (CompleteMenuUI != null)
        {
            CompleteMenuUI.gameObject.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
          
        }
        else
        {
            Debug.LogWarning("Complete Text UI element is not assigned.");
        }

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if(Time.timeScale == 0f){
             Time.timeScale = 1.0f;
             GameIsPaused = false;
        }
    }

    public void StartGame(){
        SceneManager.LoadScene(1);
    }

    public void QuitGame(){
        Debug.Log("Quit game");
        Application.Quit();
    }

    public void MainMenu(){
        SceneManager.LoadScene(0);
    }
}
