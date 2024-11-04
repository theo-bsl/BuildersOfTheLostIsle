using AI.BT;
using Resources;

namespace AI.Villagers.Harvest.Resource
{
    public class DepositResource : Node
    {
        private ResourceType _resourceType;
        
        public DepositResource(ResourceType resourceType)
        {
            _resourceType = resourceType;
        }
        
        public override NodeState Evaluate()
        {
            return NodeState.FAILURE;
        }
    }
}
