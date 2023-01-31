using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace VampireSurvivorsLike {

    public class AttributeSaveFormat {

        public string name;
        public string message;
        public float initialValue;
        public float bonusModifier;
        public string iconName;
        public int upgradeCost;
        public int baseLevel;

        public AttributeSaveFormat(string name, string message, float initialValue, float bonusModifier, int baseLevel,
            int upgradeCost, string iconName) {
            this.name = name;
            this.message = message;
            this.initialValue = initialValue;
            this.bonusModifier = bonusModifier;
            this.baseLevel = baseLevel;
            this.upgradeCost = upgradeCost;
            this.iconName = iconName;
        }

    }

    public class Attribute {

        public string Name { get; private set; }
        private string Message { get; set; }
        private float InitialValue { get; set; }
        private float BonusModifier { get; set; }
        public int BaseLevel { get; set; }
        private int UpgradeCost { get; set; }
        public Texture Icon { get; private set; }

        private CircleShape2D shape;
        private int sessionLevel;

        public CircleShape2D Shape {
            set {
                this.shape = value;
                this.shape.Radius = this.GetCurrentValue();
            }
            get => this.shape;
        }

        public Attribute(string name, string message, float initialValue, float bonusInPercent, string iconName, int upgradeCost=100, int baseLevel = 0 ) {
            this.Name = name;
            this.Message = message;
            this.InitialValue = initialValue;
            this.BonusModifier = bonusInPercent;
            this.Icon = ResourceLoader.Load($"res://MyPixelArts/images/{iconName}") as Texture;
            this.sessionLevel = 0;
            this.UpgradeCost = upgradeCost;
            this.BaseLevel = baseLevel;
        }

        public float GetCurrentValue() {
            return this.BaseValue() + (this.BaseValue() * this.BonusModifier) * this.sessionLevel;
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
            return new AttributeSaveFormat(this.Name, this.Message, this.InitialValue, this.BonusModifier,
                this.BaseLevel, this.UpgradeCost, this.Icon.ResourcePath.Split('/').Last());
        }

    }

}