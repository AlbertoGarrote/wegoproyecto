using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Necesario para Corrutinas

public class TirarBotellaMinigame : MinigameBase
{
    [Header("Configuración Golf Vertical")]
    [SerializeField] private Slider powerBar;
    [SerializeField] private RectTransform targetZone;

    [Header("Ajustes de Juego")]
    public float fillSpeed = 1.5f;
    public float successThreshold = 0.1f;

    [Header("Referencias Visuales")]
    public GameObject spriteInicial;    // El que empieza activo
    public GameObject spriteAlternativo; // El que se activa al acertar
    public GameObject botellaPrefab;     // El prefab que volará
    public Transform puntoLanzamiento;   // Derecha de la pantalla
    public Transform puntoObjetivo;      // Centro de la pantalla

    private float targetValue;
    private bool isCharging = false;
    private int goalsRequired;
    private int currentGoals = 0;
    private bool bloqueadoPorAnimacion = false; // Para que no tiren mientras vuela la botella

    public TextMeshProUGUI lanzamientos;

    protected override void Start()
    {
        base.Start();
        goalsRequired = 2 + (GameManager.Instance.score / 3);
        lanzamientos.text = $"Lanzamientos: {currentGoals}/{goalsRequired}";

        // Estado inicial de los sprites
        spriteInicial.SetActive(true);
        spriteAlternativo.SetActive(false);

        SetupNewTarget();
    }

    void SetupNewTarget()
    {
        powerBar.value = 0;
        targetValue = Random.Range(0.2f, 0.8f);
        float barLength = powerBar.GetComponent<RectTransform>().rect.width;
        targetZone.anchoredPosition = new Vector2(targetValue * barLength, 0);
    }

    protected override void Update()
    {
        base.Update();
        if (!isGameActive || bloqueadoPorAnimacion) return;

        if (Input.GetMouseButtonDown(0)) isCharging = true;

        if (isCharging && Input.GetMouseButton(0))
        {
            powerBar.value += fillSpeed * Time.deltaTime;
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

            // Iniciamos la secuencia de acierto
            StartCoroutine(SecuenciaAcierto());
        }
        else
        {
            powerBar.value = 0;
        }
    }

    IEnumerator SecuenciaAcierto()
    {
        bloqueadoPorAnimacion = true;

        // 1. Cambiar Sprites
        spriteInicial.SetActive(false);
        spriteAlternativo.SetActive(true);

        // 2. Instanciar Botella
        GameObject botella = Instantiate(botellaPrefab, puntoLanzamiento.position, Quaternion.identity);

        float duracion = 0.4f; // Cuánto tarda en llegar
        float tiempo = 0f;
        Vector3 escalaInicial = botella.transform.localScale;
        Vector3 escalaFinal = escalaInicial * 0.2f; // Se hace pequeña

        // 3. Animación de vuelo
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;

            // Mover al centro
            botella.transform.position = Vector3.Lerp(puntoLanzamiento.position, puntoObjetivo.position, t);

            // Rotar muy rápido
            botella.transform.Rotate(Vector3.forward * 1500f * Time.deltaTime);

            // Escalar (hacerse pequeña)
            botella.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);

            yield return null;
        }

        Destroy(botella); // Borramos la botella al llegar

        // 4. Resetear para el siguiente tiro o terminar
        if (currentGoals >= goalsRequired)
        {
            EndMinigame(true);
        }
        else
        {
            // Volvemos a los sprites originales y nueva diana
            spriteInicial.SetActive(true);
            spriteAlternativo.SetActive(false);
            bloqueadoPorAnimacion = false;
            SetupNewTarget();
        }
    }
}