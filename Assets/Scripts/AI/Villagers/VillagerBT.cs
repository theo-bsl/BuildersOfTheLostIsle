using System;
using System.Collections.Generic;
using AI.BT;
using AI.Villagers.Harvest;
using AI.Villagers.Harvest.Horse;
using AI.Villagers.Harvest.Resource;
using AI.Villagers.Harvest.Tools;
using AI.Villagers.Order;
using Resources;
using UnityEngine;

namespace AI.Villagers
{
    [RequireComponent(typeof(VillagerBlackboard))]
    public class VillagerBT : BehaviorTree
    {
        private VillagerBlackboard _blackboard;

        private void Awake()
        {
            _blackboard = GetComponent<VillagerBlackboard>();
        }

        protected override Node SetupTree()
        {
            Selector root = new Selector();
            
            root.AddChild(CreatePlayerOrderSubtree());
            
            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                Node harvestSubtree = CreateHarvestSubtree(resourceType);
                root.AddChild(harvestSubtree);
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
            
            root.AddChild(CreateCheckNeedResourceNode(resourceType));
            root.AddChild(CreateFindResourceNode(resourceType));
            root.AddChild(CreateGetHorseSubtree());
            root.AddChild(CreateGetToolsSubtree());
            
            //root.AddChild(new HarvestResource(resourceType));
            
            root.AddChild(CreateReturnToolsSubtree());
            root.AddChild(CreateReturnHorseSubtree());
            root.AddChild(CreateDepositResourceSubtree(resourceType));

            return root;
        }

        private Node CreateCheckNeedResourceNode(ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Wood => new CheckNeedResource(() => _blackboard.NeedWood),
                ResourceType.Food => new CheckNeedResource(() => _blackboard.NeedIron),
                ResourceType.Iron => new CheckNeedResource(() => _blackboard.NeedFood),
                _ => null
            };
        }
        
        private Node CreateFindResourceNode(ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Wood => new TaskFindResource(resourceType),
                ResourceType.Food => new TaskFindResource(resourceType),
                ResourceType.Iron => new TaskFindResource(resourceType),
                _ => null
            };
        }

        private Node CreateGetHorseSubtree()
        {
            return new Selector(new List<Node>()
            {
                new Sequence(new List<Node>()
                {
                    new CheckCanHaveHorse(()=> _blackboard.NeedHorse),
                    new TaskFindStable(),
                    new TaskGoToStable(),
                    new TaskTakeHorse()
                }),
                new SkipToNextAction()
            });
        }
        
        private Node CreateGetToolsSubtree()
        {
            return new Selector(new List<Node>()
            {
                new Sequence(new List<Node>()
                {
                    new CheckCanHaveTools(()=> _blackboard.NeedTools),
                    new TaskFindTools(),
                    new TaskGoToTools(),
                    new TaskTakeTools()
                }),
                new SkipToNextAction()
            });
        }
        
        private Node CreateReturnToolsSubtree()
        {
            return new Selector(new List<Node>()
            {
                new Sequence(new List<Node>()
                {
                    new CheckHaveTools(()=> _blackboard.HasTools),
                    new TaskGoToTools(),
                    new TaskReturnTools()
                }),
                new SkipToNextAction()
            });
        }
        
        private Node CreateReturnHorseSubtree()
        {
            return new Selector(new List<Node>()
            {
                new Sequence(new List<Node>()
                {
                    new CheckHaveHorse(()=> _blackboard.HasHorse),
                    new TaskGoToStable(),
                    new TaskReturnHorse()
                }),
                new SkipToNextAction()
            });
        }
        
        private Node CreateDepositResourceSubtree(ResourceType resourceType)
        {
            return new Sequence(new List<Node>()
            {
                new TaskGoToStorage(),
                new TaskDepositResource(resourceType)
            });
        }
    }
}
