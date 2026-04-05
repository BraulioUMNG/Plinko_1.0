using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    public static MicrophoneInput Instance { get; private set; }

    [Header("Configuración")]
    [Tooltip("Índice del micrófono a usar (0 = predeterminado)")]
    public int microphoneIndex = 0;
    [Tooltip("Cuántas muestras se analizan por frame")]
    public int sampleWindow = 128;

    private AudioClip _micClip;
    private string _micDevice;
    public float Loudness { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("[MicrophoneInput] No se encontró micrófono.");
            return;
        }

        _micDevice = Microphone.devices[microphoneIndex];
        Debug.Log($"[MicrophoneInput] Usando: {_micDevice}");

        // Grabación en loop, 1 segundo de buffer, 44100 Hz
        _micClip = Microphone.Start(_micDevice, true, 1, 44100);

        // Espera a que el micrófono empiece
        while (!(Microphone.GetPosition(_micDevice) > 0)) { }
    }

    void Update()
    {
        Loudness = GetLoudnessFromMic();
    }

    float GetLoudnessFromMic()
    {
        if (_micClip == null) return 0f;

        int micPos = Microphone.GetPosition(_micDevice) - sampleWindow;
        if (micPos < 0) return 0f;

        float[] samples = new float[sampleWindow];
        _micClip.GetData(samples, micPos);

        float sum = 0f;
        foreach (float s in samples)
            sum += Mathf.Abs(s);

        return sum / sampleWindow; // valor entre 0 y ~0.5
    }

    void OnDestroy()
    {
        if (_micDevice != null && Microphone.IsRecording(_micDevice))
            Microphone.End(_micDevice);
    }
}
