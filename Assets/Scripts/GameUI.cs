using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("HUD en juego")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text multiplierText;
    [SerializeField] private TMP_Text ballCountText;
    [SerializeField] private Slider timerSlider;

    [Header("Pantallas")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;

    [Header("Resultados")]
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private TMP_Text winScoreText;

    void OnEnable()
    {
        GameManager.OnStateChanged += HandleStateChange;
        ScoreManager.OnScoreChanged += UpdateScore;
        ScoreManager.OnMultiplierChanged += UpdateMultiplier;
        TimerManager.OnTimeTick += UpdateTimer;
    }

    void OnDisable()
    {
        GameManager.OnStateChanged -= HandleStateChange;
        ScoreManager.OnScoreChanged -= UpdateScore;
        ScoreManager.OnMultiplierChanged -= UpdateMultiplier;
        TimerManager.OnTimeTick -= UpdateTimer;
    }

    void Start() => ShowPanel(startPanel);

    // ── Handlers ─────────────────────────────────────────────

    void HandleStateChange(GameState state)
    {
        hudPanel.SetActive(state == GameState.Playing || state == GameState.Paused);

        ShowPanel(state switch
        {
            GameState.Idle => startPanel,
            GameState.Paused => pausePanel,
            GameState.GameOver => gameOverPanel,
            GameState.Win => winPanel,
            _ => null
        });

        if (state == GameState.GameOver && gameOverScoreText)
            gameOverScoreText.text = $"Puntaje: {ScoreManager.Instance.Score}";

        if (state == GameState.Win && winScoreText)
            winScoreText.text = $"¡{ScoreManager.Instance.Score} pts!";
    }

    void UpdateScore(int score)
    {
        if (scoreText) scoreText.text = score.ToString("N0");
        if (highScoreText) highScoreText.text = $"Mejor: {ScoreManager.Instance.HighScore:N0}";

        int spawned = GameManager.Instance.BallsSpawned;
        int max = GameManager.Instance.MaxBalls;
        if (ballCountText)
            ballCountText.text = max > 0 ? $"{spawned}/{max}" : spawned.ToString();
    }

    void UpdateMultiplier(float m)
    {
        if (multiplierText) multiplierText.text = $"x{m:F1}";
    }

    void UpdateTimer(float remaining)
    {
        int m = Mathf.FloorToInt(remaining / 60f);
        int s = Mathf.FloorToInt(remaining % 60f);
        if (timerText) timerText.text = $"{m:00}:{s:00}";
        if (timerSlider)
        {
            timerSlider.value = remaining / TimerManager.Instance.TotalTime;
            // Rojo cuando queda menos del 20%
            var fill = timerSlider.fillRect?.GetComponent<Image>();
            if (fill) fill.color = remaining < TimerManager.Instance.TotalTime * 0.2f
                ? Color.red : Color.white;
        }
    }

    // ── Helpers ──────────────────────────────────────────────

    void ShowPanel(GameObject target)
    {
        foreach (var p in new[] { startPanel, pausePanel, gameOverPanel, winPanel })
            if (p) p.SetActive(p == target);
    }

    // ── Botones (asignar en Inspector) ────────────────────────
    public void OnStartButton() => GameManager.Instance.StartGame();
    public void OnPauseButton() => GameManager.Instance.PauseGame();
    public void OnResumeButton() => GameManager.Instance.ResumeGame();
    public void OnRestartButton() => GameManager.Instance.RestartGame();
}