using UnityEngine;

[CreateAssetMenu(fileName = "New AutoTile Sprite Set", menuName = "AutoTile/Sprite Set")]
public class AutoTileSpriteSet : ScriptableObject
{
    [Header("Full Health Sprites")]
    public Sprite fullSprite;
    public Sprite topSprite;
    public Sprite bottomSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite topLeftSprite;
    public Sprite topRightSprite;
    public Sprite bottomLeftSprite;
    public Sprite bottomRightSprite;
    public Sprite horizontalSprite;
    public Sprite verticalSprite;
    public Sprite borderLeftSprite;
    public Sprite borderRightSprite;
    public Sprite borderTopSprite;
    public Sprite borderBottomSprite;
    public Sprite isolatedSprite;

    [Header("Damaged Sprites")]
    public Sprite damagedSprite;
}