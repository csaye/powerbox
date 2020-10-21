using System;
using System.Collections;
using UnityEngine;

namespace Powerbox
{
    enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    public class Gameboard : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Square[] squares = new Square[64];
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private Sprite[] nodeSprites = new Sprite[16];
        [SerializeField] private Sprite[] wireSprites = new Sprite[10];

        // Returns square index based on current mouse position
        private int GetSquareIndex()
        {
            // Parse screen position to translated index position
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int xIndex = Mathf.FloorToInt(mousePosition.x + 4);
            int yIndex = Mathf.FloorToInt(-mousePosition.y + 4);
            // Return -1 if out of bounds
            if (xIndex < 0 || xIndex > 7) return -1;
            if (yIndex < 0 || yIndex > 7) return -1;
            return xIndex + (8 * yIndex);
        }

        private void OnMouseDown()
        {
            StartCoroutine(Drag());
        }

        private IEnumerator Drag()
        {
            int currentSquareIndex = GetSquareIndex();
            int lastSquareIndex = currentSquareIndex;

            // If out of bounds, return
            if (currentSquareIndex == -1) yield break;

            // Get square color and check if valid starting position
            Color dragColor = squares[currentSquareIndex].color;
            if (!CanStartDrag(currentSquareIndex, dragColor)) yield break;

            // While left mouse button pressed
            while (Input.GetMouseButton(0))
            {
                currentSquareIndex = GetSquareIndex();
                // If out of bounds, return
                if (currentSquareIndex == -1) yield break;
                // If dragged into new square, parse
                if (currentSquareIndex != lastSquareIndex)
                {
                    // If dragged onto nonempty space, return
                    if (!IsSquareType(currentSquareIndex, SquareType.Empty)) yield break;
                    // Update connected squares
                    squares[lastSquareIndex].connectedSquares.Add(currentSquareIndex);
                    squares[currentSquareIndex].connectedSquares.Add(lastSquareIndex);
                    // Set wire
                    SetWire(currentSquareIndex, dragColor, lastSquareIndex);
                    lastSquareIndex = currentSquareIndex;
                }
                yield return null;
            }
        }

        // Sets square at given index to given type and color
        private void SetWire(int index, Color color, int lastIndex)
        {
            // Get square, set color and type
            Square square = squares[index];
            square.color = color;
            square.type = SquareType.Wire;
            square.color = color;
            // Set wire sprite
            square.sprite = GetSprite(index);
            // Update last square sprite
            Square lastSquare = squares[lastIndex];
            lastSquare.sprite = GetSprite(lastIndex);
        }

        // Returns corresponding sprite for square index
        private Sprite GetSprite(int index)
        {
            Square square = squares[index];
            // Return null if square empty
            if (square.type == SquareType.Empty) return null;
            Color squareColor = square.color;

            bool wireUp = IsConnected(index, Direction.Up, squareColor);
            bool wireRight = IsConnected(index, Direction.Right, squareColor);
            bool wireDown =  IsConnected(index, Direction.Down, squareColor);
            bool wireLeft = IsConnected(index, Direction.Left, squareColor);

            switch (square.type)
            {
                case SquareType.Node:
                    return GetNodeSprite(wireUp, wireRight, wireDown, wireLeft);
                case SquareType.Wire:
                    return GetWireSprite(wireUp, wireRight, wireDown, wireLeft);
            }
            return null;
        }

        // Returns corresponsing node sprite with given connections
        private Sprite GetNodeSprite(bool wireUp, bool wireRight, bool wireDown, bool wireLeft)
        {
            if (wireUp && wireRight && wireDown && wireLeft) return nodeSprites[15];
            if (wireUp && wireRight && wireDown) return nodeSprites[14];
            if (wireRight && wireDown && wireLeft) return nodeSprites[13];
            if (wireDown && wireLeft && wireUp) return nodeSprites[12];
            if (wireLeft && wireUp && wireRight) return nodeSprites[11];
            if (wireUp && wireRight) return nodeSprites[10];
            if (wireRight && wireDown) return nodeSprites[9];
            if (wireDown && wireLeft) return nodeSprites[8];
            if (wireLeft && wireUp) return nodeSprites[7];
            if (wireUp && wireDown) return nodeSprites[6];
            if (wireLeft && wireRight) return nodeSprites[5];
            if (wireUp) return nodeSprites[4];
            if (wireRight) return nodeSprites[3];
            if (wireDown) return nodeSprites[2];
            if (wireLeft) return nodeSprites[1];
            return nodeSprites[0];
        }

        // Returns corresponsing wire sprite with given connections
        private Sprite GetWireSprite(bool wireUp, bool wireRight, bool wireDown, bool wireLeft)
        {
            if (wireUp && wireRight) return wireSprites[9];
            if (wireRight && wireDown) return wireSprites[8];
            if (wireDown && wireLeft) return wireSprites[7];
            if (wireLeft && wireUp) return wireSprites[6];
            if (wireUp && wireDown) return wireSprites[5];
            if (wireLeft && wireRight) return wireSprites[4];
            if (wireUp) return wireSprites[3];
            if (wireRight) return wireSprites[2];
            if (wireDown) return wireSprites[1];
            if (wireLeft) return wireSprites[0];
            return null;   
        }

        // Returns whether a drag can begin from given square index
        private bool CanStartDrag(int index, Color color)
        {
            // Return whether square is node or open wire
            return IsSquareType(index, SquareType.Node) || IsOpenWire(index, color);
        }

        // Returns whether given index is that of an open wire
        private bool IsOpenWire(int index, Color color)
        {
            // Return if not wire
            if (!IsSquareType(index, SquareType.Wire)) return false;
            Square square = squares[index];
            return Array.IndexOf(wireSprites, square.spriteRenderer.sprite) <= 3;
        }

        // Return if square at index is type
        private bool IsSquareType(int index, SquareType type) => squares[index].type == type;

        // Return if square is connected in given direction
        private bool IsConnected(int index, Direction direction, Color color)
        {
            Square square = squares[index];
            int adjacent = -1;
            // Return if out of bounds, otherwise get square
            switch (direction)
            {
                case Direction.Up:
                    if (index < 8) return false;
                    adjacent = index - 8;
                    break;
                case Direction.Right:
                    if (index % 8 == 7) return false;
                    adjacent = index + 1;
                    break;
                case Direction.Down:
                    if (index > 55) return false;
                    adjacent = index + 8;
                    break;
                case Direction.Left:
                    if (index % 8 == 0) return false;
                    adjacent = index - 1;
                    break;
            }
            // Return whether square matches color connector
            return square.connectedSquares.Contains(adjacent);
        }
    }
}
