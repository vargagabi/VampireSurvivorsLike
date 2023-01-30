using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace VampireSurvivorsLike {

    public class AttributeManagerSingleton : Node {

        private static AttributeManagerSingleton instance;

        public Attribute MaxHealth { get; private set; }
        public Attribute HealthRegen { get; private set; }
        public Attribute Speed { get; private set; }
        public Attribute PickupArea { get; private set; }
        
        private const string savePath = "user://attributes.save";

        public static AttributeManagerSingleton Instance {
            get => instance;
            private set {
                instance = value;
                Init();
            }
        }

        public override void _Ready() {
            GD.Print("AttributeManagerSingleton ready...");
            Instance = this;
        }

        private static void Init() {
            File state = new File();
            if (!state.FileExists(savePath)) {
                Instance.Reset();
                Instance.Save();
            } else {
                Instance.Load();
            }
        }

        public void Reset() {
            this.MaxHealth = new Attribute("MaxHealth", "Increase max health by", 200f, 10f,  "HealthIcon");
            this.HealthRegen = new Attribute("HealthRegen", "Increase health regeneration by", 2.0f, 50f, "HealthRegenIcon");
            this.Speed = new Attribute("Speed", "Increase speed by", 100.0f, 20f, "SpeedIcon");
            this.PickupArea = new Attribute("PickupArea", "Increase the pickup range by", 0f, 10.0f, "PickupAreaIcon");
        }

        public void SetPickupArea(CircleShape2D shape) {
            this.PickupArea.Shape = shape;
        }

        public List<Attribute> GetAttributes() {
            return new List<Attribute>() {
                this.MaxHealth, this.HealthRegen, this.Speed, this.PickupArea,
            };
        }

        public void Save() {
            File state = new File();
            state.Open(savePath, File.ModeFlags.Write);
            foreach (Attribute a in this.GetAttributes()) {
                state.StoreLine(JSON.Print(a.ToJson()));
            }
            state.Close();
        }

        public void Load() {
            File state = new File();
            state.Open("user://attributes.save", File.ModeFlags.Read);
            while (state.GetPosition() < state.GetLen()) {
                Godot.Collections.Dictionary<string, object> data =
                    new Godot.Collections.Dictionary<string, object>((Dictionary)JSON.Parse(state.GetLine()).Result);
                this.GetType().GetProperty((string)data["name"])?.SetValue(this, new Attribute(data));
            }
        }

    }

}