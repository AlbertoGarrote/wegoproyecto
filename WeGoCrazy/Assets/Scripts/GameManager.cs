using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuración")]
    public List<MinigameData> minigamePool;
    private MinigameData lastMinigame;

    [Header("Estado")]
    public int lives = 5;
    public int score = 0;
    public MinigameData nextMinigame;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void StartGameCycle()
    {
        lives = 5;  // Reiniciamos vidas
        score = 0;  // Reiniciamos puntuación
        PrepareNextMinigame(); // Esto elige el primero y carga la Transición
    }

    public void OnMinigameEnded(bool success)
    {
        if (!success) lives--;
        else score++;

        if (lives <= 0)
        {
            Debug.Log("Game Over");
            SceneManager.LoadScene("MainMenu"); // O una escena de Game Over
        }
        else
        {
            PrepareNextMinigame();
        }
    }

    private void PrepareNextMinigame()
    {
        MinigameData selected = minigamePool[Random.Range(0, minigamePool.Count)];

        // Si solo hay uno, no podemos evitar repetir, pero si hay más...
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