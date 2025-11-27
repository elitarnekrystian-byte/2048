using UnityEngine;

public class ExtremeCameraShake : MonoBehaviour
{
    // --- G³ówne Ustawienia Intensywnoœci (ZARZ¥DZANE PRZEZ GAMEMANAGER) ---
    [Header("Intensywnoœæ Szarpania")]
    public float positionMagnitude = 0f; // Docelowa si³a (ustawiana przez GM)
    public float rotationMagnitude = 0f; // Docelowa rotacja (ustawiana przez GM)

    [Space]
    [Tooltip("Czêstotliwoœæ odœwie¿ania szarpania (im ni¿sza wartoœæ, tym szybciej).")]
    public float shakeInterval = 0.02f;

    [Tooltip("Szybkoœæ, z jak¹ intensywnoœæ trzêsienia narasta/zanika (im wy¿sza, tym p³ynniejsze przejœcie).")]
    public float intensityLerpSpeed = 5f; // Szybkoœæ p³ynnego przejœcia

    // --- Kontrola Stanu ---
    [Header("Kontrola Stanu")]
    public bool isShaking = false;

    private float timeSinceLastChange;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // --- Zmienne u¿ywane do aktualnego trzêsienia (p³ynnie d¹¿¹ do docelowych Magnitude) ---
    private float currentPositionMagnitude;
    private float currentRotationMagnitude;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        currentPositionMagnitude = 0f;
        currentRotationMagnitude = 0f;

        if (!isShaking)
        {
            transform.localPosition = initialPosition;
            transform.localRotation = initialRotation;
        }
    }

    void Update()
    {
        // KLUCZ DO P£YNNOŒCI (SMOOTH): Bie¿¹ca si³a p³ynnie d¹¿y do docelowej si³y ustawionej przez GameManager
        currentPositionMagnitude = Mathf.Lerp(currentPositionMagnitude, positionMagnitude, Time.deltaTime * intensityLerpSpeed);
        currentRotationMagnitude = Mathf.Lerp(currentRotationMagnitude, rotationMagnitude, Time.deltaTime * intensityLerpSpeed);


        if (isShaking)
        {
            timeSinceLastChange += Time.deltaTime;

            if (timeSinceLastChange >= shakeInterval)
            {
                ApplyExtremeJitter();
                timeSinceLastChange = 0f;
            }
        }
        else if (transform.localPosition != initialPosition || transform.localRotation != initialRotation)
        {
            // P³ynny powrót do bazowej pozycji po wy³¹czeniu trzêsienia
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * 5f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, initialRotation, Time.deltaTime * 5f);
        }
    }

    private void ApplyExtremeJitter()
    {
        // U¿ywamy interpolowanych wartoœci do trzêsienia
        float randomX = Random.Range(-currentPositionMagnitude, currentPositionMagnitude);
        float randomY = Random.Range(-currentPositionMagnitude, currentPositionMagnitude);
        Vector3 newPosition = initialPosition + new Vector3(randomX, randomY, 0f);

        float randomZRotation = Random.Range(-currentRotationMagnitude, currentRotationMagnitude);
        Quaternion newRotation = initialRotation * Quaternion.Euler(0f, 0f, randomZRotation);

        transform.localPosition = newPosition;
        transform.localRotation = newRotation;
    }

    // ===================================
    // PUBLICZNE METODY KONTROLNE I USTAWIAJ¥CE
    // ===================================

    public void StartShaking()
    {
        isShaking = true;
    }

    public void StopShaking()
    {
        isShaking = false;
        timeSinceLastChange = shakeInterval;
    }

    public void SetShakeInterval(float newInterval)
    {
        shakeInterval = newInterval;
    }

    public void SetShakeIntensity(float newPositionMag, float newRotationMag)
    {
        // GameManager ustawia DOCELOW¥ si³ê, a Update() p³ynnie do niej d¹¿y.
        positionMagnitude = newPositionMag;
        rotationMagnitude = newRotationMag;
    }
}