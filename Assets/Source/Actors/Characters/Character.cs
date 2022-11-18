using DungeonCrawl.Core;
using System.Collections.Generic;
using UnityEngine;
using Assets.Source.Core;
using DungeonCrawl.Actors.Static;
using System;

namespace DungeonCrawl.Actors.Characters
{
    public abstract class Character : Actor
    {
        public int MaxHealth { get; set; }
        public int ActualHealth { get; set; }
        public int Damage { get; set; }
        public int MaxArmor { get; set; }
        public int ActualArmor { get; set; }

        public void ApplyDamage(int damage, string curse = null)
        {
            if (this.ActualArmor > 0 && curse == null)
            {
                this.ActualArmor -= damage/2;
                
                if(this is Boss1 b1 && ActualArmor <= 0)
                {
                    b1.TrueForm();
                }
            }
            else
            {
                this.ActualHealth -= damage;
            }

            if (this is Player player)
            {
                player.ShowStats();
            }

            if (this is Boss1 boss1)
            {
                boss1.ShowStats();
            }

            if (ActualHealth <= 0)
            {
                // Die
                Drop();
                OnDeath();

                ActorManager.Singleton.DestroyActor(this);
            }
        }

        public void Heal(int hp)
        {
            ActualHealth += hp;
            if (ActualHealth > MaxHealth)
            {
                ActualHealth = MaxHealth;
            }
        }

        protected abstract void OnDeath();

        /// <summary>
        ///     All characters are drawn "above" floor etc
        /// </summary>
        public override int Z => -2;

        public abstract void Starter();

        public void HitEnemy()
        {
            List<Actor> actors = new List<Actor>
            {
                ActorManager.Singleton.GetActorAt((Position.x, Position.y + 1)),
                ActorManager.Singleton.GetActorAt((Position.x, Position.y - 1)),
                ActorManager.Singleton.GetActorAt((Position.x - 1, Position.y)),
                ActorManager.Singleton.GetActorAt((Position.x + 1, Position.y))
            };

            foreach(Actor actor in actors)
            {
                Hit(actor);
            }
        }

        public abstract void Hit(Actor actor);

        public virtual void Drop()
        {
            var rand = new System.Random();
            var dropRate = rand.Next(100);
            if (dropRate < 20) { ActorManager.Singleton.Spawn<Heal>(Position); }
            if (dropRate == 20) { ActorManager.Singleton.Spawn<Meat>(Position); }
        }
    }
}
