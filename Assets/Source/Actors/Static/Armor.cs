namespace Source.Actors.Static
{
    public class Armor : Actor
    {
        public override int DefaultSpriteId => 79;
        public override string DefaultName => "Armor";
        public override bool OnGround => true;

        protected override bool OnCollision(Actor anotherActor)
        {
            return true;
        }

        protected override int Z => -1;
    }
}