using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInImageLoader : MonoBehaviour
{
    public GameObject blurLoader; // Ссылка на игровой объект BlurLoader
    public Image imageFabe; // Ссылка на компонент ImageFabe

    private Canvas canvas; // Ссылка на компонент Canvas у BlurLoader
    private bool hasFadedIn = false; // Флаг, указывающий, было ли уже выполнено плавное увеличение непрозрачности

    private void Start()
    {
        // Получаем компонент Canvas из игрового объекта BlurLoader
        canvas = blurLoader.GetComponent<Canvas>();
    }

    private void Update()
    {
        if (canvas.enabled && !hasFadedIn)
        {
            StartCoroutine(FadeInImageFabe());
            hasFadedIn = true;
        }
        else if (!canvas.enabled && hasFadedIn)
        {
            StartCoroutine(FadeOutImageFabe());
            hasFadedIn = false;
        }
    }

    private IEnumerator FadeInImageFabe()
    {
        // Сбрасываем непрозрачность до 0
        imageFabe.color = new Color(imageFabe.color.r, imageFabe.color.g, imageFabe.color.b, 0f);

        float targetAlpha = 240f / 255f;
        float currentAlpha = 0f;
        float fadeSpeed = 2.3f;

        // Плавно увеличиваем непрозрачность до 250
        while (currentAlpha < targetAlpha)
        {
            currentAlpha += fadeSpeed * Time.deltaTime;
            imageFabe.color = new Color(imageFabe.color.r, imageFabe.color.g, imageFabe.color.b, currentAlpha);
            yield return null;
        }
    }

    private IEnumerator FadeOutImageFabe()
    {
        float targetAlpha = 0f;
        float currentAlpha = 250f / 255f;
        float fadeSpeed = 2.3f;

        // Плавно сбрасываем непрозрачность до 0
        while (currentAlpha > targetAlpha)
        {
            currentAlpha -= fadeSpeed * Time.deltaTime;
            imageFabe.color = new Color(imageFabe.color.r, imageFabe.color.g, imageFabe.color.b, currentAlpha);
            yield return null;
        }
    }
}
