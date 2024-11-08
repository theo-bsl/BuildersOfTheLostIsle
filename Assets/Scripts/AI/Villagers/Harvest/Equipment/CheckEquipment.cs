using System;
using AI.BT;

namespace AI.Villagers.Harvest.Equipment
{
    public class CheckEquipment : Node
    {
        private readonly EquipmentType _equipmentType;
        private readonly Func<EquipmentType, bool> _checkEquipment;
    
        public CheckEquipment(EquipmentType equipmentType, Func<EquipmentType, bool> checkEquipment) 
        {
            _equipmentType = equipmentType;
            _checkEquipment = checkEquipment;
        }
    
        public override NodeState Evaluate()
        {
            if (!_checkEquipment(_equipmentType))
                return NodeState.FAILURE;
        
            SetDataInParent("EquipmentType", _equipmentType);
        
            return NodeState.SUCCESS;
        }
    }
}
