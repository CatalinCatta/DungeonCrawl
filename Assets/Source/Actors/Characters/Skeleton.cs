using UnityEngine;
using System.Collections;
using DungeonCrawl.Core;
using DungeonCrawl.Actors.Static;
using Assets.Source.Core;

namespace DungeonCrawl.Actors.Characters
{
    public class Skeleton : Character
    {
        public override bool OnCollision(Actor anotherActor)
        {
            return false;
        }

        protected override void OnDeath()
        {
            Debug.Log("Well, I was already dead anyway...");
        }

        public override void Hit(Actor actor)
        {
            if (actor is Player player)
            {
                if (Position == (6, -8))
                {
                    UserInterface.Singleton.SetText("Press SPACE for combat", UserInterface.TextPosition.TopRight);
                }
                player.ApplyDamage(Damage);
            }
        }

        IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                HitEnemy();
            }
        }

        public override void Starter()
        {
            MaxHealth = 15;
            ActualHealth = 15;
            Damage = 5;
        }

        public override void Drop()
        {
            if (Position == (6, -8))
            {
                ActorManager.Singleton.Spawn<Key>(Position);
            }
            else
            {
                var rand = new System.Random();
                var dropRate = rand.Next(100);
                if (dropRate < 20) { ActorManager.Singleton.Spawn<Heal>(Position); }
                if (dropRate == 20) { ActorManager.Singleton.Spawn<Meat>(Position); }
            }

        }

        public override int DefaultSpriteId => 316;
        public override string DefaultName => "Skeleton";
    }
}
