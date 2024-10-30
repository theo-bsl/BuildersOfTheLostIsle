using System;
using AI.BT;

namespace AI.Villagers.Order
{
    public class HaveReceivedPlayerOrder : Node
    {
        private readonly Func<bool> _checkHaveReceivedPlayerOrder;

        public HaveReceivedPlayerOrder(Func<bool> checkHaveReceivedPlayerOrder)
        {
            _checkHaveReceivedPlayerOrder = checkHaveReceivedPlayerOrder;
        }
        
        public override NodeState Evaluate()
        {
            return _checkHaveReceivedPlayerOrder() ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}