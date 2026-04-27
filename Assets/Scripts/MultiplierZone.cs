using UnityEngine;
using TMPro;

public class MultiplierZone : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Cuántas bolas extra genera al contacto (2 = duplica, 3 = triplica)")]
    public int multiplierValue = 2;
    [Tooltip("Segundos de cooldown entre activaciones de esta zona")]
    public float cooldown = 0.3f;

    [Header("Visual")]
    public TMP_Text multiplierLabel;
    public Renderer zoneRenderer;
    public Color normalColor = new Color(0.2f, 0.6f, 1f, 0.5f);
    public Color flashColor = new Color(1f, 0.9f, 0.2f, 0.9f);
    [Tooltip("Duración del flash visual al activarse")]
    public float flashDuration = 0.15f;

    private float _cooldownTimer = 0f;
    private bool _flashing = false;

    void Start()
    {
        // Mostrar el número en el label
        if (multiplierLabel != null)
            multiplierLabel.text = $"x{multiplierValue}";

        // Color inicial
        if (zoneRenderer != null)
            zoneRenderer.material.color = normalColor;
    }

    void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Multiplier] Trigger tocado por: {other.gameObject.name}");

        if (_cooldownTimer > 0f) return;

        Ball ball = other.GetComponent<Ball>();
        if (ball == null)
        {
            Debug.Log("[Multiplier] El objeto NO tiene componente Ball");
            return;
        }

        Debug.Log($"[Multiplier] Bola detectada, spawneando x{multiplierValue - 1} extra");

        int extraBalls = multiplierValue - 1;
        for (int i = 0; i < extraBalls; i++)
            BallSpawner.Instance.SpawnBallAt(other.transform.position);

        _cooldownTimer = cooldown;

        if (!_flashing)
            StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        _flashing = true;
        if (zoneRenderer != null)
            zoneRenderer.material.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        if (zoneRenderer != null)
            zoneRenderer.material.color = normalColor;
        _flashing = false;
    }
}
