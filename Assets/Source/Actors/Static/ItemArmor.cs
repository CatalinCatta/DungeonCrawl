namespace Source.Actors.Static
{
    public class ItemArmor : Actor
    {
        public override int DefaultSpriteId => 79;
        public override string DefaultName => "ItemArmor";
        public override bool OnGround => true;

        protected override bool OnCollision(Actor anotherActor)
        {
            return true;
        }

        protected override int Z => -1;
    }
}