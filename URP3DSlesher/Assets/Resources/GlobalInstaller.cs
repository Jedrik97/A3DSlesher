using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    [Header("Prefab")]
    [SerializeField] GameObject loadingCanvasPrefab;

    [Header("Optional")]
    [Tooltip("Also bind the instantiated GameObject with this ID")]
    [SerializeField] string gameObjectBindId = "LoadingCanvas";

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<EnemyDiedSignal>();
        
        if (!loadingCanvasPrefab)
        {
            Debug.LogError("[GlobalInstaller] Loading Canvas Prefab is not assigned.");
            return;
        }
        
        var instance = Container.InstantiatePrefab(loadingCanvasPrefab);
        Object.DontDestroyOnLoad(instance);
        
        var controller = instance.GetComponent<LoadingScreenController>();
        if (!controller)
        {
            Debug.LogError("[GlobalInstaller] LoadingScreenController component is missing on the prefab root.");
            return;
        }
        
        Container.Bind<LoadingScreenController>()
            .FromInstance(controller)
            .AsSingle();

        if (!string.IsNullOrEmpty(gameObjectBindId))
        {
            Container.Bind<GameObject>()
                .WithId(gameObjectBindId)
                .FromInstance(instance)
                .AsSingle();
        }
    }
}