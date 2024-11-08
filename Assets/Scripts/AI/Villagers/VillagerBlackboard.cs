using AI.BT;

namespace AI.Villagers
{
    public class VillagerBlackboard
    {
        public Node PlayerOrder { get; set; }
        
        
        public bool NeedWood { get; set; }
        public bool NeedIron { get; set; }
        public bool NeedFood { get; set; }
        
        public ResourceType NeedResource()
        {
            if (NeedWood)
                return ResourceType.Wood;
            else if (NeedIron)
                return ResourceType.Iron;
            else if (NeedFood)
                return ResourceType.Food;
            
            return ResourceType.None;
        }


        public bool NeedHorse { get; set; }
        public bool HasHorse { get; set; }
        
        
        public bool NeedTools { get; set; }
        public bool HasTools { get; set; }
        
        public bool IsEquipmentNeeded(EquipmentType equipmentType)
        {
            return equipmentType switch
            {
                EquipmentType.Horse => NeedHorse,
                EquipmentType.Tools => NeedTools,
                _ => false
            };
        }

        public bool IsEquipmentAvailable(EquipmentType equipmentType)
        {
            return equipmentType switch
            {
                EquipmentType.Horse => HasHorse,
                EquipmentType.Tools => HasTools,
                _ => false
            };
        }        
    }
}