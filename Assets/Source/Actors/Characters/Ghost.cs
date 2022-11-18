using UnityEngine;
using System.Collections;
using DungeonCrawl.Core;
using DungeonCrawl.Actors.Static;
using Assets.Source.Core;
using System;

namespace DungeonCrawl.Actors.Characters
{
    public class Ghost : Character
    {
        private int Curse = 0;
        public (int x, int y) GravePosition;

        public override bool OnCollision(Actor anotherActor)
        {
            return true;
        }

        protected override void OnDeath()
        {
            Debug.Log("Well, I was already dead anyway...");
        }

        public override void Hit(Actor actor)
        {
            if (actor is Player player)
            {
                Curse++;
                player.ApplyDamage(Damage);
            }
        }

        IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.3f);
                RandomPosition();
                ActorManager.Singleton.GetActor<Player>().ApplyDamage(Curse, "Curse");
            }
        }

        public void RandomPosition()
        {
            Array values = Enum.GetValues(typeof(Direction));
            System.Random random = new System.Random();
            var direction = (Direction)values.GetValue(random.Next(values.Length));
            TryMove(direction);
            
        }

        public override void Starter()
        {
            MaxHealth = 5;
            ActualHealth = 5;
            Damage = 3;
        }

        public override void TryMove(Direction direction)
        {
            var vector = direction.ToVector();
            (int x, int y) targetPosition = (Position.x + vector.x, Position.y + vector.y);

            var actorAtTargetPosition = ActorManager.Singleton.GetActorAt(targetPosition);

            if (targetPosition.x <= GravePosition.x + 6 && targetPosition.x >= GravePosition.x - 6 &&
                targetPosition.y <= GravePosition.y + 6 && targetPosition.y >= GravePosition.y - 6)
            {
                Position = targetPosition;
                Hit(actorAtTargetPosition);
            }
        }

        public override int DefaultSpriteId => 314;
        public override string DefaultName => "Ghost";
    }
}