namespace ArkanoidGame
{
    /// <summary> Глобальные настройки игры и константы </summary>
    public static class GameSettings
    {
        public const int BlockWidth = 70;
        public const int BlockHeight = 30;
        public const int BlockPadding = 5;
        public const int GridRows = 5;
        public const int GridCols = 10;
        public const int GridOffsetX = 40;
        public const int GridOffsetY = 60;
        public const int PaddleOffsetDivisor = 7;
        public const string PowerUpTag = "buff";
        public const int BallStartSpeedX = 4;
        public const int BallStartSpeedY = -4;
        public const int MaxBallSpeedX = 7;
        public const int MinBallSpeedY = 3;
        public const int BonusSize = 15;
        public const int BonusFallSpeed = 4;
        public const int BonusChancePercent = 20;
        public const int MaxBallDamage = 3;
        public const int TimerInterval = 20;
    }
}