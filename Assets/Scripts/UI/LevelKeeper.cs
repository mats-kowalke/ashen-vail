public class LevelKeeper
{
    public static float levelScore;

    public static void SetLevelScore(float newScore)
    {
        if (levelScore < newScore)
        {
            levelScore = newScore;
        }
    }
}