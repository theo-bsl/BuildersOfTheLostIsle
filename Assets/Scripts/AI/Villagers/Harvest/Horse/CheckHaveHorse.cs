using System;
using AI.BT;

namespace AI.Villagers.Harvest.Horse
{
    public class CheckHaveHorse : Node
    {
        private readonly Func<bool> _checkHaveHorse;

        public CheckHaveHorse(Func<bool> checkHaveHorse)
        {
            _checkHaveHorse = checkHaveHorse;
        }
        
        public override NodeState Evaluate()
        {
            return _checkHaveHorse() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}