namespace Source.Actors.Static
{
    public class Ring1 : Actor
    {
        public override int DefaultSpriteId => 330;
        public override string DefaultName => "Ring1";

        public override bool OnGround => true;

        protected override bool OnCollision(Actor anotherActor)
        {
            return true;
        }
    }
}