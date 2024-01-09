using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HideLogo : MonoBehaviour
{
    public RawImage logoImage;
    public GameObject logoImageObj; 

    public void OnSignupButtonClick()
    {        
        StartCoroutine(FadeRawImageHide(logoImage, 1.0f, 0.0f, 0.5f)); 
    }

    public void  OnLoginButtonClick()
    {        
        StartCoroutine(FadeRawImageShow(logoImage, 0.0f, 1.0f, 0.5f)); 
    }
    IEnumerator FadeRawImageShow(RawImage rawImage, float startAlpha, float endAlpha, float duration)
    {
        if (rawImage.enabled)
            yield break;

        rawImage.enabled = true;

        Color color = rawImage.color;
        color.a = startAlpha;
        rawImage.color = color;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            rawImage.color = color;
            yield return null;

            elapsedTime += Time.deltaTime;
        }

        color.a = endAlpha;
        rawImage.color = color;

        if (endAlpha == 0f)
        {
            rawImage.enabled = false;
        }
    }

    IEnumerator FadeRawImageHide(RawImage rawImage, float startAlpha, float endAlpha, float duration)
    {
        if (!rawImage.enabled)
            yield break;

        rawImage.enabled = true;

        Color color = rawImage.color;
        color.a = startAlpha;
        rawImage.color = color;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            rawImage.color = color;
            yield return null;

            elapsedTime += Time.deltaTime;
        }

        color.a = endAlpha;
        rawImage.color = color;

        if (endAlpha == 0f)
        {
            rawImage.enabled = false;
        }
    }

}