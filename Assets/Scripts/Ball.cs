using UnityEngine;

public class Ball : MonoBehaviour
{
    [Tooltip("Segundos antes de auto-destruirse si no llega al fondo")]
    public float lifetime = 15f;

    private BallSpawner _spawner;

    public void Init(BallSpawner spawner)
    {
        _spawner = spawner;
        Destroy(gameObject, lifetime);
    }

    void OnDestroy()
    {
        _spawner?.OnBallDestroyed();
    }

    // Destruir al tocar el colector del fondo (tagged "Collector")
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Collector"))
            Destroy(gameObject);
    }
}