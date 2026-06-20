using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject gameOverUI;
    public GameObject gameClearUI;
    public GameObject howToUseUI;
    public KeyCode pauseKey = KeyCode.P;
    private bool isPaused = false;

    private const string FirstPlayKey = "FirstPlayDone";

    void Start()
    {
        // 初回プレイならHow to Use UIを表示
        if (!PlayerPrefs.HasKey(FirstPlayKey))
        {
            ShowHowToUseUI();
            PlayerPrefs.SetInt(FirstPlayKey, 1);
        }
        else
        {
            LockCursor();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
            TogglePause();
    }

    public void TogglePause()
    {
        // ゲームオーバーまたはゲームクリア中はポーズできない
        if (gameOverUI.activeSelf || gameClearUI.activeSelf) return;

        if (isPaused)
            HidePauseUI();
        else
            ShowPauseUI();
    }

    public void ShowPauseUI()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        UnlockCursor();
    }

    public void HidePauseUI()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        LockCursor();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ShowGameOverUI()
    {
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
        UnlockCursor();
    }

    public void ShowGameClearUI()
    {
        Time.timeScale = 0f;
        gameClearUI.SetActive(true);
        UnlockCursor();
    }

    public void ShowHowToUseUI()
    {
        howToUseUI.SetActive(true);
        Time.timeScale = 0f;
        UnlockCursor();
    }

    public void HideHowToUseUI()
    {
        howToUseUI.SetActive(false);
        Time.timeScale = 1f;
        LockCursor();
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
