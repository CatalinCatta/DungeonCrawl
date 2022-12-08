namespace Source.Actors.Static
{
    public class ItemMeat : Actor
    {
        public override int DefaultSpriteId => 800;
        public override string DefaultName => "ItemMeat";

        public override bool OnGround => true;

        protected override bool OnCollision(Actor anotherActor)
        {
            return true;
        }

        protected override int Z => -1;
    }
}