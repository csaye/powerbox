using UnityEngine;

namespace Powerbox
{
    public class Gameboard : MonoBehaviour
    {
        [Header("References")]
        // [SerializeField] private GameObject square = null;
        [SerializeField] private Square[] squares = new Square[64];
        [SerializeField] private Camera mainCamera = null;

        private void Start()
        {
            for (int i = 0; i < 64; i++)
            {
                SpriteRenderer sr = transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();
                squares[i] = new Square(sr, SquareType.Empty, SquareColor.None);
            }
        }

        private void OnMouseDown()
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int xIndex = Mathf.FloorToInt(mousePosition.x + 4);
            int yIndex = Mathf.FloorToInt(-mousePosition.y + 4);
            int squareIndex = xIndex + (8 * yIndex);
            ClickSquare(squareIndex);
        }

        private void ClickSquare(int squareIndex)
        {
            
        }
    }
}
