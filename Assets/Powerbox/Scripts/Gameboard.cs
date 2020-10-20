using UnityEngine;

namespace Powerbox
{
    public class Gameboard : MonoBehaviour
    {
        [Header("References")]
        // [SerializeField] private GameObject square = null;
        [SerializeField] private SpriteRenderer[] squares = new SpriteRenderer[64];
        [SerializeField] private Camera mainCamera = null;

        // private void Start()
        // {
        //     int i = 0;
        //     for (int y = 4; y > -4; y--)
        //     {
        //         for (int x = -4; x < 4; x++)
        //         {
        //             GameObject obj = Instantiate(square, new Vector2(x + 0.5f, y - 0.5f), Quaternion.identity, transform);
        //             obj.name = $"Square{i}";
        //             squares[i] = obj.GetComponent<SpriteRenderer>();
        //             i++;
        //         }
        //     }
        // }

        public void OnMouseDown()
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int xIndex = Mathf.FloorToInt(mousePosition.x + 4);
            int yIndex = Mathf.FloorToInt(-mousePosition.y + 4);
            int squareIndex = xIndex + (8 * yIndex);
            Debug.Log(squareIndex);
        }
    }
}
