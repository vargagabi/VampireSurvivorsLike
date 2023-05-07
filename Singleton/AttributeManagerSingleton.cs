using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace VampireSurvivorsLike {

    public class AttributeManagerSingleton : Node {

        private static AttributeManagerSingleton instance;

        public Attribute MaxHealth { get; private set; }
        public Attribute HealthRegen { get; private set; }
        public Attribute Speed { get; private set; }
        public Attribute PickupArea { get; private set; }
        public int Gold { get; set; }
        private int goldSpentOnAttributes = 0;
        private const string SavePath = "user://attributes.cfg";

        public static AttributeManagerSingleton Instance {
            get => instance;
            private set {
                instance = value;
                Init();
            }
        }

        public override void _Ready() {
            Instance = this;
        }

        private static void Init() {
            File file = new File();
            if (!file.FileExists(SavePath) || file.Open(SavePath, File.ModeFlags.Read) != Error.Ok ||
                file.GetLen() == 0) {
                Instance.Reset();
                Instance.Save();
            } else {
                Instance.Load();
            }
        }

        private void Reset() {
            this.MaxHealth = new Attribute("MaxHealth", "Health", "Increase max health by", 200, 0.1f, "HealthIcon.png", 50);
            this.HealthRegen = new Attribute("HealthRegen", "Regeneration", "Increase health regeneration by", 2, 0.5f,
                "HealthRegenIcon.png", 80);
            this.Speed = new Attribute("Speed","Speed", "Increase speed by", 100, 0.05f, "SpeedIcon.png", 100);
            this.PickupArea = new Attribute("PickupArea", "Pickup Area", "Increase the pickup range by", 25, 0.5f,
                "PickupAreaIcon.png", 150);
            this.Gold = 500;
            this.goldSpentOnAttributes = 0;
        }

        public void ResetAttributes() {
            foreach (PropertyInfo prop in this.GetType().GetProperties()) {
                if (prop.GetValue(this) is Attribute attribute) {
                    attribute.BaseLevel = 0;
                }
            }
            this.Gold += this.goldSpentOnAttributes;
            this.goldSpentOnAttributes = 0;
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
                    file.SetValue(attr.Name, prop.Name, prop.GetValue(attr));
                }
            }
            file.SetValue("Gold", "value", this.Gold);
            file.SetValue("GoldSpentOnAttributes", "value", this.goldSpentOnAttributes);
            file.Save(SavePath);
        }

        private void Load() {
            ConfigFile file = new ConfigFile();
            if (file.Load(SavePath) != Error.Ok) {
                return;
            }
            foreach (string section in file.GetSections()) {
                string[] keys = file.GetSectionKeys(section);
                if (section == "Gold") {
                    this.Gold = (int)file.GetValue(section, keys[0]);
                    continue;
                }
                if (section == "GoldSpentOnAttributes") {
                    this.goldSpentOnAttributes = (int)file.GetValue(section, keys[0]);
                    continue;
                }
                this.GetType().GetProperty(section)?.SetValue(this, new Attribute(
                    (string)file.GetValue(section, keys[0]),
                    (string)file.GetValue(section, keys[1]),
                    (string)file.GetValue(section, keys[2]),
                    (int)file.GetValue(section, keys[3]),
                    (float)file.GetValue(section, keys[4]),
                    (string)file.GetValue(section, keys[5]),
                    (int)file.GetValue(section, keys[6]),
                    (int)file.GetValue(section, keys[7])
                ));
            }
        }

        public bool IncreaseBaseLevel(string attributeName) {
            PropertyInfo prop = this.GetType().GetProperty(attributeName);
            if (prop != null && prop.GetValue(this) is Attribute attribute &&
                attribute.BaseLevel < attribute.MaxBaseLevel && attribute.GetCurrentCost() <= this.Gold) {
                this.Gold -= attribute.GetCurrentCost();
                this.goldSpentOnAttributes += attribute.GetCurrentCost();
                attribute.BaseLevel += 1;
                return true;
            }
            return false;
        }

        public int GetAttributeBaseLevel(string attributeName) {
            PropertyInfo prop = this.GetType().GetProperty(attributeName);
            if (prop != null && prop.GetValue(this) is Attribute attribute) {
                return attribute.BaseLevel;
            }
            return -1;
        }

        public int GetAttributeCost(string attributeName) {
            PropertyInfo prop = this.GetType().GetProperty(attributeName);
            if (prop != null && prop.GetValue(this) is Attribute attribute) {
                return attribute.GetCurrentCost();
            }
            return -1;
        }

        public int GetCurrentCost(string attributeName) {
            PropertyInfo prop = this.GetType().GetProperty(attributeName);
            Attribute attribute = (prop.GetValue(this) as Attribute);
            return attribute.GetCurrentCost();
        }

        public bool DecreaseGold(int value) {
            if (this.Gold - value < 0) {
                return false;
            }
            this.Gold -= value;
            return true;
        }

        public void IncreaseGold(int value) {
            this.Gold += value;
        }

    }

}