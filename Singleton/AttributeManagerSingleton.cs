using System.Collections.Generic;
using System.Reflection;
using Godot;
using Godot.Collections;

namespace VampireSurvivorsLike {

    public class AttributeManagerSingleton : Node {

        private static AttributeManagerSingleton instance;

        public Attribute MaxHealth { get; private set; }
        public Attribute HealthRegen { get; private set; }
        public Attribute Speed { get; private set; }
        public Attribute PickupArea { get; private set; }
        public int Gold { get; set; }
        private const string savePath = "user://attributes.cfg";

        public readonly Vector2 levelRange = new Vector2(0, 8);
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
            File file = new File();
            if (!file.FileExists(savePath) || file.Open(savePath, File.ModeFlags.Read) != Error.Ok ||
                file.GetLen() == 0) {
                Instance.Reset();
                Instance.Save();
            } else {
                Instance.Load();
            }
        }

        public void Reset() {
            this.MaxHealth = new Attribute("MaxHealth", "Increase max health by", 200f, 10f/100f, "HealthIcon.png");
            this.HealthRegen = new Attribute("HealthRegen", "Increase health regeneration by", 2.0f, 50f/100f,
                "HealthRegenIcon.png");
            this.Speed = new Attribute("Speed", "Increase speed by", 100.0f, 20f/100f, "SpeedIcon.png");
            this.PickupArea = new Attribute("PickupArea", "Increase the pickup range by", 15f, 10.0f/100f,
                "PickupAreaIcon.png");
            this.Gold = 500;
        }

        public void SetPickupArea(CircleShape2D shape) {
            this.PickupArea.Shape = shape;
        }

        public List<Attribute> GetAttributes() {
            return new List<Attribute>() {
                this.MaxHealth, this.HealthRegen, this.Speed, this.PickupArea,
            };
        }

        private List<AttributeSaveFormat> GetAttributesForSave() {
            return new List<AttributeSaveFormat>() {
                this.MaxHealth.ToSaveFormat(), this.HealthRegen.ToSaveFormat(), this.Speed.ToSaveFormat(),
                this.PickupArea.ToSaveFormat(),
            };
        }

        public void Save() {
            ConfigFile file = new ConfigFile();
            foreach (AttributeSaveFormat attr in this.GetAttributesForSave()) {
                foreach (FieldInfo prop in attr.GetType().GetFields()) {
                    file.SetValue(attr.name, prop.Name, prop.GetValue(attr));
                }
            }
            file.SetValue("Gold","value", this.Gold);
            file.Save(savePath);
        }

        public void Load() {
            ConfigFile file = new ConfigFile();
            if (file.Load(savePath) != Error.Ok) {
                return;
            }
            foreach (string section in file.GetSections()) {
                string[] keys = file.GetSectionKeys(section);
                if (section == "Gold") {
                    this.Gold = (int)file.GetValue(section, keys[0]);    
                    continue;
                }
                this.GetType().GetProperty(section)?.SetValue(this, new Attribute(
                    (string)file.GetValue(section, keys[0]),
                    (string)file.GetValue(section, keys[1]),
                    (float)file.GetValue(section, keys[2]),
                    (float)file.GetValue(section, keys[3]),
                    (string)file.GetValue(section, keys[4]),
                    (int)file.GetValue(section, keys[5]),
                    (int)file.GetValue(section, keys[6])
                ));
            }
        }

        public int IncreaseBaseLevel(string attributeName) {
            PropertyInfo prop = this.GetType().GetProperty(attributeName);
            Attribute attribute = (prop.GetValue(this) as Attribute);
            if (attribute != null && attribute.BaseLevel + 1 >= this.levelRange.x && attribute.BaseLevel + 1 <= this.levelRange.y) {
                attribute.BaseLevel += 1;
            }
            return attribute.BaseLevel;
        }

        public bool DecreaseGold(int value) {
            if (this.Gold - value < 0) {
                return false;
            }
            this.Gold -=value;
            return true;
        }

    }

}