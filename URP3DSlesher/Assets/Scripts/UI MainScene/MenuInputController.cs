using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class MenuInputController : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onStartClicked;
    public UnityEvent onLoadClicked;
    public UnityEvent onTrainingClicked;
    public UnityEvent onScoreClicked;
    public UnityEvent onCreatorsClicked;
    public UnityEvent onExitClicked;
    public UnityEvent onBackClicked;

    [Header("Mobile Feedback")]
    [SerializeField] bool enableHaptics = true;
    [SerializeField] AudioSource clickSfx;
    
    public void StartButtonPressed()    => InvokeWithFeedback(onStartClicked);
    public void LoadButtonPressed()     => InvokeWithFeedback(onLoadClicked);
    public void TrainingButtonPressed() => InvokeWithFeedback(onTrainingClicked);
    public void ScoreButtonPressed()    => InvokeWithFeedback(onScoreClicked);
    public void CreatorsButtonPressed() => InvokeWithFeedback(onCreatorsClicked);
    public void ExitButtonPressed()     => InvokeWithFeedback(onExitClicked);
    public void BackButtonPressed()     => InvokeWithFeedback(onBackClicked);

    void InvokeWithFeedback(UnityEvent evt)
    {
        if (EventSystem.current && !EventSystem.current.IsPointerOverGameObject())
            return;

        if (clickSfx && clickSfx.enabled)
            clickSfx.Play();

        if (enableHaptics)
            Handheld.Vibrate();

        evt?.Invoke();
    }
}