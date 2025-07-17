using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    private bool isPaused = false;

    public GameObject pauseMenuUI;

    private void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }
    public void restartGame() 
    {
        string previousScene = PlayerPrefs.GetString("PreviousScene", "");
        if (!string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("No previous scene found!");
        }
    }

    public void pauseGame()
    {
        Debug.Log("Pause clicked");

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f; //pause
        isPaused = true;
    }

    public void resumeGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f; //resume
        isPaused = false;
    }

    // Optional: pause by using ESC
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                pauseGame();
            else
                resumeGame();
        }
    }

    public void loadLevel1Scece() 
    {
        SceneManager.LoadScene(3);
    }
    public void loadLevel2Scece()
    {
        SceneManager.LoadScene(4);
    }

    public void loadMenu()
    {
        Debug.Log("loadMenu clicked!");
        SceneManager.LoadScene(0);
    }

    public void loadIntruction() 
    {
        SceneManager.LoadScene(5);
    }
}
