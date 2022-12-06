namespace Source.Actors.Static
{
    public class Meat : Actor
    {
        public override int DefaultSpriteId => 800;
        public override string DefaultName => "Meat";

        public override bool OnGround => true;

        protected override bool OnCollision(Actor anotherActor)
        {
            return true;
        }

        protected override int Z => -1;
    }
}