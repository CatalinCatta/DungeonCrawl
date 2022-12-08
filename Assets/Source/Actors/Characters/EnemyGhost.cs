using System;
using System.Collections;
using Source.Core;
using UnityEngine;

namespace Source.Actors.Characters
{
    public class EnemyGhost : Character
    {
        public int curse;
        public (int x, int y) GravePosition;

        protected override bool OnCollision(Actor anotherActor)
        {
            return true;
        }

        protected override void OnDeath()
        {
            Debug.Log("Well, I was already dead anyway...");
        }

        protected override void Hit(Actor actor)
        {
            if (!(actor is Player player)) return;
            curse++;
            player.ApplyDamage(Damage);
            Console.WriteLine();
        }

        private IEnumerator Start()
        {
            while (true)
            {
                if (ActualHealth <= 0)
                {
                    break;
                }

                yield return new WaitForSeconds(0.3f);
                RandomPosition();
                yield return new WaitForSeconds(0.3f);
                RandomPosition();
                yield return new WaitForSeconds(0.3f);
                RandomPosition();
                if (curse > 0)
                {
                    ActorManager.Singleton.GetActor<Player>().ApplyDamage(curse, "Curse");
                }
            }
        }

        private void RandomPosition()
        {
            var values = Enum.GetValues(typeof(Direction));
            var random = new System.Random();
            var direction = (Direction)values.GetValue(random.Next(values.Length));
            TryMove(direction);
        }

        public override void Starter()
        {
            MaxHealth = 5;
            ActualHealth = 5;
            Damage = 3;
        }

        protected override void TryMove(Direction direction)
        {
            var vector = direction.ToVector();
            (int x, int y) targetPosition = (Position.x + vector.x, Position.y + vector.y);

            var actorAtTargetPosition = ActorManager.Singleton.GetActorAt(targetPosition);

            if (targetPosition.x > GravePosition.x + 6 || targetPosition.x < GravePosition.x - 6 ||
                targetPosition.y > GravePosition.y + 6 || targetPosition.y < GravePosition.y - 6) return;
            Position = targetPosition;
            Hit(actorAtTargetPosition);
        }

        public override int DefaultSpriteId => 314;
        public override string DefaultName => "EnemyGhost";
    }
}