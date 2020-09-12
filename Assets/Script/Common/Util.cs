using UnityEngine;

static class Util
{
    public static Index CalcIndex(Index index, Direction direction)
    {
        switch (direction)
        {
            case Direction.UP:
                {
                    index.Y += 1;
                    break;
                }
            case Direction.DOWN:
                {
                    index.Y -= 1;
                    break;
                }
            case Direction.LEFTUP:
                {
                    index.Y += (index.X % 2 == 0 ? 1 : 0);
                    index.X -= 1;
                    break;
                }
            case Direction.LEFTDOWN:
                {
                    index.Y -= (index.X % 2 == 0 ? 0 : 1);
                    index.X -= 1;
                    break;
                }
            case Direction.RIGHTUP:
                {
                    index.Y += (index.X % 2 == 0 ? 1 : 0);
                    index.X += 1;

                    break;
                }
            case Direction.RIGHTDOWN:
                {
                    index.Y -= (index.X % 2 == 0 ? 0 : 1);
                    index.X += 1;
                    break;
                }
        }

        return index;
    }

    public static Vector3 CalcPositionByIndex(int indexX, int indexY)
    {
        float addPosY = indexX % 2 == 0 ? Const.FRAME_IMAGE_HEIGHT / 2 : 0;
        Vector3 position = new Vector3(
            indexX * Const.FRAME_IMAGE_WIDTH + Const.SPACE_X,
            indexY * Const.FRAME_IMAGE_HEIGHT + Const.SPACE_Y + addPosY,
            0);

        return position;
    }

    public static Vector3 CalcPositionByIndex(Index index)
    {
        return CalcPositionByIndex(index.X, index.Y);
    }

    public static bool IsOutOfIndex(Index index, int arraySizeX, int arraySizeY)
    {
        if (index.X < 0 || index.X >= arraySizeX ||
            index.Y < 0 || index.Y >= arraySizeY)
        {
            return true;
        }

        return false;
    }

    public static BlockType GetRandomBlockType()
    {
        return (BlockType)Random.Range((int)(BlockType.NONE) + 1, (int)(BlockType.MAX));
    }
}
