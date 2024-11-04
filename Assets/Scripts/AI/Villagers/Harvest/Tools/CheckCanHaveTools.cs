using System;
using AI.BT;

namespace AI.Villagers.Harvest.Tools
{
    public class CheckCanHaveTools : Node
    {
        private readonly Func<bool> _canHaveTools;

        public CheckCanHaveTools(Func<bool> canHaveTools)
        {
            _canHaveTools = canHaveTools;
        }

        public override NodeState Evaluate()
        {
            return _canHaveTools() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
