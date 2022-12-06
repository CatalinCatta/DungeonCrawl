namespace Source.Actors.Characters
{
    public class DogCompanion : Actor
    {
        public override int DefaultSpriteId => 366;
        public override string DefaultName => "DogCompanion";

        public override bool Detectable => false;

        protected override int Z => -1;
    }
}