namespace Source.Actors.Static
{
    public class ItemSword : Actor
    {
        public override int DefaultSpriteId => 371;
        public override string DefaultName => "ItemSword";

        public override bool OnGround => true;

        protected override bool OnCollision(Actor anotherActor)
        {
            return true;
        }
    }
}