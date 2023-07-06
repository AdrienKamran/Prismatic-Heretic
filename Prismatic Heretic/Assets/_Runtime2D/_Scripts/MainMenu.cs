using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public Animator transition;

    public static bool special = false;
    public static int previousLevel = 0;
    public static int currentLevel = 0;
    public static int previousCheckpoint=4;
    public static int first = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            previousLevel = 0;
            StartCoroutine(ResumeLevel(previousCheckpoint));
        }
    }

    public void PlayGame()
    {
        special = false;
        StartCoroutine(LoadLevel(4));
    }

    public IEnumerator LoadLevel(int levelIndex)
    {
       
        transition.SetTrigger("Start");
        Debug.Log(levelIndex);
        yield return new WaitForSeconds(1);

        if (levelIndex != 0)
        {
            
            if (first == 0)
            {
                SelectSword.hasBlue = false;
                SelectSword.hasRed = false;
                SelectSword.hasYellow = false;
                SceneManager.LoadScene(levelIndex, LoadSceneMode.Single);
                SceneManager.LoadScene(1, LoadSceneMode.Additive);
                
                previousLevel = currentLevel;
                currentLevel = levelIndex;
            }
            else
            {
                Debug.Log("Changing level");
                SceneManager.UnloadSceneAsync(currentLevel);
                SceneManager.LoadScene(levelIndex, LoadSceneMode.Additive);
                previousLevel = currentLevel;
                previousCheckpoint = levelIndex;
                currentLevel = levelIndex;
            }
            
        }
        else {

            first = 0;
            Debug.Log("called");
            previousLevel = 0;
            SceneManager.UnloadSceneAsync(currentLevel);
            SceneManager.LoadScene(0,LoadSceneMode.Single);
            
            
            
        }
       
    }


    public IEnumerator ResumeLevel(int levelIndex)
    {

        transition.SetTrigger("Start");
        Debug.Log(levelIndex);
        yield return new WaitForSeconds(1);

        if (levelIndex != 0)
        {
                SceneManager.LoadScene(levelIndex, LoadSceneMode.Single);
                SceneManager.LoadScene(1, LoadSceneMode.Additive);

                previousLevel = 0;
                currentLevel = levelIndex;
        }
    }

    public void LoadLevelDebug(int levelIndex)
    {
        Debug.Log("[DEBUG] Switching to" + levelIndex.ToString());
        StartCoroutine(LoadLevel(levelIndex));
    }

    public void GoBackToMain()
    {
        WarpSword.isSwinging = false;
        AttackAOE.isSwinging = false;
        first = 0;
        previousLevel = 0;
        StartCoroutine(LoadLevel(0));
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}

