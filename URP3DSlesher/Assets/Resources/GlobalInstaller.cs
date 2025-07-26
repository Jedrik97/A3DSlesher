using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    [SerializeField] private GameObject loadingCanvasPref;
    
    public override void InstallBindings()
    {
        var canvasInstance = Container.InstantiatePrefab(loadingCanvasPref);
        Object.DontDestroyOnLoad(canvasInstance);
        Container.Bind<GameObject>().
            WithId("LoadingCanvas").
            FromInstance(canvasInstance).
            AsSingle();
    }
}