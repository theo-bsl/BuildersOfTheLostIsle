using AI.BT;
using UnityEngine;

namespace AI.Villagers
{
    public class VillagerBlackboard : MonoBehaviour
    {
        public Node PlayerOrder { get; set; }
        
        public bool NeedWood { get; set; }

        public bool NeedIron { get; set; }

        public bool NeedFood { get; set; }
        
        public bool NeedHorse { get; set; }
        
        public bool NeedTools { get; set; }
        
        public bool HasHorse { get; set; }
        
        public bool HasTools { get; set; }
    }
}