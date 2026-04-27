using UnityEngine;

public class Ball : MonoBehaviour
{
    [Tooltip("Segundos antes de auto-destruirse si cae fuera de la escena")]
    public float fallOutLifetime = 20f;

    private BallSpawner _spawner;
    private bool _inBowl = false;

    public void Init(BallSpawner spawner)
    {
        _spawner = spawner;
    }

    void OnDestroy()
    {
        if (!_inBowl)
            _spawner?.OnBallDestroyed();
    }

    void OnCollisionEnter(Collision col)
    {
        // Si toca algo taggeado "OutOfBounds" se destruye
        if (col.gameObject.CompareTag("OutOfBounds"))
            Destroy(gameObject);
    }

    // Llamado por BowlManager para registrar que esta bola llegó
    public void MarkAsInBowl()
    {
        if (_inBowl) return;
        _inBowl = true;
        // Liberar el slot inmediatamente al llegar al bowl
        _spawner?.OnBallReachedBowl();
    }

    public bool IsInBowl() => _inBowl;
}