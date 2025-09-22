using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<EnemyDiedSignal>();

        Container.Bind<PlayerInventory>().AsSingle();

        // Bind interfaces too, so Zenject calls Initialize()
        Container.BindInterfacesAndSelfTo<GameManager>()
            .FromInstance(gameManager)
            .AsSingle();

        var playerGO = Container.InstantiatePrefab(playerPrefab);

        var playerStats = playerGO.GetComponent<PlayerStats>();
        var xpController = playerGO.GetComponent<PlayerXpController>();

        Container.Bind<PlayerStats>().FromInstance(playerStats).AsSingle();
        Container.Bind<PlayerXpController>().FromInstance(xpController).AsSingle();
    }
}