public static class SaveKeys
{
    public static string BestScore(int level) => "Level" + level + "_BestScore";
    public static string BestTime(int level) => "Level" + level + "_BestTime";
    public static string BestGrade(int level) => "Level" + level + "_BestGrade";
    public static string LevelUnlocked(int level) => "Level" + level + "_Unlocked";
    public static string LevelCompleted(int level) => "Level" + level + "_Completed";

    public const string CheckpointX = "Checkpoint_X";
    public const string CheckpointY = "Checkpoint_Y";
    public const string CheckpointZ = "Checkpoint_Z";
    public const string HasCheckpoint = "HasCheckpoint";
}