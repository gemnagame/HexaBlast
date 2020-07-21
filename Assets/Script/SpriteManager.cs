using UnityEngine;
using UnityEngine.UI;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance = null;

    public Sprite block_red;
    public Sprite block_orange;
    public Sprite block_yellow;
    public Sprite block_green;
    public Sprite block_blue;
    public Sprite block_purple;

    public Sprite block_top;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
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
