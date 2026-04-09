using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Inicio : MonoBehaviour
{
    public Image panelNegro; // Arrastra el Panel aquí
    public Color colorInicial = Color.white;
    public Color colorFinal = Color.black;

    void Start()
    {
        if (panelNegro != null)
        {
            StartCoroutine(SecuenciaFade());
        }
    }

    IEnumerator SecuenciaFade()
    {
        // 1. Espera inicial de 2 segundos
        yield return new WaitForSeconds(2f);

        // 2. Fundido a negro (durante 2 segundos más)
        float tiempo = 0;

        while (tiempo < 2f)
        {
            tiempo += Time.deltaTime;
            float alpha = tiempo / 2f; // Va de 0 a 1
            panelNegro.color = Color.Lerp(colorInicial, colorFinal, alpha);
            yield return null;
        }

        // 3. Cambio de escena al llegar a los 4s totales
        SceneManager.LoadScene("MainMenu");
    }
}
