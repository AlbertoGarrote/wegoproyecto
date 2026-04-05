using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelfieMinigame : MinigameBase
{
    [Header("Ajustes del Alien")]
    public GameObject alien;
    public float speed = 5f;
    public Collider2D photoArea; // El collider del móvil en el centro

    [Header("Movimiento")]
    private Vector2 direction;
    private bool isAlienInside = false;

    protected override void Start()
    {
        base.Start();

        // Aumentar velocidad según el score para dificultad
        speed += GameManager.Instance.score * 0.5f;

        // Dirección inicial aleatoria
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        // Colocar al alien fuera del centro al empezar
        alien.transform.localPosition = new Vector3(-5, 0, 0);
    }

    protected override void Update()
    {
        base.Update();
        if (!isGameActive) return;

        MoverAlien();

        // Si hace click, comprobamos si el alien está dentro
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TomarFoto();
        }
    }

    void MoverAlien()
    {
        // Movimiento básico
        alien.transform.Translate(direction * speed * Time.deltaTime);

        // Rebote simple contra los bordes de la pantalla (ajusta los valores según tu cámara)
        Vector3 pos = Camera.main.WorldToViewportPoint(alien.transform.position);

        if (pos.x < 0.05f || pos.x > 0.95f) direction.x *= -1;
        if (pos.y < 0.05f || pos.y > 0.95f) direction.y *= -1;

        // Obligar al alien a pasar por el centro si el tiempo se agota
        // Si queda menos del 30% del tiempo, corregimos la trayectoria hacia el centro
        if (timer < maxTime * 0.3f)
        {
            Vector2 haciaElCentro = (photoArea.transform.position - alien.transform.position).normalized;
            direction = Vector2.Lerp(direction, haciaElCentro, Time.deltaTime * 2f);
        }
    }

    void TomarFoto()
    {
        // Usamos OverlapPoint o comprobamos la bandera que actualizan los Triggers
        if (isAlienInside)
        {
            Debug.Log("¡Foto capturada!");
            EndMinigame(true);
        }
        else
        {
            Debug.Log("Fallaste el disparo");
            // Opcional: Podrías terminar el juego aquí como derrota o dejar que siga intentándolo
            EndMinigame(false); 
        }
    }

    // Detección de si está en el área
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monstro")) isAlienInside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Monstro")) isAlienInside = false;
    }
}