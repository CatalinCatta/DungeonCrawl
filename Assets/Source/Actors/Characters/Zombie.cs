using UnityEngine;
using System.Collections;
using DungeonCrawl.Core;
using DungeonCrawl.Actors.Static;
using Assets.Source.Core;
using System;

namespace DungeonCrawl.Actors.Characters
{
    public class Zombie : Character
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
                player.ApplyDamage(Damage);
            }
        }

        IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                RandomPosition();
                HitEnemy();
                Heal(1);
            }
        }

        public void RandomPosition()
        {
            Array values = Enum.GetValues(typeof(Direction));
            System.Random random = new System.Random();
            TryMove((Direction)values.GetValue(random.Next(values.Length)));
        }

        public override void Starter()
        {
            MaxHealth = 10;
            ActualHealth = 10;
            Damage = 10;
        }

        public override int DefaultSpriteId => 120;
        public override string DefaultName => "Zombie";
    }
}