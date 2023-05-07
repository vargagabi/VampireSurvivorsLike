using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace VampireSurvivorsLike {

    public class AttributeSaveFormat {

        public string Name;
        public string NameText;
        public string Message;
        public int InitialValue;
        public float BonusModifier;
        public string IconName;
        public int UpgradeCost;
        public int BaseLevel;
        public int MaxBaseLevel;

        public AttributeSaveFormat(string name, string nameText, string message, int initialValue, float bonusModifier, int baseLevel,
            int upgradeCost, string iconName, int maxBaseLevel) {
            this.Name = name;
            this.NameText = nameText;
            this.Message = message;
            this.InitialValue = initialValue;
            this.BonusModifier = bonusModifier;
            this.BaseLevel = baseLevel;
            this.UpgradeCost = upgradeCost;
            this.IconName = iconName;
            this.MaxBaseLevel = maxBaseLevel;
        }

    }

    public class Attribute {

        public string Name { get; }
        public string NameText {get;}
        private string Message { get; }
        private int InitialValue { get; }
        private float BonusModifier { get; }
        public int BaseLevel { get; set; } //The saved level of attribute
        private int InitialCost { get; }
        public Texture Icon { get; }
        private CircleShape2D shape;
        private int sessionLevel; //The level of attribute in a gameplay 
        public int MaxBaseLevel { get; }

        public CircleShape2D Shape {
            set {
                this.shape = value;
                this.shape.Radius = this.GetCurrentValue();
            }
            get => this.shape;
        }

        public Attribute(string name, string nameText, string message, int initialValue, float bonusInPercent, string iconName,
            int initialCost = 100, int baseLevel = 0, int maxBaseLevel = 8) {
            this.Name = name;
            this.NameText = nameText;
            this.Message = message;
            this.InitialValue = initialValue;
            this.BonusModifier = bonusInPercent;
            this.Icon = ResourceLoader.Load($"res://MyPixelArts/images/{iconName}") as Texture;
            this.sessionLevel = 0;
            this.InitialCost = initialCost;
            this.BaseLevel = baseLevel;
            this.MaxBaseLevel = maxBaseLevel;
        }

        public int GetCurrentCost() {
            return (int)(this.InitialCost * Math.Pow(2, this.BaseLevel));
        }

        public int GetCurrentValue() {
            return (int)(this.BaseValue() + (this.BaseValue() * this.BonusModifier) * this.sessionLevel);
        }

        private float BaseValue() {
            return this.InitialValue + this.BaseLevel * this.InitialValue * this.BonusModifier;
        }

        public void Increase() {
            this.sessionLevel++;
            if (this.Shape != null) {
                this.Shape.Radius = this.BaseValue() + this.BaseValue() * this.BonusModifier * this.sessionLevel;
            }
        }

        public override string ToString() {
            return this.Message + " " + this.BonusModifier * 100 + "%";
        }

        public AttributeSaveFormat ToSaveFormat() {
            return new AttributeSaveFormat(this.Name, this.NameText, this.Message, this.InitialValue, this.BonusModifier,
                this.BaseLevel, this.InitialCost, this.Icon.ResourcePath.Split('/').Last(), this.MaxBaseLevel);
        }

    }

}