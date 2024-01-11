using Components.Input;
using Components.Units;
using Helpers.Cache;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Models.Units;
using UnityEngine;
using UnityEngine.AI;

namespace Systems.Unit
{
    public class UnitGoSystem : IEcsInitSystem, IEcsRunSystem
    {
        private static readonly float distanceToChangeGoal = 0.5f;

        private readonly EcsFilterInject<Inc<UnitComponent>> unitFilter = null;
        private readonly EcsPoolInject<UnitComponent> unitPool = null;

        private readonly EcsFilterInject<Inc<InputKeyPressedEvent, InputKeyDownEvent>> keyDownEventFilter = null;
        private static readonly int StayTrigger = Animator.StringToHash("Stay");
        private static readonly int RunTrigger = Animator.StringToHash("Run");

        public void Init(IEcsSystems systems)
        {
            SpawnUnit();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var unitEntity in unitFilter.Value)
            {
                ref var unitComponent = ref unitPool.Value.Get(unitEntity);

                var agent = unitComponent.NavMeshAgent;
                if (agent.remainingDistance < distanceToChangeGoal)
                {
                    unitComponent.Animator.SetTrigger(StayTrigger);
                    unitComponent.Animator.ResetTrigger(RunTrigger);
                    foreach (var keyEntity in keyDownEventFilter.Value)
                    {
                        if (keyDownEventFilter.Pools.Inc1.Get(keyEntity).Code == KeyCode.Space)
                        {
                            agent.SetDestination(new Vector3(Random.Range(-20, 20f), 0, Random.Range(-20, 20f)));
                            unitComponent.Animator.SetTrigger(RunTrigger);
                            unitComponent.Animator.ResetTrigger(StayTrigger);
                        }
                    }
                }
            }
        }

        private void SpawnUnit()
        {
            var prefab = UnitPrefabCache.GetPrefab(UnitType.Black);
            var unit = Object.Instantiate(prefab, new Vector3(10, 0, 10), Quaternion.Euler(0, 0, 0));

            ref var unitComponent = ref unitPool.NewEntity(out _);
            unitComponent.GameObject = unit;
            unitComponent.NavMeshAgent = unit.GetComponent<NavMeshAgent>();
            unitComponent.Animator = unit.GetComponent<Animator>();
        }
    }
}