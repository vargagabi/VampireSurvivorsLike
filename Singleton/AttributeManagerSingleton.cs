using System.Collections.Generic;
using Godot;

namespace VampireSurvivorsLike {

    public class AttributeManagerSingleton {

        private static AttributeManagerSingleton instance;

        public Attribute MaxHealth { get; set; }
        public Attribute HealthRegen { get; set; }
        public Attribute Speed { get; set; }
        private Attribute PickupRange { get; set; }
        private string IconBasePath { get; set; }

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
            MaxHealth = new Attribute("Increase max health by", 200f, 10f, this.IconBasePath + "HealthIcon");
            HealthRegen = new Attribute("Increase health regeneration by", 2.0f, 50f, this.IconBasePath+"HealthRegenIcon");
            Speed = new Attribute("Increase speed by", 100.0f, 20f, this.IconBasePath + "SpeedIcon");
            this.PickupRange = null;
        }

        public void SetPickupRange(CircleShape2D shape) {
            this.PickupRange = new Attribute("Increase the pickup range by", shape, 10.0f, this.IconBasePath + "PickupAreaIcon");
        }

        public List<Attribute> GetAttributes() {
            return new List<Attribute>() {
                this.MaxHealth, this.HealthRegen, this.Speed, this.PickupRange,
            };
        }

    }

}