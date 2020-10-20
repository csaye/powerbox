using System.Collections;
using UnityEngine;

namespace Powerbox
{
    public class Gameboard : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Square[] squares = new Square[64];
        [SerializeField] private Camera mainCamera = null;

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
            int lastSquareIndex = -1;

            // If out of bounds or not starter square, return
            if (currentSquareIndex == -1 || !CanStartDrag(currentSquareIndex)) yield break;
            
            Color dragColor = squares[currentSquareIndex].color;

            // While left mouse button pressed
            while (Input.GetMouseButton(0))
            {
                currentSquareIndex = GetSquareIndex();
                // If out of bounds, return
                if (currentSquareIndex == -1) yield break;
                // If dragged into new square, parse
                if (currentSquareIndex != lastSquareIndex)
                {
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
            // Get square and set color
            Square square = squares[index];
            square.color = color;
            SpriteRenderer squareRenderer = square.spriteRenderer;
            // spriteRenderer.color = color;
            // Get wire sprite based on surrounding wires
            // Sprite sprite;
            
            // sprite = wireHorizontalSprite;

            // spriteRenderer.sprite = sprite;
        }

        // Returns whether a drag can begin from given square index
        private bool CanStartDrag(int index)
        {
            Square square = squares[index];
            // Return false if empty square
            if (square.type == SquareType.Empty) return false;
            // Return true if wire open
            if (square.type == SquareType.Wire)
            {

            }
            return true;
        }

        // Return if square at index is type
        private bool IsSquareType(int index, SquareType type) => squares[index].type == type;

        // Return if square above is wire of given color
        // private bool IsColorWire
    }
}
