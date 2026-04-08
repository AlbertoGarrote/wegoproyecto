// Force Recompile
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace WeGoCrazy.Minigames.SimonSays
{
    public class SimonSaysController : MinigameBase
    {
        [Header("Simon Settings")]
        public List<HouseholdItemInteractable> householdItems;
        public float timeBetweenHighlights = 1f;
        public int targetLevelToWin = 4; // Rondas necesarias para ganar
        public float maxSimonTime = 25f; // Tiempo base

        [Header("UI & Events")]
        public TextMeshProUGUI timeText;
        public UnityEvent<float> onHallucinationChanged;
        public UnityEvent onGameOver;
        public UnityEvent onRoundWon;

        [Header("State")]
        private List<int> currentSequence = new List<int>();
        private int playerSequenceIndex = 0;
        private bool isPlayerTurn = false;
        private int currentLevel = 1;

        protected override void Start()
        {
            base.Start();

            // Suscribir el hub del controlador a cada uno de los inputs del objeto
            foreach (var item in householdItems)
            {
                if(item != null) item.onItemInteracted.AddListener(OnPlayerInput);
            }

            int currentScore = (GameManager.Instance != null) ? GameManager.Instance.score : 0;
            
            // LA MAGIA DE LA PROGRESIÓN INFINITA
            // La primera vez pide 4 rondas. A medida que ganes globalmente, pedirá 5, 6, 7...
            targetLevelToWin = 4 + currentScore;

            float baseIntensity = (GameManager.Instance != null) ? GameManager.Instance.globalHallucinationIntensity : 0.3f;

            // Ocultamos/Ignoramos el UI del temporizador si existe
            if (timeBar != null) timeBar.gameObject.SetActive(false);
            if (timeText != null) timeText.text = $"Rondas: {currentLevel}/{targetLevelToWin}";

            // Aplicamos la alucinación visual base de la partida
            onHallucinationChanged?.Invoke(baseIntensity);

            isGameActive = true; 
            
            // Empieza el juego de luces automáticamente
            StartGame();
        }

        private bool showingFeedback = false;

        protected override void Update()
        {
            // ELIMINAMOS EL base.Update(); para que el cronómetro deje de bajar y no se pierda por tiempo.
            if (!isGameActive) return;

            // Actualizar cuenta de rondas en el texto si existe y no estamos mostrando el feedback
            if (timeText != null && !showingFeedback) 
            {
                timeText.text = $"Rondas: {currentLevel}/{targetLevelToWin}";
            }

            // Las alucinaciones en Simon Says empeoran mientras avanza la ronda
            float baseIntensity = (GameManager.Instance != null) ? GameManager.Instance.globalHallucinationIntensity : 0.25f;
            float extraIntesity = (currentLevel - 1) * 0.1f; // Se distorsiona un poco más cada vez que aciertas
            onHallucinationChanged?.Invoke(Mathf.Clamp01(baseIntensity + extraIntesity));
        }

        public void StartGame()
        {
            currentSequence.Clear();
            currentLevel = 1;
            AddNewStep();
            StartCoroutine(PlaySequence());
        }

        private void AddNewStep()
        {
            if (householdItems.Count == 0) return;
            int randomId = householdItems[Random.Range(0, householdItems.Count)].itemId;
            currentSequence.Add(randomId);
        }

        private IEnumerator PlaySequence()
        {
            isPlayerTurn = false;
            
            // Avisamos al jugador que debe esperar
            if (timeText != null) timeText.text = "¡MEMORIZA!";
            
            yield return new WaitForSeconds(0.8f);

            foreach (int eqId in currentSequence)
            {
                if (!isGameActive) break;

                HouseholdItemInteractable item = householdItems.Find(e => e.itemId == eqId);
                if (item != null)
                {
                    item.PlayEffect();
                }
                yield return new WaitForSeconds(timeBetweenHighlights);
            }

            playerSequenceIndex = 0;
            isPlayerTurn = true;
            
            // Avisamos que ya puede pulsar
            if (timeText != null) timeText.text = "¡TU TURNO!";
            Debug.Log($"[Simon] Turno del jugador. Nivel actual: {currentLevel}");
        }

        public void OnPlayerInput(int itemId)
        {
            // Si el jugador pulsa antes de tiempo o el juego no está activo, ignoramos
            if (!isPlayerTurn || !isGameActive) return;

            if (currentSequence[playerSequenceIndex] == itemId)
            {
                playerSequenceIndex++;
                if (playerSequenceIndex >= currentSequence.Count)
                {
                    RoundWon();
                }
            }
            else
            {
                GameOver();
            }
        }

        private void RoundWon()
        {
            if (!isGameActive) return;

            isPlayerTurn = false;
            currentLevel++;
            onRoundWon?.Invoke();

            if (currentLevel > targetLevelToWin)
            {
                StartCoroutine(ShowFinalVictory());
            }
            else
            {
                StartCoroutine(ShowRoundFeedback());
            }
        }

        private IEnumerator ShowRoundFeedback()
        {
            showingFeedback = true;
            if (timeText != null) 
            {
                // currentLevel ya se ha incrementado, por eso restamos 1 para el mensaje
                timeText.text = $"RONDA {currentLevel - 1} ¡OK!";
                timeText.color = Color.green;
            }
            yield return new WaitForSeconds(1.0f);
            
            if (timeText != null) 
            {
                timeText.text = $"SIGUIENTE: {currentLevel}/{targetLevelToWin}";
                timeText.color = Color.white;
            }
            yield return new WaitForSeconds(0.5f);

            showingFeedback = false;
            
            AddNewStep();
            StartCoroutine(PlaySequence());
        }

        private IEnumerator ShowFinalVictory()
        {
            showingFeedback = true;
            if (timeText != null) 
            {
                timeText.text = "¡MINIJUEGO SUPERADO!";
                timeText.color = Color.cyan;
            }
            yield return new WaitForSeconds(1.5f);
            EndMinigame(true);
        }

        private void GameOver()
        {
            if (!isGameActive) return;
            isPlayerTurn = false;
            EndMinigame(false);
        }
    }
}
