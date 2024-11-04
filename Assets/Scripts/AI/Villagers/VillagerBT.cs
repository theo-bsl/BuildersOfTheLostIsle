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
                ResourceType.Wood => new FindResource(resourceType),
                ResourceType.Food => new FindResource(resourceType),
                ResourceType.Iron => new FindResource(resourceType),
                _ => null
            };
        }

        private Node CreateGetHorseSubtree()
        {
            return new Selector(new List<Node>()
            {
                new Sequence(new List<Node>()
                {
                    new CanHaveHorse(()=> _blackboard.NeedHorse),
                    new FindStable(),
                    new GoToStable(),
                    new TakeHorse()
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
                    new CanHaveTools(()=> _blackboard.NeedTools),
                    new FindTools(),
                    new GoToTools(),
                    new TakeTools()
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
                    new HaveTools(()=> _blackboard.HasTools),
                    new GoToTools(),
                    new ReturnTools()
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
                    new HaveHorse(()=> _blackboard.HasHorse),
                    new GoToStable(),
                    new ReturnHorse()
                }),
                new SkipToNextAction()
            });
        }
        
        private Node CreateDepositResourceSubtree(ResourceType resourceType)
        {
            return new Sequence(new List<Node>()
            {
                new GoToStorage(),
                new DepositResource(resourceType)
            });
        }
    }
}
