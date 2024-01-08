using Definitions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Systems;
using Systems.Buildings;
using Systems.Camera;
using Systems.Initialize;
using Systems.Input;
using Systems.Ui;
using Systems.Ui.Initialize;
using UnityEngine;

public class EcsEntryPoint : MonoBehaviour
{
    public GameDefinitions gameDefinitions;
    private IEcsSystems systems;
    private EcsWorld world;

    private void Start()
    {
        world = new EcsWorld();
        systems = new EcsSystems(world);
        systems
            .Add(new InitGameLevelSystem())
            .Add(new CubeSystem())
            .Add(new InputSystem())
            .Add(new CameraSystem())
            .Add(new CleanKeysWhenWindowUnfocusSystem())
            .Add(new RaycastObjectSystem())
            .Add(new RaycastCoordinatesDetectSystem())
            .Add(new InGameUiInitializeSystem())
            .Add(new ButtonEventProcessSystem())
            .Add(new OpenBuildPanelSystem())
            .Add(new StartBuildingSystem())
            .Add(new BuildingPlaceSelectionSystem())
            .Add(new BuildingProcessVisualizationSystem())
            .Add(new BuildingInstallSystem())
            .Inject(gameDefinitions)
#if UNITY_EDITOR
            // не выносить префикс неймспейса в юзинг, поломается релизный билд
            .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
            .Add(new Leopotam.EcsLite.UnityEditor.EcsSystemsDebugSystem())
#endif
            .Inject()
            .Init();
    }

    private void Update()
    {
        systems?.Run();
    }

    private void OnDestroy()
    {
        if (systems != null)
        {
            systems.Destroy();
            systems = null;
        }

        if (world != null)
        {
            world.Destroy();
            world = null;
        }
    }
}