using Components.Buildings;
using Components.Input;
using Components.Navigation;
using Components.Units;
using Helpers;
using Helpers.Cache;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Models.Units;
using UnityEngine;

namespace Systems.Unit
{
    public class UnitGoSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float DistanceToChangeGoal = 0.5f;
        private static readonly int StayTrigger = Animator.StringToHash("Stay");
        private static readonly int RunTrigger = Animator.StringToHash("Run");

        private readonly EcsFilterInject<Inc<UnitComponent>> unitFilter = null;
        private readonly EcsPoolInject<UnitComponent> unitPool = null;

        private readonly EcsFilterInject<Inc<RaycastObjectEvent>> raycastObjectFilter = null;

        private readonly EcsFilterInject<Inc<BuildingModeExitEvent>> buildingModeExitEventFilter = null;
        
        private readonly EcsFilterInject<Inc<GlobalNavigationDataComponent>> navigationDataFilter = null;

        public void Init(IEcsSystems systems)
        {
            SpawnUnit();
        }

        public void Run(IEcsSystems systems)
        {
            StopMovementIfNeed();
            StartMovementIfNeed();
        }

        private void StartMovementIfNeed()
        {
            if (buildingModeExitEventFilter.Value.GetEntitiesCount() != 0)
                return;

            foreach (var raycastObj in raycastObjectFilter.Value)
            {
                ref var raycastComponent = ref raycastObjectFilter.Pools.Inc1.Get(raycastObj);

                if (raycastComponent.RaySourceKeyCode != KeyCode.Mouse1)
                {
                    continue;
                }
                
                var navigationData = navigationDataFilter.Pools.Inc1.Get(navigationDataFilter.Value.Single());
                var path = navigationData.NavigationSystem.FindPathTo(new Vector3(0, 0, 0), raycastComponent.RaycastPoint);
                if (path == null)
                {
                    return;
                }

                foreach (var point in path)
                {
                    Debug.DrawLine(point.Position, point.Position + Vector3.up * 2, Color.white, 10000);
                }

                foreach (var unitEntity in unitFilter.Value)
                {
                    ref var unitComponent = ref unitPool.Value.Get(unitEntity);

                    Debug.Log($"go {raycastComponent.RaycastPoint}");
                    unitComponent.Animator.SetTrigger(RunTrigger);
                    unitComponent.Animator.ResetTrigger(StayTrigger);
                }
            }
        }

        private void StopMovementIfNeed()
        {
            foreach (var unitEntity in unitFilter.Value)
            {
                ref var unitComponent = ref unitPool.Value.Get(unitEntity);

                //var agent = unitComponent.NavMeshAgent;
                //if (agent.remainingDistance < DistanceToChangeGoal)
                //{
                //    unitComponent.Animator.SetTrigger(StayTrigger);
                //    unitComponent.Animator.ResetTrigger(RunTrigger);
                //}
            }
        }

        private void SpawnUnit()
        {
            var prefab = UnitPrefabCache.GetPrefab(UnitType.Black);
            var unit = Object.Instantiate(prefab, new Vector3(-16, 0, 40), Quaternion.Euler(0, 0, 0));

            ref var unitComponent = ref unitPool.NewEntity(out _);
            unitComponent.GameObject = unit;
            unitComponent.Animator = unit.GetComponent<Animator>();
        }
    }
}