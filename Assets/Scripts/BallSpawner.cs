using UnityEngine;
using System.Collections;

public class BallSpawner : MonoBehaviour
{

    public static BallSpawner Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    [Header("Referencias")]
    public GameObject ballPrefab;
    public Transform spawnPoint;

    [Header("Umbrales de volumen")]
    [Tooltip("Volumen mínimo para spawnear 1 bola")]
    public float thresholdLow = 0.02f;
    [Tooltip("Volumen para spawnear múltiples bolas")]
    public float thresholdHigh = 0.08f;

    [Header("Spawn")]
    [Tooltip("Máximo de bolas en escena al mismo tiempo")]
    public int maxBalls = 60;
    [Tooltip("Segundos entre spawns (cooldown mínimo)")]
    public float spawnCooldown = 0.15f;
    [Tooltip("Radio de dispersión horizontal al spawnear")]
    public float spawnSpread = 0.5f;

    private float _cooldownTimer = 0f;
    private int _ballCount = 0;

    void Update()
    {
        if (PauseMenu.IsPaused) return;

        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer > 0f) return;

        float loudness = MicrophoneInput.Instance != null
            ? MicrophoneInput.Instance.Loudness
            : 0f;

        if (loudness < thresholdLow) return;
        if (_ballCount >= maxBalls) return;

        // Cuántas bolas spawnear según volumen
        int count = loudness >= thresholdHigh ? 3 : 1;
        count = Mathf.Min(count, maxBalls - _ballCount);

        for (int i = 0; i < count; i++)
            SpawnBall();

        _cooldownTimer = spawnCooldown;
    }

    void SpawnBall()
    {
        Vector3 offset = new Vector3(
            Random.Range(-spawnSpread, spawnSpread),
            0f,
            Random.Range(-spawnSpread, spawnSpread) // <-- esto es lo nuevo
        );

        Vector3 pos = spawnPoint != null
            ? spawnPoint.position + offset
            : transform.position + offset;

        GameObject ball = Instantiate(ballPrefab, pos, Quaternion.identity);

        // Darle un pequeño impulso inicial en Z también
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 initialForce = new Vector3(
                Random.Range(-0.5f, 0.5f),
                0f,
                Random.Range(-1f, 1f)  // impulso en Z
            );
            rb.AddForce(initialForce, ForceMode.Impulse);
        }

        ball.GetComponent<Ball>()?.Init(this);
        _ballCount++;
    }

    // Llamado por Ball.cs cuando la bola es destruida
    public void OnBallDestroyed()
    {
        _ballCount = Mathf.Max(0, _ballCount - 1);
    }

    public void ResetBalls()
    {
        foreach (var b in FindObjectsByType<Ball>(FindObjectsSortMode.None))
            Destroy(b.gameObject);
        _ballCount = 0;

        // También limpiar el bowl
        BowlManager.Instance?.ResetBowl();
    }

    // Llamado por los multiplicadores — spawna una bola en una posición específica
    public void SpawnBallAt(Vector3 position)
    {
        if (_ballCount >= maxBalls) return;

        // Pequeña dispersión para que no salgan todas en el mismo punto
        Vector3 offset = new Vector3(
            Random.Range(-0.2f, 0.2f),
            0f,
            Random.Range(-0.2f, 0.2f)
        );

        GameObject ball = Instantiate(ballPrefab, position + offset, Quaternion.identity);

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float zForce = 0.1f;
            rb.linearVelocity = new Vector3(0f, 0f, zForce);
        }

        ball.GetComponent<Ball>()?.Init(this);
        _ballCount++;
    }

}