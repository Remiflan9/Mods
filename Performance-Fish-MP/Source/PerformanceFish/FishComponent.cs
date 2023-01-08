namespace PerformanceFish
{
    public class FishComponent : GameComponent
    {
        public FishComponent(Game game)
        {

        }

        public override void FinalizeInit()
        {
            Cache.Utility.Clear();
            Log.Message("Performance Fish :: Cache cleared");
        }
    }
}