using TMPro;
using UnityEngine;

public class NoDesmayarseMinigame : MinigameBase
{
    [Header("Mecanica Mash")]
    public int clicksRequired = 20;
    public TextMeshProUGUI clicks;

    [Header("Efecto Párpados (UI)")]
    public RectTransform parpadoSuperior;
    public RectTransform parpadoInferior;

    private int currentClicks = 0;

    // Usamos Vector3 para localPosition
    private Vector3 posInicialSupLocal;
    private Vector3 posInicialInfLocal;

    protected override void Start()
    {
        base.Start();

        // Lógica de dificultad basada en el score global
        int currentScore = (GameManager.Instance != null) ? GameManager.Instance.score : 0;
        clicksRequired += currentScore * 2;

        currentClicks = 0;
        if (clicks != null) clicks.text = $"Clicks: {currentClicks}/{clicksRequired}";

        // GUARDAR POSICIONES LOCALES INICIALES
        // El (0,0,0) local siempre es el centro del Panel padre
        if (parpadoSuperior != null) posInicialSupLocal = parpadoSuperior.localPosition;
        if (parpadoInferior != null) posInicialInfLocal = parpadoInferior.localPosition;
    }

    protected override void Update()
    {
        base.Update(); // Gestiona el temporizador de la clase base

        if (!isGameActive) return;

        ManejarEfectoOjos();

        // Detectar clicks del jugador
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentClicks++;
            if (clicks != null) clicks.text = $"Clicks: {currentClicks}/{clicksRequired}";

            if (currentClicks >= clicksRequired)
            {
                EndMinigame(true);
            }
        }
    }

    void ManejarEfectoOjos()
    {
        if (parpadoSuperior == null || parpadoInferior == null) return;

        // t va de 0 (ojos abiertos) a 1 (ojos cerrados)
        float t = 1f - (timer / maxTime);

        // El destino final es Y = 0 en el espacio local del Panel
        Vector3 destinoSup = new Vector3(posInicialSupLocal.x, 567.13f, 0);
        Vector3 destinoInf = new Vector3(posInicialInfLocal.x, -509.3f, 0);

        // Interpolar posición
        parpadoSuperior.localPosition = Vector3.Lerp(posInicialSupLocal, destinoSup, t);
        parpadoInferior.localPosition = Vector3.Lerp(posInicialInfLocal, destinoInf, t);
    }
}