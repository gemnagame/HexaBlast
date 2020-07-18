
static class Const
{
    public const float SPACE_X = 90;
    public const float SPACE_Y = 300;

    public const float BLOCK_MOVE_SPEED = 100f;
}

public enum BlockType
{
    NONE = 0,
    RED,
    ORANGE,
    YELLOW,
    GREEN,
    BLUE,
    PURPLE,
    TOP
}

public enum Direction
{
    UP = 0,
    DOWN,
    LEFTUP,
    LEFTDOWN,
    RIGHTUP,
    RIGHTDOWN
}

public struct Index
{
    public Index(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X;
    public int Y;
}



