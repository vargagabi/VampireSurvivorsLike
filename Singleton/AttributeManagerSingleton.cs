using System.Collections.Generic;

namespace VampireSurvivorsLike { 

    public class AttributeManagerSingleton {

        private static AttributeManagerSingleton instance;
        private List<Attribute> Attributes { get; set; }

        public static AttributeManagerSingleton Instance {
            get {
                if (instance == null) {
                    instance = new AttributeManagerSingleton();
                    InitAttributes();
                }
                return instance;
            }
        }

        private static void InitAttributes() {
            Attribute maxHealth = new Attribute("Increase max health", 200f, 10f);
            Attribute healthRegen = new Attribute("Increase health regeneration", 5.0f, 10f);
            Attribute speed = new Attribute("Increase speed", 100.0f, 20f);
            Attribute pickupRange;
            List<Attribute> upgradeableStats = new List<Attribute>();
        }

    }

}