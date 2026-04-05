using UnityEngine;
using System.Collections;

public class BallSpawner : MonoBehaviour
{
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
            0f
        );
        Vector3 pos = spawnPoint != null
            ? spawnPoint.position + offset
            : transform.position + offset;

        GameObject ball = Instantiate(ballPrefab, pos, Quaternion.identity);
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
        // Destruir todas las bolas activas
        foreach (var b in FindObjectsByType<Ball>(FindObjectsSortMode.None))
            Destroy(b.gameObject);
        _ballCount = 0;
    }
}