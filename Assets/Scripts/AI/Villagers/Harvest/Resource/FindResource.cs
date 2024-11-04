using AI.BT;
using Resources;

namespace AI.Villagers.Harvest
{
    public class FindResource : Node
    {
        private readonly ResourceType _resourceType;

        public FindResource(ResourceType resourceType)
        {
            _resourceType = resourceType;
        }

        public override NodeState Evaluate()
        {
            // Find the closest resource of the given type
            // Move towards it
            // If the villager is close enough, start harvesting
            
            
            
            return NodeState.SUCCESS;
        }
    }
}
