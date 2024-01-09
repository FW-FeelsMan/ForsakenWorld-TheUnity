using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInImageLoader : MonoBehaviour
{
    public GameObject blurLoader; // ������ �� ������� ������ BlurLoader
    public Image imageFabe; // ������ �� ��������� ImageFabe

    private Canvas canvas; // ������ �� ��������� Canvas � BlurLoader
    private bool hasFadedIn = false; // ����, �����������, ���� �� ��� ��������� ������� ���������� ��������������

    private void Start()
    {
        // �������� ��������� Canvas �� �������� ������� BlurLoader
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
        // ���������� �������������� �� 0
        imageFabe.color = new Color(imageFabe.color.r, imageFabe.color.g, imageFabe.color.b, 0f);

        float targetAlpha = 240f / 255f;
        float currentAlpha = 0f;
        float fadeSpeed = 2.3f;

        // ������ ����������� �������������� �� 250
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

        // ������ ���������� �������������� �� 0
        while (currentAlpha > targetAlpha)
        {
            currentAlpha -= fadeSpeed * Time.deltaTime;
            imageFabe.color = new Color(imageFabe.color.r, imageFabe.color.g, imageFabe.color.b, currentAlpha);
            yield return null;
        }
    }
}
