namespace Source.Actors.Static
{
    public class Key : Actor
    {
        public override int DefaultSpriteId => 559;
        public override string DefaultName => "Key";
        public override bool OnGround => true;

        protected override bool OnCollision(Actor anotherActor)
        {
            return true;
        }

        protected override int Z => -1;
    }
}