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
            if (currentSquareIndex == -1 || IsSquareEmpty(currentSquareIndex) || IsSquareWire(currentSquareIndex)) yield break;
            
            // While left mouse button pressed
            while (Input.GetMouseButton(0))
            {
                currentSquareIndex = GetSquareIndex();
                // If out of bounds, return
                if (currentSquareIndex == -1) yield break;
                // If same index as last check, skip check
                if (currentSquareIndex == lastSquareIndex) continue;
                // If dragged into new square, parse
                lastSquareIndex = currentSquareIndex;
                yield return null;
            }
        }

        // Return if square at index is empty
        private bool IsSquareEmpty(int index) => squares[index].type == SquareType.Empty;

        // Return if square at index is node
        private bool IsSquareNode(int index) => squares[index].type == SquareType.Node;

        // Return if square at index is wire
        private bool IsSquareWire(int index) => squares[index].type == SquareType.Wire;

        // Return if square at index is open wire
        private bool IsSquareOpenWire(int index) => squares[index].type == SquareType.OpenWire;
    }
}
