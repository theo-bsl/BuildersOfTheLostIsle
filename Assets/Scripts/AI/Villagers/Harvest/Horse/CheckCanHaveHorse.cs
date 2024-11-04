using System;
using AI.BT;

namespace AI.Villagers.Harvest.Horse
{
    public class CheckCanHaveHorse : Node
    {
        private readonly Func<bool> _checkNeedHorse;
        
        public CheckCanHaveHorse(Func<bool> checkNeedHorse) 
        {
            _checkNeedHorse = checkNeedHorse;
        }
        
        public override NodeState Evaluate()
        {
            return NodeState.FAILURE;
        }
    }
}