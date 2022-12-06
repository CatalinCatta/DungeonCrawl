namespace Source.Actors.Characters
{
    public class Grave : Character
    {
        public Ghost ghost;

        protected override bool OnCollision(Actor anotherActor)
        {
            return false;
        }

        protected override void OnDeath()
        {
            if (ghost != null && ghost.ActualHealth > 0)
            {
                ghost.ApplyDamage(Damage);
            }
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
        public override string DefaultName => "Grave";
    }
}