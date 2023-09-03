using UnityEngine;

public class Block : MonoBehaviour
{
    [HideInInspector] public bool Blocked;
    [HideInInspector] public bool Filled;

    [SerializeField] private SpriteRenderer _blockSprite;
    [SerializeField] private Color _blockedColor;
    [SerializeField] private Color _emptyColor;
    [SerializeField] private Color _activeColor;

    public void Init(int fill)
    {
        Blocked = fill == 1;
        Filled = Blocked;
        _blockSprite.color = Blocked ? _blockedColor : _emptyColor;
    }

    public void Add()
    {
        Filled = true;
        _blockSprite.color = _activeColor;
    }
}
