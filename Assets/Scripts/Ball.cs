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
        _inBowl = true;
        // Cancelar el auto-destroy — esta bola ya llegó a donde tiene que estar
        CancelInvoke();
    }

    public bool IsInBowl() => _inBowl;
}