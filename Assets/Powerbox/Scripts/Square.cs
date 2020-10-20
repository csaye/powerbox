using System;
using UnityEngine;

namespace Powerbox
{
    public enum SquareType
    {
        Empty,
        Node,
        Wire
    }

    [Serializable]
    public class Square
    {
        public SpriteRenderer spriteRenderer;
        public SquareType type;
        public Color color;

        public Square(SpriteRenderer _spriteRenderer, SquareType _type, Color _color)
        {
            spriteRenderer = _spriteRenderer;
            type = _type;
            color = _color;
        }
    }
}
