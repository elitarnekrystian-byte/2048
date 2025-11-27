using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TileBoard tileBoard;
    public CanvasGroup gameOver;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestText;

    private int score;

    public ExtremeCameraShake cameraShake;

    // --- PROGI GRY ---
    [Header("Progi Gry")]
    [Tooltip("Wynik, przy którym w³¹cza siê trzêsienie kamery i zaczyna skalowanie.")]
    public int shakeThreshold = 50;

    // --- SKALOWANIE SZYBKOŒCI ---
    [Header("Skalowanie Czêstotliwoœci (Szybkoœci)")]
    [Tooltip("Pocz¹tkowy, wolny interwa³ dla progu 50pkt (np. 0.3s).")]
    public float baseShakeInterval = 0.3f;
    [Tooltip("Wartoœæ, o jak¹ interwa³ jest ZMNIEJSZANY co 50 punktów.")]
    public float intervalDecreaseStep = 0.07f; // Przyspieszenie o 0.07
    [Tooltip("Minimalna szybkoœæ, jak¹ mo¿emy osi¹gn¹æ (np. 0.02s).")]
    public float minShakeInterval = 0.02f;

    // --- SKALOWANIE SI£Y (INTENSYWNOŒCI) ---
    [Header("Skalowanie Si³y (Intensywnoœci)")]
    [Tooltip("Minimalna intensywnoœæ pozycji dla progu 50 pkt.")]
    public float basePositionMagnitude = 0.005f;
    [Tooltip("Intensywnoœæ pozycji dodawana na ka¿de kolejne 50 punktów.")]
    public float positionMagnitudeStep = 0.15f;

    [Space]
    [Tooltip("Minimalna intensywnoœæ rotacji dla progu 50 pkt.")]
    public float baseRotationMagnitude = 0.05f;
    [Tooltip("Intensywnoœæ rotacji dodawana na ka¿de kolejne 50 punktów.")]
    public float rotationMagnitudeStep = 1.0f;
    // -------------------------------------------------------------------------

    private void Start()
    {
        NewGame();

        if (cameraShake != null)
        {
            cameraShake.StopShaking();
            cameraShake.SetShakeInterval(baseShakeInterval);
            cameraShake.SetShakeIntensity(0f, 0f); // Reset intensywnoœci
        }
    }

    /// <summary>
    /// Nowa metoda Update nas³uchuj¹ca wciœniêcia klawisza 'R'
    /// </summary>
    private void Update()
    {
        // SprawdŸ, czy klawisz 'R' zosta³ wciœniêty
        if (Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
    }

    public void NewGame()
    {
        SetScore(0);

        if (bestText != null)
        {
            bestText.text = LoadBestScore().ToString();
        }

        gameOver.alpha = 0f;
        gameOver.interactable = false;

        tileBoard.ClearBoard();
        tileBoard.CreateTile();
        tileBoard.CreateTile();
        tileBoard.enabled = true;

        if (cameraShake != null)
        {
            cameraShake.StopShaking();
            cameraShake.SetShakeInterval(baseShakeInterval);
            cameraShake.SetShakeIntensity(0f, 0f); // Reset intensywnoœci
        }
    }

    public void GameOver()
    {
        tileBoard.enabled = false;
        gameOver.interactable = true;
        StartCoroutine(Fade(gameOver, 1f, 1f));

        if (cameraShake != null)
        {
            cameraShake.StopShaking();
            cameraShake.SetShakeInterval(baseShakeInterval);
            cameraShake.SetShakeIntensity(0f, 0f);
        }
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float toAlpha, float delay)
    {
        yield return new WaitForSeconds(delay);
        float elapsedTime = 0f;
        float duration = 0.5f;
        float startAlpha = canvasGroup.alpha;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, toAlpha, elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = toAlpha;
    }

    private void SetScore(int score)
    {
        this.score = score;

        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        if (cameraShake != null)
        {
            // JEDNA METODA ZARZ¥DZA AKTYWACJ¥, SI£¥ I SZYBKOŒCI¥
            UpdateCameraShakeEffects(score);
        }

        SaveBestScore();
    }

    private void SaveBestScore()
    {
        int bestScore = LoadBestScore();
        if (score > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", score);
        }
    }

    private int LoadBestScore()
    {
        return PlayerPrefs.GetInt("BestScore", 0);
    }

    public void UpdateScore(int points)
    {
        SetScore(score + points);
    }

    /// <summary>
    /// Zarz¹dza aktywacj¹, szybkoœci¹ i intensywnoœci¹ trzêsienia kamery.
    /// </summary>
    private void UpdateCameraShakeEffects(int currentScore)
    {
        if (cameraShake == null) return;

        if (currentScore >= shakeThreshold) // WARUNEK AKTYWACJI (>= 50)
        {
            int stepCount = (currentScore / shakeThreshold);

            // =========================
            // 1. SKALOWANIE SZYBKOŒCI
            // =========================
            float newInterval = baseShakeInterval - (stepCount * intervalDecreaseStep);
            newInterval = Mathf.Max(newInterval, minShakeInterval);
            cameraShake.SetShakeInterval(newInterval);

            // =========================
            // 2. SKALOWANIE SI£Y (DLA P£YNNEGO NARASTANIA)
            // =========================
            float newPosMag = basePositionMagnitude + (stepCount * positionMagnitudeStep);
            float newRotMag = baseRotationMagnitude + (stepCount * rotationMagnitudeStep);
            cameraShake.SetShakeIntensity(newPosMag, newRotMag); // Ustawiamy docelow¹ si³ê, do której Lerp d¹¿y

            // =========================
            // 3. AKTYWACJA
            // =========================
            cameraShake.StartShaking();
        }
        else
        {
            // PONI¯EJ PROGU - RESET I WY£¥CZENIE
            cameraShake.StopShaking();
            cameraShake.SetShakeInterval(baseShakeInterval);
            cameraShake.SetShakeIntensity(0f, 0f); // Ustawiamy docelow¹ si³ê na 0, aby Lerp p³ynnie zanik³
        }
    }
}