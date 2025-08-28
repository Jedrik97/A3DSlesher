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
        // GameManager как singleton
        Container.Bind<GameManager>().FromInstance(gameManager).AsSingle();

        // Inventory берём из GameManager
        Container.Bind<PlayerInventory>()
            .FromMethod(ctx => gameManager.Inventory)
            .AsSingle();

        // Player создаётся из префаба
        Container.Bind<PlayerStats>()
            .FromComponentInNewPrefab(playerPrefab)
            .AsSingle()
            .NonLazy(); // сразу создаём
    }
}