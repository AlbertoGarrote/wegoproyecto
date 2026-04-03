using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;

public class TransitionController : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI scoreText;

    void Start()
    {
        // Recuperamos la info del GameManager
        if (GameManager.Instance.nextMinigame != null)
        {
            nameText.text = GameManager.Instance.nextMinigame.gameName;
            instructionText.text = GameManager.Instance.nextMinigame.instruction;
            livesText.text = "VIDAS: " + GameManager.Instance.lives;
            scoreText.text = "SCORE: " + GameManager.Instance.score;
        }
    }

    void Update()
    {
        // Al pulsar espacio, cargamos la escena que el GameManager preparó
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string sceneToLoad = GameManager.Instance.nextMinigame.sceneName;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}