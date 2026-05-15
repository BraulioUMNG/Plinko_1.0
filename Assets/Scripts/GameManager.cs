using System;
using UnityEngine;

public enum GameState { Idle, Playing, Paused, GameOver, Win }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Condiciones")]
    [SerializeField] private int scoreToWin = 500;
    [SerializeField] private int maxBallsAllowed = 20;   // -1 = ilimitado

    public GameState State { get; private set; } = GameState.Idle;

    // Eventos
    public static event Action<GameState> OnStateChanged;
    public static event Action OnGameOver;
    public static event Action OnWin;

    private int _ballsSpawned = 0;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnEnable()
    {
        ScoreManager.OnScoreChanged += CheckWinCondition;
        TimerManager.OnTimeOut += TriggerGameOver;
    }

    void OnDisable()
    {
        ScoreManager.OnScoreChanged -= CheckWinCondition;
        TimerManager.OnTimeOut -= TriggerGameOver;
    }

    // ── API pública ──────────────────────────────────────────

    public void StartGame()
    {
        _ballsSpawned = 0;
        ScoreManager.Instance.Reset();
        TimerManager.Instance.StartTimer();
        SetState(GameState.Playing);
    }

    public void PauseGame()
    {
        if (State != GameState.Playing) return;
        TimerManager.Instance.Pause();
        Time.timeScale = 0f;
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (State != GameState.Paused) return;
        TimerManager.Instance.Resume();
        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        StartGame();
    }

    /// <summary>Llamar desde BallSpawner cada vez que nace una bola.</summary>
    public bool RegisterBallSpawn()
    {
        if (State != GameState.Playing) return false;
        if (maxBallsAllowed > 0 && _ballsSpawned >= maxBallsAllowed)
        {
            TriggerGameOver();
            return false;
        }
        _ballsSpawned++;
        return true;
    }

    public int BallsSpawned => _ballsSpawned;
    public int MaxBalls => maxBallsAllowed;

    // ── Internos ─────────────────────────────────────────────

    void SetState(GameState next)
    {
        State = next;
        OnStateChanged?.Invoke(next);
    }

    void CheckWinCondition(int score)
    {
        if (State != GameState.Playing) return;
        if (scoreToWin > 0 && score >= scoreToWin)
        {
            TimerManager.Instance.Stop();
            SetState(GameState.Win);
            OnWin?.Invoke();
        }
    }

    void TriggerGameOver()
    {
        if (State != GameState.Playing) return;
        TimerManager.Instance.Stop();
        Time.timeScale = 0f;
        SetState(GameState.GameOver);
        OnGameOver?.Invoke();
    }
}
