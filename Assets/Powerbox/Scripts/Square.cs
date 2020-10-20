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

    public enum SquareColor
    {
        None,
        Red,
        Green,
        Blue
    }

    [Serializable]
    public class Square
    {
        public SpriteRenderer spriteRenderer;
        public SquareType type;
        public SquareColor color;

        public Square(SpriteRenderer _spriteRenderer, SquareType _type, SquareColor _color)
        {
            spriteRenderer = _spriteRenderer;
            type = _type;
            color = _color;
        }
    }
}
