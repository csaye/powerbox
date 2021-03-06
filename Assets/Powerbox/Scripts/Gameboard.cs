﻿using System;
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
        [Header("Attributes")]
        [Range(1, 4)] public int nodeCount = 4;
        public Color[] colors = null;
        
        [Header("References")]
        [SerializeField] private ReceiverManager receiverManager = null;
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private Square[] squares = new Square[64];
        [SerializeField] private Sprite[] nodeSprites = new Sprite[16];
        [SerializeField] private Sprite[] wireSprites = new Sprite[10];

        private Coroutine dragCoroutine = null;

        private void Start()
        {
            GenerateGameboard();
        }

        // Returns a random number between low and high inclusive
        private int Rand(int low, int high)
        {
            return UnityEngine.Random.Range(low, high + 1);
        }

        // Generates a default gameboard of nodes
        private void GenerateGameboard()
        {
            int[] nodeIndices = new int[nodeCount];
            for (int i = 0; i < nodeCount; i++)
            {
                // Get random index of position not on edge of board
                int randomIndex = Rand(1, 6) + (8 * Rand(1, 6));
                // Generate new index while touching existing node
                while (NodeTouches(nodeIndices, i, randomIndex)) randomIndex = Rand(1, 6) + (8 * Rand(1, 6));
                SetSquare(randomIndex, SquareType.Node, colors[i], nodeSprites[0]);
                nodeIndices[i] = randomIndex;
            }
        }

        // Returns whether a node in 
        private bool NodeTouches(int[] nodeIndices, int currentIndex, int index)
        {
            for (int i = 0; i < currentIndex; i++)
            {
                int nodeIndex = nodeIndices[i];
                // Above row
                if (nodeIndex - 9 <= index && index <= nodeIndex - 7) return true;
                // On row
                if (nodeIndex - 1 <= index && index <= nodeIndex + 1) return true;
                // Below row
                if (nodeIndex + 7 <= index && index <= nodeIndex + 9) return true;
            }
            return false;
        }

        // Sets square at index to given values
        private void SetSquare(int index, SquareType type, Color color, Sprite sprite)
        {
            Square square = squares[index];
            square.type = type;
            square.color = color;
            square.sprite = sprite;
        }

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
            // Return if game over
            if (ReceiverManager.gameOver) return;
            StopDragCoroutine();
            dragCoroutine = StartCoroutine(Drag());
        }

        public void StopDragCoroutine()
        {
            if (dragCoroutine != null) StopCoroutine(dragCoroutine);
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
                    // Get reference to squares
                    Square currentSquare = squares[currentSquareIndex];
                    Square lastSquare = squares[lastSquareIndex];

                    // If dragged onto nonempty space, attempt to parse removal
                    if (!IsSquareType(currentSquareIndex, SquareType.Empty))
                    {
                        // If incorrect color, break
                        if (currentSquare.color != dragColor) yield break;
                        // If not connected, break
                        if (!currentSquare.connectedSquares.Contains(lastSquareIndex)) yield break;
                        // If last square type not wire, break
                        if (lastSquare.type != SquareType.Wire) yield break;
                        // Remove connections
                        lastSquare.connectedSquares.Remove(currentSquareIndex);
                        currentSquare.connectedSquares.Remove(lastSquareIndex);
                        // Remove wire
                        RemoveWire(lastSquareIndex, currentSquareIndex);
                    }
                    // If dragged onto empty square, set wire
                    else
                    {
                        // Create connections
                        lastSquare.connectedSquares.Add(currentSquareIndex);
                        currentSquare.connectedSquares.Add(lastSquareIndex);
                        // Set wire
                        AddWire(currentSquareIndex, dragColor, lastSquareIndex);
                    }
                    lastSquareIndex = currentSquareIndex;
                }
                yield return null;
            }
        }

        // Adds wire at given index of given color
        private void AddWire(int index, Color color, int indexToUpdate)
        {
            // Get square, set color and type
            Square square = squares[index];
            square.color = color;
            square.type = SquareType.Wire;
            square.color = color;
            // Update wire sprite
            UpdateSprite(index);
            // Update last square sprite
            UpdateSprite(indexToUpdate);
        }

        // Removes wire at given index
        private void RemoveWire(int index, int indexToUpdate)
        {
            // Reset square
            Square square = squares[index];
            square.Reset();
            // Update last square sprite
            UpdateSprite(indexToUpdate);
        }

        // Updates sprite for given square index
        public void UpdateSprite(int index)
        {
            squares[index].sprite = GetSprite(index);
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
                {
                    // If in top row
                    if (index < 8)
                    {
                        // Return false on corners, otherwise check for receiver
                        if (index == 0 || index == 7) return false;
                        int receiverIndex = index - 1;
                        if (receiverManager.ReceiverActive(receiverIndex, color))
                        {
                            receiverManager.MakeReceiverConnection(index, receiverIndex);
                            StopDragCoroutine();
                            return true;
                        }
                        else return false;
                    }
                    adjacent = index - 8;
                    break;
                }
                case Direction.Right:
                {
                    // If in rightmost column
                    if (index % 8 == 7)
                    {
                        // Return false on corners, otherwise check for receiver
                        if (index == 7 || index == 63) return false;
                        int receiverIndex = ((index + 1) / 8) + 4;
                        if (receiverManager.ReceiverActive(receiverIndex, color))
                        {
                            receiverManager.MakeReceiverConnection(index, receiverIndex);
                            StopDragCoroutine();
                            return true;
                        }
                        else return false;
                    }
                    adjacent = index + 1;
                    break;
                }
                case Direction.Down:
                {
                    // If in bottom row
                    if (index > 55)
                    {
                        // Return false on corners, otherwise check for receiver
                        if (index == 56 || index == 63) return false;
                        int receiverIndex = index - 45;
                        if (receiverManager.ReceiverActive(receiverIndex, color))
                        {
                            receiverManager.MakeReceiverConnection(index, receiverIndex);
                            StopDragCoroutine();
                            return true;
                        }
                        else return false;
                    }
                    adjacent = index + 8;
                    break;
                }
                case Direction.Left:
                {
                    // If in leftmost column
                    if (index % 8 == 0)
                    {
                        // Return false on corners, otherwise check for receiver
                        if (index == 0 || index == 56) return false;
                        int receiverIndex = (index / 8) + 17;
                        if (receiverManager.ReceiverActive(receiverIndex, color))
                        {
                            receiverManager.MakeReceiverConnection(index, receiverIndex);
                            StopDragCoroutine();
                            return true;
                        }
                        else return false;
                    }
                    adjacent = index - 1;
                    break;
                }
            }
            // Return whether square matches color connector
            return square.connectedSquares.Contains(adjacent);
        }
    }
}
