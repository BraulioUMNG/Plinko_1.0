using System.Collections;
using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
    public static CameraAnimator Instance { get; private set; }

    [Header("Posiciones")]
    public Transform menuCamPos;
    public Transform gameCamPos;

    [Header("Animación")]
    [Tooltip("Duración total de la animación en segundos")]
    public float animationDuration = 2.5f;
    [Tooltip("Curva de suavizado del movimiento")]
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Camera _cam;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _cam = Camera.main;
    }

    void Start()
    {
        // Al iniciar, poner la cámara en posición de menú
        if (menuCamPos != null)
        {
            _cam.transform.position = menuCamPos.position;
            _cam.transform.rotation = menuCamPos.rotation;
        }
    }

    // Llamado desde el botón Jugar
    public void AnimateToGamePosition(System.Action onComplete = null)
    {
        StartCoroutine(MoveCamera(menuCamPos, gameCamPos, onComplete));
    }

    private IEnumerator MoveCamera(Transform from, Transform to, System.Action onComplete)
    {
        float elapsed = 0f;

        Vector3 startPos = from.position;
        Quaternion startRot = from.rotation;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            float curvedT = easeCurve.Evaluate(t);

            _cam.transform.position = Vector3.Lerp(startPos, to.position, curvedT);
            _cam.transform.rotation = Quaternion.Slerp(startRot, to.rotation, curvedT);

            yield return null;
        }

        // Asegurar posición exacta al final
        _cam.transform.position = to.position;
        _cam.transform.rotation = to.rotation;

        onComplete?.Invoke();
    }
}