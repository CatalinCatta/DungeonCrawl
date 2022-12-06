namespace Source.Actors.Static
{
    public class Heal : Actor
    {
        public override int DefaultSpriteId => 568;
        public override string DefaultName => "Heal";

        public override bool OnGround => true;

        protected override bool OnCollision(Actor anotherActor)
        {
            return true;
        }

        protected override int Z => -1;
    }
}