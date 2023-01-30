using System;
using Godot;
using Godot.Collections;

namespace VampireSurvivorsLike {

    public class Attribute {

        private string name;
        private string Message { get; set; }
        private float initialValue;
        private CircleShape2D shape;
        private readonly float bonusModifier;
        private int sessionLevel;
        private int baseLevel;
        public Texture Icon { get; set; }

        public CircleShape2D Shape {
            set {
                this.shape = value;
                this.initialValue = this.shape.Radius;
            }
            get => this.shape;
        }
        public Attribute(string name, string message, float initialValue, float bonusInPercent, string iconPath) {
            this.name = name;
            this.Message = message;
            this.initialValue = initialValue;
            this.bonusModifier = bonusInPercent / 100f;
            this.Icon = ResourceLoader.Load($"res://MyPixelArts/images/{iconPath}.png") as Texture;
            this.sessionLevel = 0;
        }

        public Attribute(Dictionary<string, object> data) {
            this.name = (string)data[ToLowerFirstChar(nameof(this.name))]; 
            this.Message = data[ToLowerFirstChar(nameof(this.Message))] as string;
            this.initialValue = (float)data[ToLowerFirstChar(nameof(this.initialValue))];
            this.bonusModifier = (float)data[ToLowerFirstChar(nameof(this.bonusModifier))];
            this.Icon = ResourceLoader.Load(data[ToLowerFirstChar(nameof(this.Icon))] as string) as Texture;
            this.sessionLevel = 0;
        }

        public float GetCurrentValue() {
            return this.BaseValue() + (this.BaseValue() * this.bonusModifier) * this.sessionLevel;
        }

        private float BaseValue() {
           return this.initialValue + this.baseLevel * this.initialValue * this.bonusModifier; 
        }

        public void Increase() {
            this.sessionLevel++;
            if (this.Shape != null) {
                this.Shape.Radius = this.BaseValue() + this.BaseValue() * this.bonusModifier * this.sessionLevel;
            }
        }

        public override string ToString() {
            return this.Message + " " + this.bonusModifier * 100 + "%";
        }

        public Dictionary<string,object> ToJson() {
            return new Dictionary<string, object>() {
                {this.ToLowerFirstChar(nameof(this.name)), this.name}, 
                {this.ToLowerFirstChar(nameof(this.Message)), this.Message},
                {this.ToLowerFirstChar(nameof(this.initialValue)), this.initialValue},
                {this.ToLowerFirstChar(nameof(this.bonusModifier)), this.bonusModifier},
                {this.ToLowerFirstChar(nameof(this.baseLevel)), this.baseLevel},
                {this.ToLowerFirstChar(nameof(this.Icon)), this.Icon.ResourcePath},
            };
        }

        private string ToLowerFirstChar(string value) {
            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        public void Load(Dictionary<string, object> data) {
            GD.Print(data["name"] + "=> Loaded");
            
        }
    }

}