using AI.BT;

namespace AI.Villagers.Harvest.Horse
{
    public class CanHaveHorse : Node
    {
        public override NodeState Evaluate()
        {
            return NodeState.FAILURE;
        }
    }
}