using System;
using AI.BT;

namespace AI.Villagers.Harvest
{
    public class CheckNeedResource : Node
    {
        private readonly Func<bool> _checkNeedResource;

        public CheckNeedResource(Func<bool> checkNeedResource)
        {
            _checkNeedResource = checkNeedResource;
        }

        public override NodeState Evaluate()
        {
            return _checkNeedResource() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
