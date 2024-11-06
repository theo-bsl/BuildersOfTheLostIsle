using AI.BT;

namespace AI.Villagers.Harvest.Resource
{
    public class TaskDepositResource : Node
    {
        private ResourceType _resourceType;
        
        public TaskDepositResource(ResourceType resourceType)
        {
            _resourceType = resourceType;
        }
        
        public override NodeState Evaluate()
        {
            return NodeState.FAILURE;
        }
    }
}
