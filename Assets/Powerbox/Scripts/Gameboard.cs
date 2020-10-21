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
        // [SerializeField] private Sprite nodeSprite = null, nodeUpSprite = null, nodeRightSprite = null, nodeDownSprite = null, nodeLeftSprite = null;
        // [SerializeField] private Sprite wireVerticalSprite = null, wireHorizontalSprite = null, wireUpSprite = null, wireRightSprite = null, wireDownSprite = null, wireLeftSprite = null;
        // [SerializeField] private Sprite openWireUpSprite = null, openWireRightSprite = null, openWireDownSprite = null, openWireLeftSprite = null;
        [SerializeField] private Sprite wireSprite = null;

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
                    lastSquareIndex = currentSquareIndex;
                    // If dragged onto nonempty space, return
                    if (!IsSquareType(currentSquareIndex, SquareType.Empty)) yield break;
                    // Otherwise set wire
                    SetWire(currentSquareIndex, dragColor);
                }
                yield return null;
            }
        }

        // Sets square at given index to given type and color
        private void SetWire(int index, Color color)
        {
            // Get square, set color and type
            Square square = squares[index];
            square.color = color;
            square.type = SquareType.Wire;
            SpriteRenderer squareRenderer = square.spriteRenderer;
            squareRenderer.color = color;
            // Get wire sprite based on surrounding wires
            Sprite sprite;
            
            sprite = wireSprite;

            squareRenderer.sprite = sprite;
        }

        // Returns whether a drag can begin from given square index
        private bool CanStartDrag(int index, Color color)
        {
            Square square = squares[index];
            // Return false if empty square
            if (square.type == SquareType.Empty) return false;
            // Return true if wire open
            if (square.type == SquareType.Wire)
            {
                int adjacentWires = 0;
                if (IsColorWire(index, Direction.Up, color)) adjacentWires++;
                if (IsColorWire(index, Direction.Right, color)) adjacentWires++;
                if (IsColorWire(index, Direction.Down, color)) adjacentWires++;
                if (IsColorWire(index, Direction.Left, color)) adjacentWires++;
                return adjacentWires == 1;
            }
            return true;
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
