using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Pool de Minijuegos")]
    public List<MinigameData> minigamePool;

    [Header("Estado Global")]
    public int score = 0;
    public int lives = 3;
    public float globalHallucinationIntensity = 0.25f;
    public float hallucinationIncreasePerWin = 0.05f;

    [Header("Referencia")]
    public MinigameData nextMinigame;
    private MinigameData lastMinigame;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartGameCycle()
    {
        score = 0;
        lives = 3;
        globalHallucinationIntensity = 0.25f;
        PrepareNextMinigame();
    }

    public void OnMinigameEnded(bool success)
    {
        if (!success)
        {
            lives--;
            Debug.Log("[GameManager] Pierdes una vida! Vidas restantes: " + lives);
        }
        else
        {
            score++;
            globalHallucinationIntensity += hallucinationIncreasePerWin;
            Debug.Log("[GameManager] Punto ganado! Intensidad de alucinacion actual: " + globalHallucinationIntensity);
        }

        if (lives <= 0)
        {
            Debug.Log("Game Over");
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            PrepareNextMinigame();
        }
    }

    private void PrepareNextMinigame()
    {
        if (minigamePool == null || minigamePool.Count == 0) return;

        MinigameData selected = minigamePool[Random.Range(0, minigamePool.Count)];

        if (minigamePool.Count > 1)
        {
            while (selected == lastMinigame)
            {
                selected = minigamePool[Random.Range(0, minigamePool.Count)];
            }
        }

        lastMinigame = selected;
        nextMinigame = selected;
        SceneManager.LoadScene("Transition");
    }
}
