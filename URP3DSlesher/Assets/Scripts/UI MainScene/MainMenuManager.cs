using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject loadingPanel;
    public GameObject loadPanel;
    public GameObject scorePanel;
    public GameObject creatorsPanel;

    [Header("Loading UI")]
    public Slider progressBar;
    public Text hintText;

    private string[] hints = {
        "Совет: Используй уклонения, чтобы избежать критического удара!",
        "Совет: После босса не забудь прокачать навыки у NPC.",
        "Совет: Ловкость увеличивает скорость атаки и шанс крита!"
    };

    // Вызывается из MenuInputController
    public void StartNewGame()
    {
        mainMenuPanel.SetActive(false);
        loadingPanel.SetActive(true);

        hintText.text = hints[Random.Range(0, hints.Length)];
        StartCoroutine(LoadGameSceneAsync("MainGameScene"));
    }

    public void LoadGame()
    {
        mainMenuPanel.SetActive(false);
        loadPanel.SetActive(true);
    }

    public void OpenTraining()
    {
        SceneManager.LoadScene("TrainingGameScene");
    }

    public void OpenScore()
    {
        mainMenuPanel.SetActive(false);
        scorePanel.SetActive(true);
    }

    public void OpenCreators()
    {
        mainMenuPanel.SetActive(false);
        creatorsPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadGameSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float timer = 0f;

        while (!operation.isDone)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;

            if (timer >= 5f && progress >= 1f)
                operation.allowSceneActivation = true;

            yield return null;
        }
    }
}
