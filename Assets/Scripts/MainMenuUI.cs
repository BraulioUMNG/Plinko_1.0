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
        // Asegurarse que el menú esté visible al inicio
        menuPanel.SetActive(true);

        // Texto invisible al inicio
        SetTextAlpha(instructionsText, 0f);
    }

    // Enlazado al botón Jugar en el Inspector
    public void OnPlayButtonPressed()
    {
        menuPanel.SetActive(false);

        // Lanzar animación de cámara e instrucciones en paralelo
        CameraAnimator.Instance.AnimateToGamePosition(OnCameraArrived);
        StartCoroutine(ShowInstructionsSequence());
    }

    private void OnCameraArrived()
    {
        // Habilitar el spawner de bolas solo cuando la cámara llegó
        if (ballSpawner != null)
            ballSpawner.enabled = true;
    }

    private IEnumerator ShowInstructionsSequence()
    {
        // Fade in
        yield return StartCoroutine(FadeText(instructionsText, 0f, 1f, instructionsFadeInDuration));

        // Permanecer visible
        yield return new WaitForSeconds(instructionsVisibleDuration);

        // Fade out
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
