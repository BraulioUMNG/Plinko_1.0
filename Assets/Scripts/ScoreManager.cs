using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public static event Action<int> OnScoreChanged;
    public static event Action<float> OnMultiplierChanged;

    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public float Multiplier { get; private set; } = 1f;

    const string HighScoreKey = "PlinkoHighScore";

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    // ── API pública ──────────────────────────────────────────

    /// <summary>Llamar desde cada zona multiplicadora al detectar una bola.</summary>
    public void AddScore(int basePoints)
    {
        if (GameManager.Instance.State != GameState.Playing) return;

        int earned = Mathf.RoundToInt(basePoints * Multiplier);
        Score += earned;

        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
        }

        OnScoreChanged?.Invoke(Score);
    }

    /// <summary>Llamar desde las zonas que cambian el multiplicador.</summary>
    public void SetMultiplier(float value)
    {
        Multiplier = Mathf.Max(1f, value);
        OnMultiplierChanged?.Invoke(Multiplier);
    }

    public void Reset()
    {
        Score = 0;
        Multiplier = 1f;
        OnScoreChanged?.Invoke(Score);
        OnMultiplierChanged?.Invoke(Multiplier);
    }
}