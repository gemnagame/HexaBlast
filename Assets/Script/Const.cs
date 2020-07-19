
static class Const
{
    public const float SPACE_X = 90;
    public const float SPACE_Y = 300;
    public const float FRAME_IMAGE_WIDTH = 90f;
    public const float FRAME_IMAGE_HEIGHT = 90f;

    public const float BLOCK_MOVE_SPEED = 10f;

    //public static BlockType[,] MAPDESIGN = new BlockType[,]
    //    { 
    //        {BlockType.NONE, BlockType.NONE, BlockType.NONE, BlockType.PURPLE, BlockType.NONE, BlockType.NONE, BlockType.NONE},
    //        {BlockType.NONE, BlockType.ORANGE, BlockType.GREEN, BlockType.RED, BlockType.YELLOW, BlockType.RED, BlockType.NONE},
    //        {BlockType.YELLOW, BlockType.PURPLE, BlockType.TOP, BlockType.YELLOW, BlockType.ORANGE, BlockType.TOP, BlockType.GREEN},
    //        {BlockType.GREEN, BlockType.PURPLE, BlockType.YELLOW, BlockType.BLUE, BlockType.RED, BlockType.PURPLE, BlockType.YELLOW},
    //        {BlockType.BLUE, BlockType.TOP, BlockType.YELLOW, BlockType.PURPLE, BlockType.GREEN, BlockType.PURPLE, BlockType.GREEN},
    //        {BlockType.NONE, BlockType.NONE, BlockType.TOP, BlockType.TOP, BlockType.TOP, BlockType.NONE, BlockType.NONE}
    //    };
    public static BlockType[,] MAPDESIGN = new BlockType[,]
    {
            {BlockType.NONE, BlockType.NONE, BlockType.NONE, BlockType.RED, BlockType.NONE, BlockType.NONE, BlockType.NONE},
            {BlockType.NONE, BlockType.YELLOW, BlockType.GREEN, BlockType.PURPLE, BlockType.YELLOW, BlockType.RED, BlockType.NONE},
            {BlockType.PURPLE, BlockType.TOP, BlockType.PURPLE, BlockType.BLUE, BlockType.PURPLE, BlockType.PURPLE, BlockType.GREEN},
            {BlockType.TOP, BlockType.GREEN, BlockType.PURPLE, BlockType.YELLOW, BlockType.YELLOW, BlockType.GREEN, BlockType.YELLOW},
            {BlockType.GREEN, BlockType.BLUE, BlockType.BLUE, BlockType.TOP, BlockType.BLUE, BlockType.GREEN, BlockType.GREEN},
            {BlockType.NONE, BlockType.NONE, BlockType.BLUE, BlockType.TOP, BlockType.PURPLE, BlockType.NONE, BlockType.NONE}
    };

    public static int MAPSIZE_X = MAPDESIGN.GetLength(1);//7
    public static int MAPSIZE_Y = MAPDESIGN.GetLength(0);//6
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



