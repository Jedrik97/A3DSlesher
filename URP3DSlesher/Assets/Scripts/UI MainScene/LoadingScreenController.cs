using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Root panel of the loading screen")]
    public GameObject panel;
    [Tooltip("Progress bar slider (0..1)")]
    public Slider progressBar;
    [Tooltip("Hint text UI (optional)")]
    public Text hintText;

    [Header("Hints")]
    [TextArea(1, 3)]
    public string[] hints = {
        "Совет: Используй уклонения, чтобы избежать критического удара!",
        "Совет: После босса не забудь прокачать навыки у NPC.",
        "Совет: Ловкость увеличивает скорость атаки и шанс крита!"
    };

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        if (panel != null) panel.SetActive(true);
        if (progressBar != null) progressBar.value = 0f;
        if (hintText != null && hints != null && hints.Length > 0)
            hintText.text = hints[Random.Range(0, hints.Length)];
    }

    public void SetProgress(float value)
    {
        if (progressBar != null)
            progressBar.value = Mathf.Clamp01(value);
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    public IEnumerator ShowForSeconds(float seconds)
    {
        Show();
        yield return new WaitForSeconds(seconds);
        Hide();
    }
}