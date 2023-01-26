using System.Collections.Generic;
using Godot;

namespace VampireSurvivorsLike {

    public class AttributeManagerSingleton {

        private static AttributeManagerSingleton instance;

        public Attribute MaxHealth { get; set; }
        public Attribute HealthRegen { get; set; }
        public Attribute Speed { get; set; }
        private Attribute PickupRange { get; set; }

        public static AttributeManagerSingleton Instance {
            get {
                if (instance == null) {
                    GD.Print("AttributeManagerSingleton ready...");
                    instance = new AttributeManagerSingleton();
                }
                return instance;
            }
        }

        public void Reset() {
            MaxHealth = new Attribute("Increase max health by", 200f, 10f);
            HealthRegen = new Attribute("Increase health regeneration by", 2.0f, 50f);
            Speed = new Attribute("Increase speed by", 100.0f, 20f);
            this.PickupRange = null;
        }

        public void SetPickupRange(CircleShape2D shape) {
            this.PickupRange = new Attribute("Increase the pickup range by", shape, 10.0f);
        }

        public List<Attribute> GetAttributes() {
            return new List<Attribute>() {
                this.MaxHealth, this.HealthRegen, this.Speed, this.PickupRange,
            };
        }

    }

}