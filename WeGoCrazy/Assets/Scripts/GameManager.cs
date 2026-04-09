using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // --- SISTEMA DE SONIDO ---
    [System.Serializable]
    public struct SoundEffect
    {
        public string name;      // Nombre para llamar al sonido (ej: "Click", "Win")
        public AudioClip clip;   // El archivo de audio
    }

    [Header("Configuración de Sonido")]
    public List<SoundEffect> soundLibrary; // Lista de sonidos configurables
    private AudioSource audioSource;       // El componente que reproduce

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

        // Inicializamos el AudioSource automáticamente
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // --- FUNCIÓN PARA REPRODUCIR SONIDOS ---
    public void PlaySound(string soundName)
    {
        // Buscamos el sonido por nombre en la lista
        SoundEffect s = soundLibrary.Find(x => x.name == soundName);

        if (s.clip != null)
        {
            // PlayOneShot permite que varios sonidos suenen a la vez sin cortarse
            audioSource.PlayOneShot(s.clip);
        }
        else
        {
            Debug.LogWarning("Sonido no encontrado o sin clip: " + soundName);
        }
    }

    // --- LÓGICA DE JUEGO ---
    public void StartGameCycle()
    {
        score = 0;
        lives = 5;
        globalHallucinationIntensity = 0.25f;
        PrepareNextMinigame();
    }

    public void OnMinigameEnded(bool success)
    {
        if (!success)
        {
            lives--;
            PlaySound("error"); // Ejemplo de llamada
        }
        else
        {
            score++;
            globalHallucinationIntensity += hallucinationIncreasePerWin;
            PlaySound("ganar"); // Ejemplo de llamada
        }

        if (lives <= 0)
        {
            PlaySound("GameOver");
            SceneManager.LoadScene("GameOver");
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