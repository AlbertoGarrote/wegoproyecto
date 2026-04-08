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
            liftProgress = 50f; // Empezamos a la mitad para dar margen

            int currentScore = (GameManager.Instance != null) ? GameManager.Instance.score : 0;

            // --- TIEMPO MUCHO MENOR ---
            // Empezamos en 5 segundos y baja 0.3s por cada punto, mínimo 2 segundos.
            maxTime = Mathf.Max(2f, 5f - (currentScore * 0.3f));
            timer = maxTime;

            if (timeBar != null) { timeBar.maxValue = maxTime; timeBar.value = maxTime; }

            isGameActive = true;
        }


        protected override void Update()
        {
            // NO llamamos a base.Update() si queremos cambiar qué pasa al llegar a 0
            // Copiamos la lógica base pero cambiando el resultado del timer

            if (!isGameActive) return;

            timer -= Time.deltaTime;

            // Actualizamos la barra de tiempo (si existe)
            if (timeBar != null) timeBar.value = timer;

            if (timeText != null)
            {
                timeText.text = $"¡AGUANTA!: {timer:F1}s";
            }

            // --- CAMBIO CLAVE: Si el tiempo se acaba, ¡GANAS! ---
            if (timer <= 0)
            {
                timer = 0;
                WinGame();
            }
        }

        public void AddLift(float delta)
        {
            if (!isGameActive) return;

            liftProgress = Mathf.Clamp(liftProgress + delta, 0, maxLift);
            onLiftProgressUpdated?.Invoke(liftProgress / maxLift);

            // Quitamos la condición de victoria por progreso (liftProgress >= sipThreshold)
            // Ahora solo se puede perder si cae el brazo
            if (liftProgress <= 0f)
            {
                LoseGame("¡Se te cansó el brazo!");
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
