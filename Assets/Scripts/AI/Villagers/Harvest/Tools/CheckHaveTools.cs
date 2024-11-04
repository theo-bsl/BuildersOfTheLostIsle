using System;
using AI.BT;

namespace AI.Villagers.Harvest.Tools
{
    public class CheckHaveTools : Node
    {
        private readonly Func<bool> _hasTools;

        public CheckHaveTools(Func<bool> hasTools)
        {
            _hasTools = hasTools;
        }
    
        public override NodeState Evaluate()
        {
            return _hasTools() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
