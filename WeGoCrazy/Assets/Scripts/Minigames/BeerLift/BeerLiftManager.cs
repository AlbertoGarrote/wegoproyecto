using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace WeGoCrazy.Minigames.BeerLift
{
    public class BeerLiftManager : MinigameBase
    {
        [Header("State")]
        public float liftProgress = 30f; // Empieza al 30% para no perder instantáneamente al spawnear
        public float maxLift = 100f;
        public float sipThreshold = 99f; 

        [Header("Rules")]
        public float spillTolerance = 60f; // Máximo ángulo antes de derramar

        [Header("Timer & UI")]
        public float maxBeerTime = 15f; // Tiempo para beberla publicamente ajustable
        public TextMeshProUGUI timeText; // Contador numérico

        [Header("Events")]
        public UnityEvent onSpilledBeer;
        public UnityEvent onDrankBeer;
        public UnityEvent<float> onLiftProgressUpdated;
        public UnityEvent<float> onHallucinationChanged; // Para conectar a PsychedelicVisuals

        protected override void Start()
        {
            base.Start();

            // Evitamos que Unity sobreescriba en el Inspector el progreso y mate al jugar en el primer frame
            liftProgress = 30f;

            // Obtenemos intensidad global
            float intensity = (GameManager.Instance != null) ? GameManager.Instance.globalHallucinationIntensity : 0.3f;
            int currentScore = (GameManager.Instance != null) ? GameManager.Instance.score : 0;

            // Ajuste del tiempo escalable
            maxTime = Mathf.Max(5f, maxBeerTime - (currentScore * 0.5f));
            timer = maxTime;

            if (timeBar != null) { timeBar.maxValue = maxTime; timeBar.value = maxTime; }
            if (timeText != null) timeText.text = $"Tiempo: {timer:F0}/{maxTime:F0}";

            // Aplicamos la alucinación visual
            onHallucinationChanged?.Invoke(intensity);

            isGameActive = true; // Forzamos arranque
        }

        protected override void Update()
        {
            base.Update();
            if (!isGameActive) return;

            // Actualizar cuenta atrás del tiempo
            if (timeText != null) 
            {
                timeText.text = $"Tiempo: {timer:F1}/{maxTime:F0}";
            }

            // Mantenemos la alucinación constante por si cambia en directo
            float intensity = (GameManager.Instance != null) ? GameManager.Instance.globalHallucinationIntensity : 0.3f;
            onHallucinationChanged?.Invoke(intensity);
        }

        public void AddLift(float delta)
        {
            if (!isGameActive) return;
            
            liftProgress = Mathf.Clamp(liftProgress + delta, 0, maxLift);
            onLiftProgressUpdated?.Invoke(liftProgress / maxLift);

            // Condición de victoria: Arriba del todo
            if (liftProgress >= sipThreshold)
            {
                WinGame();
            }
            // Condición de derrota: Abajo del todo
            else if (liftProgress <= 0f)
            {
                LoseGame("¡El brazo se te cayó por gravedad a 0 de progreso! (liftProgress = 0)");
            }
        }

        public void CheckSpill(float currentAngle)
        {
            if (!isGameActive) return;
            
            if (Mathf.Abs(currentAngle) > spillTolerance)
            {
                LoseGame($"¡Derramaste la cerveza! Inclinación superó la tolerancia. (Ángulo actual: {currentAngle} > Tolerancia: {spillTolerance})");
            }
        }

        private void WinGame()
        {
            if (!isGameActive) return;
            Debug.Log("[Shroom Trip] ¡Te has bebido la cerveza sin derramarla! Pura proteína post-entreno.");
            onDrankBeer?.Invoke();
            EndMinigame(true);
        }

        private void LoseGame(string reason)
        {
            if (!isGameActive) return;
            Debug.Log($"[Shroom Trip DERROTA] {reason}");
            Debug.Log("[Shroom Trip DERROTA] ¡Oh no! El brazo era de goma, cerveza derramada en el sofá.");
            onSpilledBeer?.Invoke();
            EndMinigame(false);
        }
    }
}
