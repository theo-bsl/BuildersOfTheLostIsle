using System;
using System.Collections.Generic;
using AI.BT;
using AI.Villagers.Harvest;
using AI.Villagers.Harvest.Equipment;
using AI.Villagers.Harvest.Resource;
using AI.Villagers.Order;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Villagers
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class VillagerBT : BehaviorTree
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _minDistanceToTarget;
        
        private Transform _transform;
        private NavMeshAgent _navMeshAgent;
        private VillagerBlackboard _blackboard;

        private void Awake()
        {
            _transform = transform;
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _blackboard = new VillagerBlackboard();
        }

        protected override Node SetupTree()
        {
            Selector root = new Selector();
            
            root.AddChild(CreatePlayerOrderSubtree());
            
            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                root.AddChild(CreateHarvestSubtree(resourceType));
            }

            return root;
        }
        
        private Node CreatePlayerOrderSubtree()
        {
            return new Sequence(new List<Node>()
            {
                new HaveReceivedPlayerOrder(()=> _blackboard.PlayerOrder != null),
                new DoPlayerOrder(()=> _blackboard.PlayerOrder)
            });
        }

        private Node CreateHarvestSubtree(ResourceType resourceType)
        {
            // Create the root node
            Sequence root = new Sequence();
            
            root.AddChild(new CheckNeedResource(_blackboard.NeedResource));
            root.AddChild(new TaskFindResource());
            root.AddChild(CreateGetEquipmentSubtree(EquipmentType.Horse));
            root.AddChild(CreateGetEquipmentSubtree(EquipmentType.Tools));
            
            root.AddChild(new TaskGoToTarget(_transform, _navMeshAgent, _movementSpeed, _minDistanceToTarget));
            //root.AddChild(new HarvestResource(resourceType));
            
            root.AddChild(CreateReturnEquipmentSubtree(EquipmentType.Tools));
            root.AddChild(CreateReturnEquipmentSubtree(EquipmentType.Horse));
            root.AddChild(CreateDepositResourceSubtree(resourceType));

            return root;
        }
        
        private Node CreateGetEquipmentSubtree(EquipmentType equipmentType)
        {
            return new Selector(new List<Node>()
            {
                new Sequence(new List<Node>()
                {
                    new CheckEquipment(equipmentType, _blackboard.IsEquipmentNeeded),
                    new TaskFindEquipmentBuilding(),//-------------------------------------------------------------------------------------------
                    new TaskGoToTarget(_transform, _navMeshAgent, _movementSpeed, _minDistanceToTarget),
                    new TaskTakeEquipment()//-------------------------------------------------------------------------------------------
                }),
                new SkipToNextAction()
            });
        }
        
        private Node CreateReturnEquipmentSubtree(EquipmentType equipmentType)
        {
            return new Selector(new List<Node>()
            {
                new Sequence(new List<Node>()
                {
                    new CheckEquipment(equipmentType, _blackboard.IsEquipmentAvailable),
                    new TaskGoToTarget(_transform, _navMeshAgent, _movementSpeed, _minDistanceToTarget),
                    new TaskReturnEquipment()//-------------------------------------------------------------------------------------------
                }),
                new SkipToNextAction()
            });
        }
        
        private Node CreateDepositResourceSubtree(ResourceType resourceType)
        {
            return new Sequence(new List<Node>()
            {
                new TaskGoToStorage(),//-------------------------------------------------------------------------------------------
                new TaskDepositResource(resourceType)//-------------------------------------------------------------------------------------------
            });
        }
    }
}
