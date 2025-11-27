using UnityEngine;
using UnityEngine.UI;

public class UIBreathing : MonoBehaviour
{
    // Obiekt, który kontroluje pozycjê i rozmiar elementów UI
    private RectTransform rectTransform;

    // Zmienna do przechowywania oryginalnej pozycji Y obiektu
    private float originalY;

    [Header("Ustawienia Falowania")]

    // Szybkoœæ, z jak¹ obiekt faluje (czêstotliwoœæ)
    public float Speed = 1f;

    // Maksymalna odleg³oœæ, o jak¹ obiekt siê podniesie/opuœci (amplituda w pikselach)
    public float Amplitude = 5f;

    void Start()
    {
        // Pobierz komponent RectTransform, który jest niezbêdny dla UI
        rectTransform = GetComponent<RectTransform>();

        // Zapisz oryginaln¹ pozycjê Y, aby ruch by³ zawsze wokó³ tego punktu
        originalY = rectTransform.anchoredPosition.y;
    }

    void Update()
    {
        // Czas od pocz¹tku gry, pomno¿ony przez Speed, daje nam p³ynnie rosn¹c¹ wartoœæ.
        float time = Time.time * Speed;

        // Funkcja Mathf.Sin zmienia siê od -1 do 1, co tworzy efekt wdechu i wydechu (w górê i w dó³).
        float offset = Mathf.Sin(time) * Amplitude;

        // Oblicz now¹ pozycjê Y
        float newY = originalY + offset;

        // Ustaw now¹ pozycjê obiektu (X pozostaje bez zmian)
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, newY);
    }
}