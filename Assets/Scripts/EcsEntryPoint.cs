using Definitions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.UnityEditor;
using Systems;
using UnityEngine;

public class EcsEntryPoint : MonoBehaviour
{
    public GameDefinitions gameDefinitions;
    private IEcsSystems _systems;
    private EcsWorld _world;

    private void Start()
    {
        // Создаем окружение, подключаем системы.
        _world = new EcsWorld();
        _systems = new EcsSystems(_world);
        _systems
            .Add(new InitWorldSystem())
            .Add(new CubeRoundSystem())
            .Add(new InputSystem())
            .Add(new CameraSystem())
            .Add(new CleanKeysWhenWindowUnfocusSystem())
            .Inject(gameDefinitions)
#if UNITY_EDITOR
            // Регистрируем отладочные системы по контролю за состоянием каждого отдельного мира:
            // .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ("events"))
            .Add(new EcsWorldDebugSystem())
            // Регистрируем отладочные системы по контролю за текущей группой систем. 
            .Add(new EcsSystemsDebugSystem())
#endif
            .Inject()
            .Init();
    }

    private void Update()
    {
        // Выполняем все подключенные системы.
        _systems?.Run();
    }

    private void OnDestroy()
    {
        // Уничтожаем подключенные системы.
        if (_systems != null)
        {
            _systems.Destroy();
            _systems = null;
        }

        // Очищаем окружение.
        if (_world != null)
        {
            _world.Destroy();
            _world = null;
        }
    }
}