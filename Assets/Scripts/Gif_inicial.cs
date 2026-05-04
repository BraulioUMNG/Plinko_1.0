using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Gif_inicial : MonoBehaviour
{
    public static Gif_inicial Instance { get; private set; }

    [Header("Referencia al GIF")]
    public Image gifImage;

    [Header("Frames de la animación")]
    public Sprite[] frames;

    [Tooltip("Fotogramas por segundo")]
    public float fps = 12f;

    [Header("Posición y tamaño en pantalla")]
    public float offsetFromBottom = 120f;
    public float gifWidth = 600f;
    public float gifHeight = 300f;

    [Header("Micrófono")]
    public float soundThreshold = 0.02f;

    [Header("Fade")]
    public float fadeOutDuration = 0.5f;

    private CanvasGroup _canvasGroup;
    private bool _listening = false;
    private bool _fadingOut = false;
    private int _currentFrame = 0;
    private float _frameTimer = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (gifImage != null)
        {
            gifImage.gameObject.SetActive(false);

            _canvasGroup = gifImage.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gifImage.gameObject.AddComponent<CanvasGroup>();

            _canvasGroup.alpha = 0f;

            RectTransform rt = gifImage.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 0f);
                rt.anchorMax = new Vector2(0.5f, 0f);
                rt.pivot = new Vector2(0.5f, 0f);
                rt.anchoredPosition = new Vector2(0f, offsetFromBottom);
                rt.sizeDelta = new Vector2(gifWidth, gifHeight);
            }
        }
    }

    public void ShowGif()
    {
        if (gifImage == null) return;
        gifImage.gameObject.SetActive(true);
        _fadingOut = false;
        _currentFrame = 0;
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        _canvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < 0.4f)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Clamp01(elapsed / 0.4f);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
        _listening = true;
        Debug.Log("[Gif_inicial] Escuchando micrófono...");
    }

    void Update()
    {
        // Animar frames
        if (gifImage != null && gifImage.gameObject.activeSelf && frames != null && frames.Length > 0)
        {
            _frameTimer += Time.deltaTime;
            if (_frameTimer >= 1f / fps)
            {
                _frameTimer = 0f;
                _currentFrame = (_currentFrame + 1) % frames.Length;
                gifImage.sprite = frames[_currentFrame];
            }
        }

        // Escuchar micrófono
        if (!_listening || _fadingOut) return;

        if (MicrophoneInput.Instance == null)
        {
            Debug.LogWarning("[Gif_inicial] MicrophoneInput.Instance es null");
            return;
        }

        float loudness = MicrophoneInput.Instance.Loudness;
        Debug.Log($"[Gif_inicial] Loudness: {loudness}");

        if (loudness >= soundThreshold)
        {
            Debug.Log("[Gif_inicial] Sonido detectado — ocultando GIF");
            _listening = false;
            _fadingOut = true;
            StartCoroutine(FadeOutAndHide());
        }
    }

    private IEnumerator FadeOutAndHide()
    {
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        gifImage.gameObject.SetActive(false);
        Debug.Log("[Gif_inicial] GIF ocultado");
    }
}