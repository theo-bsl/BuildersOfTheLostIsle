using System;
using AI.BT;

namespace AI.Villagers.Harvest.Horse
{
    public class HaveHorse : Node
    {
        private readonly Func<bool> _checkHaveHorse;

        public HaveHorse(Func<bool> checkHaveHorse)
        {
            _checkHaveHorse = checkHaveHorse;
        }
        
        public override NodeState Evaluate()
        {
            return _checkHaveHorse() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}