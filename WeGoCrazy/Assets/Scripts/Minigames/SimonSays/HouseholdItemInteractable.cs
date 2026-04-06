// Force Recompile
using UnityEngine;
using UnityEngine.Events;

namespace WeGoCrazy.Minigames.SimonSays
{
    public class HouseholdItemInteractable : MonoBehaviour
    {
        public int itemId;
        public string hallucinationQuote = "¡Cómeme, soy una patata mágica!";
        
        [Header("Effects")]
        public Color neonColor = Color.green;
        public float lightDuration = 0.5f;

        [Header("Events")]
        public UnityEvent<int> onItemInteracted;

        private Renderer objectRenderer;
        private Color originalColor;
        private float lightTimer = 0f;

        void Awake()
        {
            objectRenderer = GetComponentInChildren<Renderer>();
            if (objectRenderer != null)
            {
                originalColor = objectRenderer.material.color;
            }
        }

        void Update()
        {
            // Restablecer el efecto de alucinación tras la duración
            if (lightTimer > 0)
            {
                lightTimer -= Time.deltaTime;
                if (lightTimer <= 0 && objectRenderer != null)
                {
                    objectRenderer.material.color = originalColor; // Restablecer al color normal
                }
            }
        }

        public void Interact()
        {
            PlayEffect();
            onItemInteracted?.Invoke(itemId);
        }

        // --- AÑADIDO: Detectar clic del ratón ---
        private void OnMouseDown()
        {
            Interact();
        }

        public void PlayEffect()
        {
            Debug.Log($"[{gameObject.name}] Brilla parpadeante en color {neonColor}");
            
            if (objectRenderer != null)
            {
                objectRenderer.material.color = neonColor;
            }
            lightTimer = lightDuration;
        }
    }
}
