using System;
using Godot;
using VampireSurvivorsLike.ItemDrops;

namespace VampireSurvivorsLike {

    public class Enemy : KinematicBody2D {

        public static int mobCount = 0;
        protected int Health { get; set; }
        public int Strength { get; protected set; }
        protected int ExpValue { get; set; }
        protected float Speed { get; set; }
        public int SpawnRate { get; set; }
        public float SpawnDistance { get; set; }
        public Vector2 SpawnTime { get; protected set; }
        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }
        private PackedScene ExpOrb { get; set; }
        private PackedScene Gold { get; set; }
        protected AnimatedSprite AnimatedSprite { get; set; }
        private PackedScene DamageIndicator { get; set; }
        public int spawnAfterMinute = 0;

        protected Enemy() {
            this.ExpOrb = ResourceLoader.Load<PackedScene>("res://ItemDrops/ExpOrbs/ExpOrb.tscn");
            this.Gold = ResourceLoader.Load<PackedScene>("res://ItemDrops/Gold/Gold.tscn");
            this.DamageIndicator = ResourceLoader.Load<PackedScene>("res://GUI/GUI/FloatingValue.tscn");
        }

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

        [Puppet]
        protected void SetPuppetPosition(Vector2 globalPosition, int frame = 0) {
            if (this.Health <= 0) {
                return;
            }
            this.GlobalPosition = globalPosition;
            this.AnimatedSprite.Frame = frame;
        }

        protected void AnimationPlay(EnemyAnimationsEnum enemyAnimations) {
            if (this.Health <= 0 && enemyAnimations != EnemyAnimationsEnum.Death) {
                return;
            }
            this.AnimatedSprite.Play(enemyAnimations.ToString());
        }

        public void OnDeath() {
            if (this.AnimatedSprite.Animation == EnemyAnimationsEnum.Death.ToString()) {
                this.QueueFree();
                ItemDropManager.Instance.CreateExperienceOrb(this.ExpValue, this.GlobalPosition);
                ItemDropManager.Instance.CreateGold(new Random().Next(1, 10), this.GlobalPosition);
            }
        }

        /*
         * When receiving damage reduce the Health by damage amount.
         * If the Health is less than or equals 0 remove the enemy and add an
         * instance of an ExpOrb at its place.
         */
        public void OnHit(int damage, Weapon weapon) {
            this.TakeDamage(damage);
            if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                Rpc(nameof(this.RemoteOnHit), damage);
            }
            if (this.Health <= 0) {
                if (weapon is Aura aura) {
                    this.ExpValue += (int)(this.ExpValue * aura.BonusExperience);
                }
                this.CollisionMask = 0;
                this.AnimationPlay(EnemyAnimationsEnum.Death);
                if (GameStateManagerSingleton.Instance.IsMultiplayer) {
                    Rpc(nameof(this.RemoteOnDeath), this.ExpValue, this.GlobalPosition);
                }
            }
        }

        [Remote]
        public void RemoteOnHit(int damage) {
            this.TakeDamage(damage);
        }

        private void TakeDamage(int damage) {
            if (this.Health <= 0) {
                return;
            }
            AudioPlayerSingleton.Instance.PlayEffect(AudioEffectEnum.EnemyHit);
            this.ShowDamage(damage);
            this.Health -= damage;
        }

        private void ShowDamage(int damage) {
            FloatingValue.CreateFloatingValue(this.GlobalPosition, new Color(0.96f, 0.24f, 0.24f), damage,
                this.GetParent().GetParent());
        }

        [Remote]
        public void RemoteOnDeath(int bonusExp, Vector2 position) {
            this.Health = 0;
            this.GlobalPosition = position;
            this.ExpValue = bonusExp;
            this.AnimationPlay(EnemyAnimationsEnum.Death);
        }

        public void OnTimerTimeout() {
            this.RpcUnreliable(nameof(this.SetPuppetPosition), this.GlobalPosition, this.AnimatedSprite.Frame);
        }

    }

}