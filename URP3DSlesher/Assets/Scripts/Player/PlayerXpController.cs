using UnityEngine;
using Zenject;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerXpController : MonoBehaviour
{
    [Header("XP")]
    [SerializeField] private float currentExp;
    [SerializeField] private float expToNextLevel = 100f;

    [Header("Auto Gains Per Level")]
    [SerializeField] private int autoStrengthPerLevel = 1;
    [SerializeField] private int autoAgilityPerLevel = 1;
    [SerializeField] private int autoVitalityPerLevel = 1;
    [SerializeField] private int statPointsPerLevel = 2;

    [Header("UI")]
    [SerializeField] private Image expFill;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI levelText;

    private PlayerStats _playerStats;
    private SignalBus _signalBus;
    private int _level = 1;

    public event Action<int> OnLevelChanged;
    public event Action<float, float> OnExpChanged;

    [Inject]
    public void Construct(PlayerStats playerStats, SignalBus signalBus)
    {
        _playerStats = playerStats;
        _signalBus = signalBus;
    }

    private void OnEnable()
    {
        _signalBus.Subscribe<EnemyDiedSignal>(OnEnemyDied);
        OnLevelChanged += UpdateLevelUI;
        OnExpChanged += UpdateExpUI;
        OnLevelChanged?.Invoke(_level);
        OnExpChanged?.Invoke(currentExp, expToNextLevel);
    }

    private void OnDisable()
    {
        _signalBus.TryUnsubscribe<EnemyDiedSignal>(OnEnemyDied);
        OnLevelChanged -= UpdateLevelUI;
        OnExpChanged -= UpdateExpUI;
    }

    public void GrantExperience(float amount)
    {
        currentExp += amount;

        while (currentExp >= expToNextLevel)
            LevelUp();

        OnExpChanged?.Invoke(currentExp, expToNextLevel);
    }

    private void LevelUp()
    {
        currentExp -= expToNextLevel;
        _level++;

        if (_level < 5)
            expToNextLevel = 100 + _level * 75f;
        else
            expToNextLevel = 500f * Mathf.Pow(1.3f, _level - 5);

        _playerStats.strength += autoStrengthPerLevel;
        _playerStats.agility += autoAgilityPerLevel;
        _playerStats.vitality += autoVitalityPerLevel;
        _playerStats.availableStatPoints += statPointsPerLevel;
        _playerStats.SetLevel(_level);

        OnLevelChanged?.Invoke(_level);
        OnExpChanged?.Invoke(currentExp, expToNextLevel);
    }

    private void OnEnemyDied(EnemyDiedSignal s)
    {
        GrantExperience(s.Xp);
    }

    private void UpdateLevelUI(int level)
    {
        if (levelText) levelText.text = $"LV. {level}";
    }

    private void UpdateExpUI(float current, float toNext)
    {
        if (expFill) expFill.fillAmount = toNext > 0f ? current / toNext : 0f;
        if (expText) expText.text = $"{Mathf.FloorToInt(current)} / {Mathf.FloorToInt(toNext)} EXP";
    }

    public int GetLevel() => _level;
    public float GetCurrentExp() => currentExp;
    public float GetExpToNextLevel() => expToNextLevel;

    public void SetHud(Image fill, TextMeshProUGUI exp, TextMeshProUGUI level)
    {
        expFill = fill;
        expText = exp;
        levelText = level;
        OnLevelChanged?.Invoke(_level);
        OnExpChanged?.Invoke(currentExp, expToNextLevel);
    }
}
