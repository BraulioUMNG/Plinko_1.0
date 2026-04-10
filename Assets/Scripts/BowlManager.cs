using UnityEngine;

public class BowlManager : MonoBehaviour
{
    public static BowlManager Instance { get; private set; }

    [Header("Configuración")]
    [Tooltip("Cantidad mínima de bolas para ganar — ajustar después de pruebas")]
    public int ballsRequiredToWin = 50;

    private int _ballsInBowl = 0;
    public int BallsInBowl => _ballsInBowl;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Detectar bolas que entran al bowl por el área del fondo
    void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball == null) return;
        if (ball.IsInBowl()) return; // evitar doble conteo

        ball.MarkAsInBowl();
        _ballsInBowl++;

        Debug.Log($"[BowlManager] Bolas en bowl: {_ballsInBowl}");
    }

    public bool HasEnoughBalls() => _ballsInBowl >= ballsRequiredToWin;

    public void ResetBowl()
    {
        _ballsInBowl = 0;
        // Destruir todas las bolas que estén en el bowl
        foreach (var ball in FindObjectsByType<Ball>(FindObjectsSortMode.None))
        {
            if (ball.IsInBowl())
                Destroy(ball.gameObject);
        }
    }
}
