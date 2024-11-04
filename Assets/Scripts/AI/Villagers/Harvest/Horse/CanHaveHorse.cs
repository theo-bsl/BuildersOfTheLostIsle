using System;
using AI.BT;

namespace AI.Villagers.Harvest.Horse
{
    public class CanHaveHorse : Node
    {
        private readonly Func<bool> _checkNeedHorse;
        
        public CanHaveHorse(Func<bool> checkNeedHorse) 
        {
            _checkNeedHorse = checkNeedHorse;
        }
        
        public override NodeState Evaluate()
        {
            return NodeState.FAILURE;
        }
    }
}