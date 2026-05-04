using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject menuPanel;
    public TMP_Text instructionsText;

    [Header("Instrucciones")]
    [Tooltip("Cuánto tarda en aparecer el texto de instrucciones")]
    public float instructionsFadeInDuration = 0.5f;
    [Tooltip("Cuánto tiempo permanece visible el texto")]
    public float instructionsVisibleDuration = 3f;
    [Tooltip("Cuánto tarda en desaparecer")]
    public float instructionsFadeOutDuration = 0.8f;

    [Header("Referencia")]
    public BallSpawner ballSpawner;

    void Start()
    {
        menuPanel.SetActive(true);
        SetTextAlpha(instructionsText, 0f);
    }

    public void OnPlayButtonPressed()
    {
        menuPanel.SetActive(false);
        CameraAnimator.Instance.AnimateToGamePosition(OnCameraArrived);
        StartCoroutine(ShowInstructionsSequence());
    }

    private void OnCameraArrived()
    {
        // Mostrar el GIF cuando la cámara terminó de moverse
        if (Gif_inicial.Instance != null)
            Gif_inicial.Instance.ShowGif();

        // Habilitar el spawner de bolas
        if (ballSpawner != null)
            ballSpawner.enabled = true;
    }

    private IEnumerator ShowInstructionsSequence()
    {
        yield return StartCoroutine(FadeText(instructionsText, 0f, 1f, instructionsFadeInDuration));
        yield return new WaitForSeconds(instructionsVisibleDuration);
        yield return StartCoroutine(FadeText(instructionsText, 1f, 0f, instructionsFadeOutDuration));
    }

    private IEnumerator FadeText(TMP_Text text, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetTextAlpha(text, Mathf.Lerp(from, to, t));
            yield return null;
        }
        SetTextAlpha(text, to);
    }

    private void SetTextAlpha(TMP_Text text, float alpha)
    {
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}