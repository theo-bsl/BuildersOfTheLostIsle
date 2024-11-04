using System;
using AI.BT;

namespace AI.Villagers.Harvest.Tools
{
    public class HasTools : Node
    {
        private readonly Func<bool> _hasTools;

        public HasTools(Func<bool> hasTools)
        {
            _hasTools = hasTools;
        }
    
        public override NodeState Evaluate()
        {
            return _hasTools() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
