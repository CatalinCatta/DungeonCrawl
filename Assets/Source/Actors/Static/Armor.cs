namespace DungeonCrawl.Actors.Static
{
    public class Armor : Actor
    {

        public override int DefaultSpriteId => 79;
        public override string DefaultName => "Armor";
        public override bool OnGround => true;

        public override bool OnCollision(Actor anotherActor)
        {
            return true;
        }

        public override int Z => -1;

    }
}