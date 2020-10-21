using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Powerbox
{
    public class ReceiverManager : MonoBehaviour
    {
        [Header("Attributes")]
        [SerializeField] [Range(0.5f, 2)] private float updateInterval = 1;
        [SerializeField] [Range(0, 1)] private float receiverSpawnChance = 0.1f;
        [SerializeField] [Range(5, 60)] private int countdownTime = 30;
        [SerializeField] [Range(0, 24)] private int maxActiveReceivers = 6;

        [Header("References")]
        [SerializeField] private Gameboard gameboard = null;
        [SerializeField] private TextMeshProUGUI[] countdowns = new TextMeshProUGUI[24];
        [SerializeField] private SpriteRenderer[] receivers = new SpriteRenderer[24];

        public static bool gameOver;
        public List<int> activeReceivers { get; private set; } = new List<int>();

        private float lastCallTime;

        private void Start()
        {
            gameOver = false;
            // Initialize last call time
            lastCallTime = Time.time;
        }

        private void Update()
        {
            // Only update receivers and countdowns every interval
            if (Time.time - lastCallTime >= updateInterval)
            {
                if (gameOver) return;
                lastCallTime = Time.time;
                UpdateInterval();
            }
        }

        // Runs once every interval
        private void UpdateInterval()
        {
            DecrementCountdown();
            // If receiver chance met, spawn receiver
            if (Random.Range(0, 1.0f) < receiverSpawnChance) SpawnReceiver();
        }

        // Decrements all active countdowns
        private void DecrementCountdown()
        {
            foreach (int i in activeReceivers)
            {
                // Parse and decrement countdown value
                TextMeshProUGUI countdown = countdowns[i];
                int countdownVal = int.Parse(countdown.text);
                // If countdown has run out, end game
                if (countdownVal <= 0)
                {
                    GameOver();
                    return;
                }
                countdown.text = (countdownVal - 1).ToString();
            }
        }

        // Spawns a new receiver
        private void SpawnReceiver()
        {
            // If max receivers active, return
            if (activeReceivers.Count >= maxActiveReceivers) return;
            int index = Random.Range(0, 24);
            // Generate new random index while receiver at index already active
            while (activeReceivers.Contains(index)) index = Random.Range(0, 24);
            activeReceivers.Add(index);

            TextMeshProUGUI countdown = countdowns[index];
            SpriteRenderer receiver = receivers[index];

            countdown.enabled = true;
            countdown.text = countdownTime.ToString();
            receiver.enabled = true;
            receiver.color = GetRandomReceiverColor();
        }

        // Returns a random receiver color
        private Color GetRandomReceiverColor()
        {
            int randomIndex = Random.Range(0, gameboard.nodeCount);
            return gameboard.colors[randomIndex];
        }

        private void GameOver()
        {
            gameOver = true;
            Debug.Log("Game over!");
        }
    }
}
