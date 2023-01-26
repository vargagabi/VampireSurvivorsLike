using System;
using System.Collections.Generic;
using Godot;

namespace VampireSurvivorsLike {

    public class LevelUpManagerSingleton {

        private static LevelUpManagerSingleton instance;
        private int numberOfRewards = 4;

        public static LevelUpManagerSingleton Instance {
            get {
                if (instance == null) {
                    GD.Print("LevelUpManagerSingleton ready...");
                    instance = new LevelUpManagerSingleton();
                }
                return instance;
            }
        }

        public void OnLevelUp() {
            this.GenerateRewards();
            this.ShowRewards();
        }

        private void GenerateRewards() {
            List<Node> possibleRewards = new List<Node>();
            possibleRewards.AddRange(ItemManagerSingleton.Instance.GetEquippedItems());
            possibleRewards.AddRange(ItemManagerSingleton.Instance.GetUnequippedItems());
        }

        private void ShowRewards() {
            throw new NotImplementedException();
        }

    }

}