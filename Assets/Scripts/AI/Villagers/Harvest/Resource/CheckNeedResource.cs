using System;
using AI.BT;

namespace AI.Villagers.Harvest
{
    public class CheckNeedResource : Node
    {
        private readonly Func<ResourceType> _checkNeedResource;

        public CheckNeedResource(Func<ResourceType> checkNeedResource)
        {
            _checkNeedResource = checkNeedResource;
        }

        public override NodeState Evaluate()
        {
            ResourceType resourceType = _checkNeedResource();
            
            if (resourceType == ResourceType.None)
                return NodeState.FAILURE;
            
            SetDataInParent("ResourceType", resourceType);
            
            return NodeState.SUCCESS;
        }
    }
}
