using System;
using AI.BT;

namespace AI.Villagers.Harvest.Tools
{
    public class CanHaveTools : Node
    {
        private readonly Func<bool> _canHaveTools;

        public CanHaveTools(Func<bool> canHaveTools)
        {
            _canHaveTools = canHaveTools;
        }

        public override NodeState Evaluate()
        {
            return _canHaveTools() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
