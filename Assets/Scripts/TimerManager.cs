using System;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    [SerializeField] private float totalTime = 60f;

    public static event Action OnTimeOut;
    public static event Action<float> OnTimeTick;   // cada frame mientras corre

    public float Remaining { get; private set; }
    public float TotalTime => totalTime;

    private bool _running;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (!_running) return;

        Remaining -= Time.deltaTime;
        OnTimeTick?.Invoke(Remaining);

        if (Remaining <= 0f)
        {
            Remaining = 0f;
            _running = false;
            OnTimeOut?.Invoke();
        }
    }

    public void StartTimer()
    {
        Remaining = totalTime;
        _running = true;
    }

    public void Pause() => _running = false;
    public void Resume() => _running = true;
    public void Stop() { _running = false; Remaining = 0f; }

    /// <summary>Agrega o quita segundos como bonus/penalización.</summary>
    public void AddTime(float seconds)
    {
        Remaining = Mathf.Clamp(Remaining + seconds, 0f, totalTime);
    }
}