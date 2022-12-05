namespace DungeonCrawl.Actors.Static
{
    public class DogCompanion : Actor
    {
        public override int DefaultSpriteId => 366;
        public override string DefaultName => "DogCompanion";

        public override bool Detectable => false;

        public override int Z => -1;
    }
}