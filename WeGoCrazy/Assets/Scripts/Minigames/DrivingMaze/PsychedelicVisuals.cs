using UnityEngine;

public class PsychedelicVisuals : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    
    [Header("Settings (Mínimos)")]
    public float cur_maxCameraShake = 0.08f;  // Aumentado para que sea molesto
    public float cur_maxFovChange = 0.4f;     // Un poco más de "respiración"
    public float cur_maxRotationTilt = 1.5f;   // Nuevo: rotación molesta
    public float cur_shakeSpeed = 4.0f;       // Más rápido = más molesto
    
    private float currentHallucinationIntensity = 0f;
    private Vector3 originalCameraPos;
    private float originalFov;
    private Quaternion originalRotation;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraPos = mainCamera.transform.localPosition;
            originalRotation = mainCamera.transform.localRotation;
            originalFov = mainCamera.orthographic ? mainCamera.orthographicSize : mainCamera.fieldOfView;
        }
    }

    public void UpdateHallucinationVisuals(float intensity)
    {
        // Quitamos el límite! Solo evitamos números negativos
        currentHallucinationIntensity = Mathf.Max(0f, intensity);
    }

    void Update()
    {
        if (mainCamera == null || currentHallucinationIntensity <= 0) return;

        // Movimiento de posición (Shake)
        float shakeX = (Mathf.PerlinNoise(Time.time * cur_shakeSpeed, 0f) - 0.5f) * 2f;
        float shakeY = (Mathf.PerlinNoise(0f, Time.time * cur_shakeSpeed) - 0.5f) * 2f;

        float shakeMagnitude = currentHallucinationIntensity * cur_maxCameraShake;
        mainCamera.transform.localPosition = originalCameraPos + new Vector3(shakeX * shakeMagnitude, shakeY * shakeMagnitude, 0);

        // Rotación (Tilt) - ¡Si llega muy alto dará vueltas de campana!
        float tilt = (Mathf.PerlinNoise(Time.time * (cur_shakeSpeed * 0.5f), 10f) - 0.5f) * 2f;
        mainCamera.transform.localRotation = originalRotation * Quaternion.Euler(0, 0, tilt * currentHallucinationIntensity * cur_maxRotationTilt);

        // FOV (Breathing)
        float fovBreath = Mathf.Sin(Time.time * 3f) * (currentHallucinationIntensity * cur_maxFovChange);
        
        if (mainCamera.orthographic)
        {
            // Protegemos que el tamaño no baje de 0.1 o Unity daría error crítico
            mainCamera.orthographicSize = Mathf.Max(0.1f, originalFov + fovBreath);
        }
        else
        {
            mainCamera.fieldOfView = Mathf.Max(0.1f, originalFov + fovBreath);
        }
    }
    
    public void ResetVisuals()
    {
        currentHallucinationIntensity = 0f;
        if (mainCamera != null)
        {
            mainCamera.transform.localPosition = originalCameraPos;
            mainCamera.transform.localRotation = originalRotation;
            if (mainCamera.orthographic) mainCamera.orthographicSize = originalFov;
            else mainCamera.fieldOfView = originalFov;
        }
    }
}
