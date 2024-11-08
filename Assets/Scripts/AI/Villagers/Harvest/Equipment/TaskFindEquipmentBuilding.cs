using AI.BT;

namespace AI.Villagers.Harvest.Equipment
{
    public class TaskFindEquipmentBuilding : Node
    {
        public override NodeState Evaluate()
        {
            if (!TryGetData("EquipmentType", out object data))
                return NodeState.FAILURE;
        
            EquipmentType equipmentType = (EquipmentType) data;
        
            /*prévoir le temps de déplacement
             c'est à dire le temps que le villager met pour aller à l'endroit où il peut trouver l'équipement
             il faut bloquer un équipement pour le villager
             et ainsi éviter qu'un autre villager plus proche du bâtiment ne prenne le même équipement
             
             enregistrer la position de la porte du batiment
             */
            
            
        
            return NodeState.SUCCESS;
        }
    }
}
