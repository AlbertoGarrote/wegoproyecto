using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necesario para layouts

public class TransitionController : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI scoreText;

    [Header("Sistema de Vidas Visual")]
    public GameObject lifePrefab;      // El icono de la vida (Prefab)
    public Transform lifeContainer;    // El objeto "ContainerVidas" con el Layout Group

    void Start()
    {
        // Limpiamos el contenedor por si acaso quedara algo del editor
        foreach (Transform child in lifeContainer)
        {
            Destroy(child.gameObject);
        }

        if (GameManager.Instance != null && GameManager.Instance.nextMinigame != null)
        {
            nameText.text = GameManager.Instance.nextMinigame.gameName;
            instructionText.text = GameManager.Instance.nextMinigame.instruction;
            scoreText.text = GameManager.Instance.score.ToString();


            // --- LÓGICA DE INSTANCIAR VIDAS ---
            for (int i = 0; i < GameManager.Instance.lives; i++)
            {
                Instantiate(lifePrefab, lifeContainer);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string sceneToLoad = GameManager.Instance.nextMinigame.sceneName;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}