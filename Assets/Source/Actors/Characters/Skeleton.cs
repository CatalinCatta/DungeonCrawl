using System.Collections;
using Source.Actors.Static;
using Source.Core;
using UnityEngine;

namespace Source.Actors.Characters
{
    public class Skeleton : Character
    {
        protected override bool OnCollision(Actor anotherActor)
        {
            return false;
        }

        protected override void OnDeath()
        {
            Debug.Log("Well, I was already dead anyway...");
        }

        protected override void Hit(Actor actor)
        {
            if (!(actor is Player player)) return;

            if (Position == (6, -8))
            {
                UserInterface.Singleton.SetText("Press SPACE for combat", UserInterface.TextPosition.TopRight);
            }

            player.ApplyDamage(Damage);
        }

        private IEnumerator Start()
        {
            while (true)
            {
                if (ActualHealth <= 0)
                {
                    break;
                }

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

        protected override void Drop()
        {
            if (Position == (6, -8))
            {
                ActorManager.Singleton.Spawn<Key>(Position);
            }
            else
            {
                var rand = new System.Random();
                var dropRate = rand.Next(100);
                if (dropRate < 20)
                {
                    ActorManager.Singleton.Spawn<Heal>(Position);
                }

                if (dropRate == 20)
                {
                    ActorManager.Singleton.Spawn<Meat>(Position);
                }
            }
        }

        public override int DefaultSpriteId => 316;
        public override string DefaultName => "Skeleton";
    }
}