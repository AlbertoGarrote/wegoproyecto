using TMPro;
using UnityEngine;

public class NoDesmayarseMinigame : MinigameBase
{
    [Header("Mecanica Mash")]
    public int clicksRequired = 20;
    public TextMeshProUGUI clicks;

    private int currentClicks = 0;

    protected override void Start()
    {
        base.Start();

        int currentScore = (GameManager.Instance != null) ? GameManager.Instance.score : 0;
        clicksRequired += currentScore * 2;

        currentClicks = 0;
        if (clicks != null) clicks.text = $"Clicks: {currentClicks}/{clicksRequired}";
    }

    protected override void Update()
    {
        base.Update();
        if (!isGameActive) return;

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
}