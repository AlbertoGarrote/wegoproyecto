using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Llamamos al GameManager para que inicie el ciclo
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGameCycle();
        }
    }
}