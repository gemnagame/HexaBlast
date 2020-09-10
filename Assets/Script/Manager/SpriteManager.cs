using UnityEngine;
using UnityEngine.UI;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance = null;

    [SerializeField]
    Sprite block_red = null;
    [SerializeField]
    Sprite block_orange = null;
    [SerializeField]
    Sprite block_yellow = null;
    [SerializeField]
    Sprite block_green = null;
    [SerializeField]
    Sprite block_blue = null;
    [SerializeField]
    Sprite block_purple = null;

    [SerializeField]
    Sprite block_top = null;

    void Awake()
    {
        Instance = this;        
    }

    public Sprite GetBlcokSprite(BlockType blockType)
    {
        Sprite sprite = null;

        switch(blockType)
        {
            case BlockType.RED:
                {
                    sprite = block_red;
                    break;
                }
            case BlockType.ORANGE:
                {
                    sprite = block_orange;
                    break;
                }
            case BlockType.YELLOW:
                {
                    sprite = block_yellow;
                    break;
                }
            case BlockType.GREEN:
                {
                    sprite = block_green;
                    break;
                }
            case BlockType.BLUE:
                {
                    sprite = block_blue;
                    break;
                }
            case BlockType.PURPLE:
                {
                    sprite = block_purple;
                    break;
                }
            case BlockType.TOP:
                {
                    sprite = block_top;
                    break;
                }
        }

        return sprite;
    }
}
