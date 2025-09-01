using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] GameObject loadingPanel;

    [Header("UI")]
    [SerializeField] Slider progressBar;
    [SerializeField] Text progressText;
    [SerializeField] Text hintText;

    [Header("Hints")]
    [TextArea(2, 5)]
    [SerializeField] string[] hints = {
        "Совет: Используй уклонения, чтобы избежать критического удара!",
        "Совет: После босса не забудь прокачать навыки у NPC.",
        "Совет: Ловкость увеличивает скорость атаки и шанс крита!"
    };
    [SerializeField] bool randomHintOnShow = true;

    [Header("Timing")]
    [Tooltip("Минимальное время показа экрана загрузки, чтобы избежать моргания")]
    [SerializeField] float minShowTime = 1.0f;

    AsyncOperation currentOp;
    bool isLoading;

    void Awake()
    {
        Hide();
    }

    public bool IsLoading() => isLoading;

    public void Show()
    {
        if (loadingPanel) loadingPanel.SetActive(true);
        if (progressBar) progressBar.value = 0f;

        if (hintText && randomHintOnShow && hints.Length > 0)
            hintText.text = hints[Random.Range(0, hints.Length)];

        if (progressText) progressText.text = "0%";
    }

    public void Hide()
    {
        if (loadingPanel) loadingPanel.SetActive(false);
    }

    public void SetHint(string text)
    {
        if (hintText) hintText.text = text;
    }

    public void ShowAndLoad(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[LoadingScreenController] Scene name is empty.");
            return;
        }

        if (isLoading)
            return;

        Show();
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        isLoading = true;

        currentOp = SceneManager.LoadSceneAsync(sceneName);
        if (currentOp == null)
        {
            isLoading = false;
            yield break;
        }

        currentOp.allowSceneActivation = false;

        float timer = 0f;
        while (!currentOp.isDone)
        {
            timer += Time.deltaTime;
            
            float progress = Mathf.Clamp01(currentOp.progress / 0.9f);

            if (progressBar)  progressBar.value = progress;
            if (progressText) progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

            if (progress >= 1f && timer >= minShowTime)
                currentOp.allowSceneActivation = true;

            yield return null;
        }

        isLoading = false;
        currentOp = null;
        
    }
}
