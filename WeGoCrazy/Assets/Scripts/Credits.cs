using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform objetoAMover; // Arrastra aquí el texto de los créditos

    [Header("Configuración")]
    public float velocidad = 50f;

    void Update()
    {
        if (objetoAMover != null)
        {
            // Mueve el objeto hacia arriba
            objetoAMover.anchoredPosition += new Vector2(0, velocidad * Time.deltaTime);

        }

        // Si el jugador pulsa Escape también vuelve (opcional)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            VolverAlMenu();
        }
    }

    // ESTA FUNCIÓN ES PARA EL BOTÓN
    public void VolverAlMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}