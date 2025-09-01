using UnityEngine;
using UnityEngine.Events;

public class MenuInputController : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onStartClicked;
    public UnityEvent onLoadClicked;
    public UnityEvent onTrainingClicked;
    public UnityEvent onScoreClicked;
    public UnityEvent onCreatorsClicked;
    public UnityEvent onExitClicked;
    
    public void StartButtonPressed() => onStartClicked?.Invoke();
    public void LoadButtonPressed() => onLoadClicked?.Invoke();
    public void TrainingButtonPressed() => onTrainingClicked?.Invoke();
    public void ScoreButtonPressed() => onScoreClicked?.Invoke();
    public void CreatorsButtonPressed() => onCreatorsClicked?.Invoke();
    public void ExitButtonPressed() => onExitClicked?.Invoke();
}