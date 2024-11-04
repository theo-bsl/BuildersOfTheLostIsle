using System;
using AI.BT;

namespace AI.Villagers.Harvest.Tools
{
    public class HaveTools : Node
    {
        private readonly Func<bool> _hasTools;

        public HaveTools(Func<bool> hasTools)
        {
            _hasTools = hasTools;
        }
    
        public override NodeState Evaluate()
        {
            return _hasTools() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
