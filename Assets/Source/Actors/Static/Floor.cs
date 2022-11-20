namespace DungeonCrawl.Actors.Static
{
    public class Floor : Actor
    {
        public override int DefaultSpriteId => 1;
        public override string DefaultName => "Floor";

        public override bool Detectable => false;
        
        public override int Z => 1;

    }
}
