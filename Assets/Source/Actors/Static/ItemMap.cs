namespace Source.Actors.Static
{
    public class ItemMap : Actor
    {
        public override int DefaultSpriteId => 751;
        public override string DefaultName => "ItemMap";

        public override bool OnGround => true;

        protected override bool OnCollision(Actor anotherActor)
        {
            return true;
        }
    }
}