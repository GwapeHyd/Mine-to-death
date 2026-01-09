using UnityEngine;

[CreateAssetMenu(fileName = "New AutoTile Sprite Set", menuName = "AutoTile/Sprite Set")]
public class AutoTileSpriteSet : ScriptableObject
{
    [Header("Sprite for special blocks")]
    public Sprite hintFarSprite;
    public Sprite hintNearSprite;
    public Sprite coinBlockSprite;
    public Sprite mineralBlockSprite;
    public Sprite actionFeedbackSprite;
    public Sprite bonusBlockSprite;

    [Header("Full Health Sprites")]
    public Sprite fullSprite;
    public Sprite topSprite;
    public Sprite topInnerLeftSprite;
    public Sprite topInnerRightSprite;
    public Sprite bottomSprite;
    public Sprite bottomInnerLeftSprite;
    public Sprite bottomInnerRightSprite;
    public Sprite leftSprite;
    public Sprite leftInnerBottomSprite;
    public Sprite leftInnerTopSprite;
    public Sprite rightSprite;
    public Sprite rightInnerBottomSprite;
    public Sprite rightInnerTopSprite;
    public Sprite topLeftSprite;
    public Sprite topRightSprite;
    public Sprite bottomLeftSprite;
    public Sprite bottomRightSprite;

    public Sprite innerTopSprite;
    public Sprite innerBottomSprite;
    public Sprite innerLeftSprite;
    public Sprite innerRightSprite;
    public Sprite innerTopLeftSprite;
    public Sprite innerTopRightSprite;
    public Sprite innerBottomLeftSprite;
    public Sprite innerBottomRightSprite;
    public Sprite innerDiagTopLeftSprite;
    public Sprite innerDiagTopRightSprite;

    public Sprite diagTopLeftSprite;
    public Sprite diagTopRightSprite;
    public Sprite diagBottomLeftSprite;
    public Sprite diagBottomRightSprite;

    public Sprite interTopLeftSprite;
    public Sprite interTopRightSprite;
    public Sprite interBottomLeftSprite;
    public Sprite interBottomRightSprite;
    public Sprite interInnerTopLeftSprite;
    public Sprite interInnerTopRightSprite;
    public Sprite interInnerBottomLeftSprite;
    public Sprite interInnerBottomRightSprite;
    public Sprite interInnerTopLeftSprite2;
    public Sprite interInnerTopRightSprite2;  
    public Sprite interInnerBottomLeftSprite2;
    public Sprite interInnerBottomRightSprite2;  

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