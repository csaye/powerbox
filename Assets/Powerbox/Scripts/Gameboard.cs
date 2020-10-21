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
                    Debug.Log(currentSquareIndex);
                    // If dragged onto nonempty space, return
                    if (!IsSquareType(currentSquareIndex, SquareType.Empty)) yield break;
                    // Otherwise set wire
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
            SpriteRenderer squareRenderer = square.spriteRenderer;
            squareRenderer.color = color;
            // Set wire sprite
            squareRenderer.sprite = GetSprite(index);
            // Update last square sprite
            Square lastSquare = squares[lastIndex];
            lastSquare.spriteRenderer.sprite = GetSprite(lastIndex);
        }

        // Returns corresponding sprite for square index
        private Sprite GetSprite(int index)
        {
            Square square = squares[index];
            Color squareColor = square.color;
            // Return null if square empty
            if (square.type == SquareType.Empty) return null;
            bool wireUp = IsColorWire(index, Direction.Up, squareColor);
            bool wireRight = IsColorWire(index, Direction.Right, squareColor);
            bool wireDown =  IsColorWire(index, Direction.Down, squareColor);
            bool wireLeft = IsColorWire(index, Direction.Left, squareColor);
            switch (square.type)
            {
                case SquareType.Node:
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
                case SquareType.Wire:
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
            // Check all wire directions
            int adjacentWires = 0;
            if (IsColorWire(index, Direction.Up, color)) adjacentWires++;
            if (IsColorWire(index, Direction.Right, color)) adjacentWires++;
            if (IsColorWire(index, Direction.Down, color)) adjacentWires++;
            if (IsColorWire(index, Direction.Left, color)) adjacentWires++;
            return adjacentWires == 1;
        }

        // Return if square at index is type
        private bool IsSquareType(int index, SquareType type) => squares[index].type == type;

        // Return if square in direction is wire of given color
        private bool IsColorWire(int index, Direction direction, Color color)
        {
            // Return if out of bounds, otherwise get square
            Square square = null;
            switch (direction)
            {
                case Direction.Up:
                    if (index < 8) return false;
                    square = squares[index - 8];
                    break;
                case Direction.Right:
                    if (index % 8 == 7) return false;
                    square = squares[index + 1];
                    break;
                case Direction.Down:
                    if (index > 55) return false;
                    square = squares[index + 8];
                    break;
                case Direction.Left:
                    if (index % 8 == 0) return false;
                    square = squares[index - 1];
                    break;
            }
            // Return whether square matches color wire
            return square.type == SquareType.Wire && square.color == color;
        }
    }
}
