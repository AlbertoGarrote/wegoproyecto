using TMPro;
using UnityEngine;

public class NoDesmayarseMinigame : MinigameBase
{
    [Header("Mecánica Mash")]
    public int clicksRequired = 20;
    public TextMeshProUGUI clicks;

    private int currentClicks = 0;

    protected override void Start()
    {
        // 1. Ejecutamos el Start del padre (configura el tiempo)
        base.Start();

        // 2. Ajustamos los clicks según el score
        // Ejemplo: 10 clicks base + 2 por cada punto de score
        clicksRequired += GameManager.Instance.score * 2;

        currentClicks = 0;
        clicks.text = $"Clicks: {currentClicks}/{clicksRequired}";
    }

    protected override void Update()
    {
        // Ejecutamos el Update de la base (el que maneja el tiempo)
        base.Update();

        if (!isGameActive) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentClicks++;
            clicks.text = $"Clicks: {currentClicks}/{clicksRequired}";

            if (currentClicks >= clicksRequired)
            {
                EndMinigame(true); 
            }
        }
    }
}