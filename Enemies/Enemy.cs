using System;
using Godot;
using VampireSurvivorsLike.ItemDrops;

namespace VampireSurvivorsLike {

    public class Enemy : KinematicBody2D {

        public static int MobCount = 0;
        public int SpawnRate { get; protected set; }
        public float SpawnDistance { get; protected set; }
        public int Strength { get; protected set; }
        public Vector2 SpawnTime { get; protected set; }
        protected int health;
        protected int expValue;
        protected float speed;

        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        protected AnimatedSprite AnimatedSprite { get; set; }

        public override void _Ready() {
            base._Ready();
            GD.Randomize();
            this.AnimatedSprite = this.GetNode<AnimatedSprite>("AnimatedSprite");
            this.AnimatedSprite.Connect("animation_finished", this, nameof(this.OnDeath));
            this.AnimationPlay(EnemyAnimationsEnum.Walk);
            if (GameStateManagerSingleton.Instance.IsMultiplayer && this.IsNetworkMaster()) {
                Timer timer = new Timer();
                timer.OneShot = false;
                timer.WaitTime = 3;
                timer.Autostart = true;
                timer.Connect("timeout", this, nameof(OnTimerTimeout));
                this.AddChild(timer);
            }
        }

        protected Vector2 GetTargetPosition() {
            if (this.PlayerTwo == null) {
                return this.PlayerOne.GlobalPosition;
            }
            if (this.PlayerOne.IsDead) {
                return this.PlayerTwo.GlobalPosition;
            }
            if (this.PlayerTwo.IsDead) {
                return this.PlayerOne.GlobalPosition;
            }
            return this.GlobalPosition.DistanceTo(this.PlayerOne.GlobalPosition) <
                   this.GlobalPosition.DistanceTo(this.PlayerTwo.GlobalPosition)
                ? this.PlayerOne.GlobalPosition
                : this.PlayerTwo.GlobalPosition;
        }


        protected void AnimationPlay(EnemyAnimationsEnum enemyAnimations) {
            if (this.health <= 0 && enemyAnimations != EnemyAnimationsEnum.Death) {
                return;
            }
            this.AnimatedSprite.Play(enemyAnimations.ToString());
        }

        public void OnDeath() {
            if (this.AnimatedSprite.Animation != EnemyAnimationsEnum.Death.ToString()) {
                return;
            }
            this.QueueFree();
            ItemDropManager.Instance.CreateExperienceOrb(this.expValue, this.GlobalPosition);
            ItemDropManager.Instance.CreateGold(new Random().Next(1, 10), this.GlobalPosition);
        }

        /*
         * When receiving damage reduce the health by damage amount.
         * If the health is less than or equals 0 remove the enemy and add an
         * instance of an ExpOrb at its place.
         */
        public void OnHit(int damage, Weapon weapon) {
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                Rpc(nameof(this.SyncTakeDamage), damage);
            } else {
                this.SyncTakeDamage(damage);
            }
            if (this.health > 0) {
                return;
            }
            if (weapon is Aura aura) {
                this.expValue += (int)(this.expValue * aura.BonusExperience);
            }
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                this.Rpc(nameof(this.SyncOnDeath), this.expValue, this.GlobalPosition);
            } else {
                this.SyncOnDeath(this.expValue, this.GlobalPosition);
            }
        }

        [RemoteSync]
        private void SyncTakeDamage(int damage) {
            if (this.health <= 0) {
                return;
            }
            AudioPlayerSingleton.Instance.PlayEffect(AudioEffectEnum.EnemyHit);
            FloatingValue.CreateFloatingValue(this.GlobalPosition, new Color(0.96f, 0.24f, 0.24f), damage,
                this.GetParent().GetParent());
            this.health -= damage;
        }

        [RemoteSync]
        public void SyncOnDeath(int experience, Vector2 position) {
            this.CollisionMask = 0;
            this.health = 0;
            this.GlobalPosition = position;
            this.expValue = experience;
            this.AnimationPlay(EnemyAnimationsEnum.Death);
        }

        [Puppet]
        protected void SetPuppetPosition(Vector2 globalPosition) {
            if (this.health <= 0) {
                return;
            }
            this.GlobalPosition = globalPosition;
        }

        public void OnTimerTimeout() {
            this.RpcUnreliable(nameof(this.SetPuppetPosition), this.GlobalPosition);
        }

    }

}