using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;

    [Header("References")]
    [SerializeField] private GameManager gameManager; // компонент GameManager, висит в сцене

    public override void InstallBindings()
    {
        // === Inventory как сервис ===
        Container.Bind<PlayerInventory>().AsSingle();

        // GameManager как singleton (берём из сцены)
        Container.Bind<GameManager>()
            .FromInstance(gameManager)
            .AsSingle();

        // Player создаётся из префаба
        Container.Bind<PlayerStats>()
            .FromComponentInNewPrefab(playerPrefab)
            .AsSingle()
            .NonLazy(); // сразу создаём
    }
}