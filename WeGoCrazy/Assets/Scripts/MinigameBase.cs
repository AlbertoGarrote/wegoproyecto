using UnityEngine;
using UnityEngine.UI; // Para Sliders normales
using TMPro;        // Para el texto

public abstract class MinigameBase : MonoBehaviour
{
    [Header("UI del Minijuego")]
    public Slider timeBar;            // Arrastra el Slider aquí

    [Header("Configuración Base")]
    public MinigameData minigameData;
    protected float timer;
    protected float maxTime; // Necesario para calcular el porcentaje de la barra
    protected bool isGameActive = false;

    protected virtual void Start()
    {
        if (minigameData == null) return;

        // --- LÓGICA DE DIFICULTAD ESCALABLE ---
        // Supongamos que el tiempo base son 5 segundos. 
        // Por cada punto, restamos 0.2 segundos
        float reductionPerPoint = 0.2f;
        maxTime = Mathf.Max(1.5f, 5f - (GameManager.Instance.score * reductionPerPoint));

        timer = maxTime;

        Debug.Log($"Minijuego: {minigameData.gameName} | Tiempo asignado: {maxTime} segundos");

        if (timeBar != null)
        {
            timeBar.maxValue = maxTime;
            timeBar.value = maxTime;
        }

        isGameActive = true;
    }

    protected virtual void Update()
    {
        if (!isGameActive) return;

        timer -= Time.deltaTime;

        // Actualizamos la UI
        UpdateTimerUI();

        if (timer <= 0)
        {
            timer = 0;
            EndMinigame(false);
        }
    }

    private void UpdateTimerUI()
    {
        if (timeBar != null)
        {
            timeBar.value = timer;
        }
    }

    protected void EndMinigame(bool win)
    {
        if (!isGameActive) return;
        isGameActive = false;
        GameManager.Instance.OnMinigameEnded(win);
    }
}