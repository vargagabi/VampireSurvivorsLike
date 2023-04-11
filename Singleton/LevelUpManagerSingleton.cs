using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace VampireSurvivorsLike {

    public class LevelUpManagerSingleton {

        private static LevelUpManagerSingleton instance;

        private bool isRewardSelected = false;
        public Player Player;
        public LevelUpScreen LevelUpScreen;
        private const int NumberOfRewards = 4;
        private List<object> generatedRewards;

        public static LevelUpManagerSingleton Instance => instance ?? (instance = new LevelUpManagerSingleton());

        public async Task OnPlayerLevelUp(int numberOfLevelUps) {
            if (GameStateManagerSingleton.Instance.GameState.Equals(GameStateEnum.GameFinished)) {
                return;
            }
            for (int i = 0; i < numberOfLevelUps; i++) {
                this.isRewardSelected = false;
                this.generatedRewards = this.GenerateRewards();
                while (!this.isRewardSelected) {
                    await Task.Delay(250);
                }
            }
        }

        private List<object> GenerateRewards() {
            List<object> possibleRewards = new List<object>();

            possibleRewards.AddRange(this.Player.ItemManager.ItemNodes
                .Where(item => item.Level < item.MaxLevel || item.Level == 0).Select(item => item));
            possibleRewards.AddRange(AttributeManagerSingleton.Instance.GetAttributes());

            List<object> rewards = new List<object>();
            for (int i = 0; i < NumberOfRewards; i++) {
                int index = (int)GD.RandRange(0, possibleRewards.Count);
                rewards.Add(possibleRewards[index]);
                possibleRewards.RemoveAt(index);
            }
            this.LevelUpScreen.SetRewards(rewards);
            return rewards;
        }

        public void OnRewardSelected(int index) {
            object reward = this.generatedRewards[index];
            if (reward is Item item) {
                this.Player.ItemManager.EquipOrUpgradeItem(item.Id);
            } else if (reward is Attribute attribute) {
                attribute.Increase();
            }
            this.isRewardSelected = true;
        }

    }

}