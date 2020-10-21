using System.Collections.Generic;
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
        public Color color
        {
            get { return spriteRenderer.color; }
            set { spriteRenderer.color = value; }
        }
        public Sprite sprite
        {
            get { return spriteRenderer.sprite; }
            set { spriteRenderer.sprite = value; }
        }
        public List<int> connectedSquares = new List<int>();

        public Square(SpriteRenderer _spriteRenderer, SquareType _type, Color _color)
        {
            spriteRenderer = _spriteRenderer;
            type = _type;
            color = _color;
        }
    }
}
