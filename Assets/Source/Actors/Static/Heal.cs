namespace DungeonCrawl.Actors.Static
{
    public class Heal : Actor
    {
        public override int DefaultSpriteId => 568;
        public override string DefaultName => "Heal";
        
        public override bool OnGround => true;

        public override bool OnCollision(Actor anotherActor)
        {
            return true;
        }

        public override int Z => -1;

    }
}