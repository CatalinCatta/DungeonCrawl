namespace Source.Actors.Characters
{
    public class EnemyGrave : Character
    {
        public EnemyGhost enemyGhost;

        protected override bool OnCollision(Actor anotherActor)
        {
            return false;
        }

        protected override void OnDeath()
        {
            if (enemyGhost == null || enemyGhost.ActualHealth <= 0) return;
            enemyGhost.curse = 0;
            enemyGhost.ApplyDamage(Damage);
        }

        protected override void Drop()
        {
        }

        protected override void Hit(Actor actor)
        {
        }

        public override void Starter()
        {
            MaxHealth = 20;
            ActualHealth = 20;
            Damage = 999;
        }

        public override int DefaultSpriteId => 672;
        public override string DefaultName => "EnemyGrave";
    }
}