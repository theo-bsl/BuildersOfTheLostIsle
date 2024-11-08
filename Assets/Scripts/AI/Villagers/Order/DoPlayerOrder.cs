using System;
using AI.BT;

namespace AI.Villagers.Order
{
    public class DoPlayerOrder : Node
    {
        private readonly Func<Node> _getPlayerOrder;
        
        public DoPlayerOrder(Func<Node> getPlayerOrder)
        {
            _getPlayerOrder = getPlayerOrder;
        }
        
        public override NodeState Evaluate()
        {
            return _getPlayerOrder().Evaluate();
        }
    }
}
