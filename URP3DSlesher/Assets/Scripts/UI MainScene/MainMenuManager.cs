using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject loadPanel;
    [SerializeField] GameObject scorePanel;
    [SerializeField] GameObject creatorsPanel;

    [Header("Loading")]
    [SerializeField] LoadingScreenController loadingController;

    [Header("Scene Names")]
    [SerializeField] string mainGameScene = "MainGameScene";
    [SerializeField] string trainingScene = "TrainingGameScene";

    [Header("Mobile")]
    [Tooltip("Блокировать нажатия, пока идёт загрузка")]
    [SerializeField] bool lockInputWhileLoading = true;

    float backPressTime;

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        BackToMain();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            HandleAndroidBack();
    }
    

    public void StartNewGame()
    {
        if (LockIfLoading()) return;

        HideAllPanels();
        if (loadingController) loadingController.ShowAndLoad(mainGameScene);
        else SceneManager.LoadScene(mainGameScene);
    }

    public void LoadGame()
    {
        if (LockIfLoading()) return;

        HideAllPanels();
        if (loadPanel) loadPanel.SetActive(true);
    }

    public void OpenTraining()
    {
        if (LockIfLoading()) return;

        if (!string.IsNullOrEmpty(trainingScene))
            SceneManager.LoadScene(trainingScene);
    }

    public void OpenScore()
    {
        if (LockIfLoading()) return;

        HideAllPanels();
        if (scorePanel) scorePanel.SetActive(true);
    }

    public void OpenCreators()
    {
        if (LockIfLoading()) return;

        HideAllPanels();
        if (creatorsPanel) creatorsPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToMain()
    {
        if (LockIfLoading()) return;

        HideAllPanels();
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }

    bool LockIfLoading()
    {
        if (!lockInputWhileLoading) return false;
        if (loadingController && loadingController.IsLoading()) return true;
        return false;
    }

    void HideAllPanels()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (loadPanel) loadPanel.SetActive(false);
        if (scorePanel) scorePanel.SetActive(false);
        if (creatorsPanel) creatorsPanel.SetActive(false);
    }

    void HandleAndroidBack()
    {
        if (mainMenuPanel && mainMenuPanel.activeSelf)
        {
            float t = Time.realtimeSinceStartup;
            if (t - backPressTime < 1.2f)
                ExitGame();
            else
                backPressTime = t;
            return;
        }
        
        if ((loadPanel && loadPanel.activeSelf) ||
            (scorePanel && scorePanel.activeSelf) ||
            (creatorsPanel && creatorsPanel.activeSelf))
        {
            BackToMain();
            return;
        }
        
        if (loadingController && loadingController.IsLoading())
            return;

        BackToMain();
    }
}
