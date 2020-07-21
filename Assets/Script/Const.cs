
static class Const
{
    public const float SPACE_X = -240f;
    public const float SPACE_Y = -300f;
    public const float FRAME_IMAGE_WIDTH = 80f;
    public const float FRAME_IMAGE_HEIGHT = 90f;

    //public const float BLOCK_MOVE_SPEED = 10f;
    public const float BLOCK_MOVE_TIME = 0.8f;

    //게임
    //public static BlockType[,] MAPDESIGN = new BlockType[,]
    //    {
    //        {BlockType.NONE, BlockType.NONE, BlockType.NONE, BlockType.PURPLE, BlockType.NONE, BlockType.NONE, BlockType.NONE},
    //        {BlockType.NONE, BlockType.ORANGE, BlockType.GREEN, BlockType.RED, BlockType.YELLOW, BlockType.RED, BlockType.NONE},
    //        {BlockType.YELLOW, BlockType.PURPLE, BlockType.TOP, BlockType.YELLOW, BlockType.ORANGE, BlockType.TOP, BlockType.GREEN},
    //        {BlockType.GREEN, BlockType.PURPLE, BlockType.YELLOW, BlockType.BLUE, BlockType.RED, BlockType.PURPLE, BlockType.YELLOW},
    //        {BlockType.BLUE, BlockType.TOP, BlockType.YELLOW, BlockType.PURPLE, BlockType.GREEN, BlockType.PURPLE, BlockType.GREEN},
    //        {BlockType.NONE, BlockType.NONE, BlockType.TOP, BlockType.TOP, BlockType.TOP, BlockType.NONE, BlockType.NONE}
    //    };

    //유튜브
    public static BlockType[,] MAPDESIGN = new BlockType[,]
        {
            {BlockType.NONE, BlockType.NONE, BlockType.NONE, BlockType.BLUE, BlockType.NONE, BlockType.NONE, BlockType.NONE},
            {BlockType.NONE, BlockType.TOP, BlockType.TOP, BlockType.GREEN, BlockType.TOP, BlockType.TOP, BlockType.NONE},
            {BlockType.YELLOW, BlockType.PURPLE, BlockType.GREEN, BlockType.RED, BlockType.BLUE, BlockType.PURPLE, BlockType.YELLOW},
            {BlockType.GREEN, BlockType.PURPLE, BlockType.RED, BlockType.TOP, BlockType.GREEN, BlockType.PURPLE, BlockType.GREEN},
            {BlockType.BLUE, BlockType.TOP, BlockType.YELLOW, BlockType.PURPLE, BlockType.RED, BlockType.TOP, BlockType.BLUE},
            {BlockType.NONE, BlockType.NONE, BlockType.TOP, BlockType.TOP, BlockType.TOP, BlockType.NONE, BlockType.NONE}
        };

    //테스트 데이터
    //public static BlockType[,] MAPDESIGN = new BlockType[,]
    //{
    //        {BlockType.NONE, BlockType.NONE, BlockType.NONE, BlockType.RED, BlockType.NONE, BlockType.NONE, BlockType.NONE},
    //        {BlockType.NONE, BlockType.YELLOW, BlockType.GREEN, BlockType.PURPLE, BlockType.YELLOW, BlockType.RED, BlockType.NONE},
    //        {BlockType.PURPLE, BlockType.TOP, BlockType.PURPLE, BlockType.BLUE, BlockType.PURPLE, BlockType.PURPLE, BlockType.GREEN},
    //        {BlockType.TOP, BlockType.GREEN, BlockType.PURPLE, BlockType.YELLOW, BlockType.YELLOW, BlockType.GREEN, BlockType.YELLOW},
    //        {BlockType.GREEN, BlockType.BLUE, BlockType.BLUE, BlockType.TOP, BlockType.BLUE, BlockType.GREEN, BlockType.GREEN},
    //        {BlockType.NONE, BlockType.NONE, BlockType.BLUE, BlockType.TOP, BlockType.PURPLE, BlockType.NONE, BlockType.NONE}
    //};

    public static int MAPSIZE_X = MAPDESIGN.GetLength(1);//7
    public static int MAPSIZE_Y = MAPDESIGN.GetLength(0);//6
    public static Index ENTRANCE_INDEX = new Index(MAPSIZE_X / 2, MAPSIZE_Y - 1);
    public static Index ENTRANCE_UP_INDEX = new Index(ENTRANCE_INDEX.X, ENTRANCE_INDEX.Y + 1);
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



