using AI.BT;

namespace AI.Villagers.Harvest
{
    public class TaskFindResource : Node
    {
        public override NodeState Evaluate()
        {
            if (!TryGetData("ResourceType", out object data))
                return NodeState.FAILURE;
            
            ResourceType resourceType = (ResourceType) data;
            
            //
            
            return NodeState.SUCCESS;
        }
    }
}
