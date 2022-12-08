using System;
using System.Collections;
using UnityEngine;

namespace Source.Actors.Characters
{
    public class EnemyZombie : Character
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
            if (actor is Player player)
            {
                player.ApplyDamage(Damage);
            }
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
                RandomPosition();
                HitEnemy();
                Heal(1);
            }
        }

        private void RandomPosition()
        {
            var values = Enum.GetValues(typeof(Direction));
            var random = new System.Random();
            TryMove((Direction)values.GetValue(random.Next(values.Length)));
        }

        public override void Starter()
        {
            MaxHealth = 10;
            ActualHealth = 10;
            Damage = 10;
        }

        public override int DefaultSpriteId => 120;
        public override string DefaultName => "EnemyZombie";
    }
}