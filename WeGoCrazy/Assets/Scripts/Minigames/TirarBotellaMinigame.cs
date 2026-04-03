using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TirarBotellaMinigame : MinigameBase
{
    [Header("Configuración Golf Vertical")]
    [SerializeField] private Slider powerBar;
    [SerializeField] private RectTransform targetZone; // La imagen verde

    [Header("Ajustes de Juego")]
    public float fillSpeed = 1.5f;
    public float successThreshold = 0.1f; // Margen de acierto

    private float targetValue;
    private bool isCharging = false;
    private int goalsRequired;
    private int currentGoals = 0;

    public TextMeshProUGUI lanzamientos;

    protected override void Start()
    {
        base.Start();

        // El número de aciertos sube con el score global
        goalsRequired = 2 + (GameManager.Instance.score / 3);

        lanzamientos.text = $"Clicks: {currentGoals}/{goalsRequired}";

        SetupNewTarget();
    }

    void SetupNewTarget()
    {
        powerBar.value = 0;
        targetValue = Random.Range(0.2f, 0.8f);

        // Si está rotado, la "altura" ahora es el "Width" (ancho)
        float barLength = powerBar.GetComponent<RectTransform>().rect.width;

        // Invertimos: Movemos X y dejamos Y en 0
        // (Pon un signo negativo en la X si ves que se mueve al lado contrario)
        targetZone.anchoredPosition = new Vector2(targetValue * barLength, 0);
    }

    protected override void Update()
    {
        base.Update(); // Maneja el tiempo global (si llega a 0, pierdes)
        if (!isGameActive) return;

        if (Input.GetMouseButtonDown(0)) isCharging = true;

        if (isCharging && Input.GetMouseButton(0))
        {
            powerBar.value += fillSpeed * Time.deltaTime;

            // Si llega arriba (1.0) y sigue pulsando, vuelve a empezar desde abajo
            if (powerBar.value >= 1f) powerBar.value = 0f;
        }

        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            isCharging = false;
            CheckShot();
        }
    }

    void CheckShot()
    {
        float difference = Mathf.Abs(powerBar.value - targetValue);

        if (difference <= successThreshold)
        {
            currentGoals++;
            lanzamientos.text = $"Lanzamientos: {currentGoals}/{goalsRequired}";

            if (currentGoals >= goalsRequired)
            {
                EndMinigame(true); // Gana el minijuego
            }
            else
            {
                SetupNewTarget(); // Nuevo objetivo en la misma partida
            }
        }
        else
        {
            // FALLO: Solo reseteamos el slider para que lo intente otra vez
            // mientras le quede tiempo en el temporizador global.
            Debug.Log("Fallo, inténtalo de nuevo...");
            powerBar.value = 0;
        }
    }
}