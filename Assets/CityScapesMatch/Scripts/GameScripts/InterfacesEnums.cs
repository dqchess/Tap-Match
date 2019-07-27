namespace Mkey
{
    public enum GameMode { Play, Edit }
    public enum MatchBoardState { ShowEstimate, Fill, Collect, Waiting, Iddle }
    public enum SpawnerStyle { AllEnabled, AllEnabledAlign, DisabledAligned }
    public enum FillType { Step, Fast }
    public enum BombDir { Vertical, Horizontal, Radial, Color}
    public enum HardMode { Easy, Hard }
    public enum MatchGroupType {Simple, HorBomb, VertBomb, Bomb, ColorBomb}
    public enum BombCombine { ColorBombAndColorBomb, BombAndBomb, RocketAndRocket, ColorBombAndBomb, BombAndRocket,  ColorBombAndRocket, None }
}