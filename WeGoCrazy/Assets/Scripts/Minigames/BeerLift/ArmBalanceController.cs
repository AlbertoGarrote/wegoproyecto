using UnityEngine;

namespace WeGoCrazy.Minigames.BeerLift
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ArmBalanceController : MonoBehaviour
    {
        public BeerLiftManager manager;
        
        [Header("Physics (QWOP style)")]
        public float balanceForce = 1500f;
        public float liftForce = 25f;
        public float armFallGravity = 10f;
        
        [Header("Shroom Effects")]
        public float randomSwayFrequency = 1.5f;
        public float baseSwayAmplitude = 400f; // El desequilibrio inicial garantizado (más bajito)
        public float extraSwayPerIntensity = 50f; // La fuerza brutal que se irá sumando a medida que ganes rondas
        
        private Rigidbody2D rb;
        private float noiseOffset;
        private float originalY; // Guardar dónde pusiste la cerveza originalmente

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // ¡Fuerza la gravedad a 0 para que no se caiga al infinito visualmente!
            noiseOffset = Random.Range(0f, 1000f);
            originalY = transform.position.y; // Memoria de su posición base
        }

        void Update()
        {
            // Efecto de desequilibrio: Sumamos la intensidad global que el GameManager gestiona tras cada victoria
            float intensity = GameManager.Instance != null ? GameManager.Instance.globalHallucinationIntensity : 0f;
            
            float shroomSway = (Mathf.PerlinNoise(Time.time * randomSwayFrequency, noiseOffset) - 0.5f) * 2f;
            
            // El total de fuerza es: La base (250f) + el extra brutal multiplicado por cuántas rondas hayas ganado
            float currentSwayAmplitude = baseSwayAmplitude + (extraSwayPerIntensity * intensity);
            
            rb.AddTorque(shroomSway * currentSwayAmplitude * Time.deltaTime);

            // Balanceo manual del gymbro
            float balanceInput = 0f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) balanceInput = 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) balanceInput = -1f;

            rb.AddTorque(balanceInput * balanceForce * Time.deltaTime);

            // Levantar la cerveza
            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                manager?.AddLift(liftForce * Time.deltaTime);
            }
            else
            {
                // Si dejas de tensar el brazo, el progreso cae
                manager?.AddLift(-armFallGravity * Time.deltaTime);
            }

            // --- MAGIA VISUAL COMPLETA ---
            if (manager != null)
            {
                // Calcula el límite inferior real (un poco por debajo de su posición inicial)
                float dropLimit = originalY - 2.5f; 
                // Calcula la altura máxima al ganar
                float topLimit = originalY + 4.5f; 
                
                // Mueve la cerveza fluidamente entre ambos puntos según tu porcentaje
                float visualY = Mathf.Lerp(dropLimit, topLimit, manager.liftProgress / manager.maxLift);
                transform.position = new Vector3(transform.position.x, visualY, transform.position.z);
            }

            // Normalizar el ángulo y comprobar derrame
            float currentAngle = transform.eulerAngles.z;
            if (currentAngle > 180) currentAngle -= 360;

            manager?.CheckSpill(currentAngle);
        }
    }
}
