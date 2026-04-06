using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; 
using TMPro; // <-- AÑADIDO PARA SOLUCIONAR EL ERROR DE COMPILACIÓN

public class MazeController : MinigameBase
{
    [Header("Maze Settings")]
    public float maxMazeTime = 20f; // Tiempo ajustable públicamente

    [Header("Maze UI")]
    public TextMeshProUGUI mazeTimerText; // Contador numérico

    [Header("Events (Visuals)")]
    public UnityEvent<float> onHallucinationChanged; 

    protected override void Start()
    {
        base.Start();

        // Obtenemos la intensidad global persistente del GameManager
        float intensity = (GameManager.Instance != null) ? GameManager.Instance.globalHallucinationIntensity : 0.3f;

        // Ajustamos el tiempo del laberinto usando la variable pública
        int currentScore = (GameManager.Instance != null) ? GameManager.Instance.score : 0;
        maxTime = Mathf.Max(5f, maxMazeTime - (currentScore * 1f)); 
        timer = maxTime;
        
        if (timeBar != null) { timeBar.maxValue = maxTime; timeBar.value = maxTime; }
        if (mazeTimerText != null) mazeTimerText.text = $"Tiempo: {timer:F0}/{maxTime:F0}";

        Debug.Log($"[Maze] Iniciando con intensidad: {intensity} y tiempo: {maxTime}s");
        
        // Aplicamos la alucinación
        onHallucinationChanged?.Invoke(intensity);

        // Forzamos que el juego empiece, por si estás testeando sin haber asignado 'minigameData'
        isGameActive = true;
    }

    protected override void Update()
    {
        base.Update();
        if (!isGameActive) return;

        // Actualizar el texto del contador estilo NoDesmayarse
        if (mazeTimerText != null) 
        {
            mazeTimerText.text = $"Tiempo: {timer:F1}/{maxTime:F0}";
        }

        // Mantenemos la alucinación visual siempre activa con el valor global
        float intensity = (GameManager.Instance != null) ? GameManager.Instance.globalHallucinationIntensity : 0.3f;
        onHallucinationChanged?.Invoke(intensity);
    }

    public void AddWallTouch()
    {
        if (!isGameActive) return;
        
        Debug.Log("[Maze] ¡BORDE TOCADO! Perdiste.");
        EndMinigame(false); // Llamada directa a la función base original
    }

    public void ReportGoalReached()
    {
        if (!isGameActive) return;
        
        Debug.Log("[Maze] ¡META ALCANZADA! Ganaste.");
        EndMinigame(true); // Llamada directa a la función base original
    }

    public void ReportWallCollision() { AddWallTouch(); }
    public void RemoveWallTouch() { }
    public void ClearTouches() { }
}
