using System.Collections;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    public Material blurMaterial;
    public float duration = 1f;
    public float animationSpeed = 1f;
    private float targetSize = 1f;

    private void Start()
    {
        StartCoroutine(AnimateBlurSize());
    }

    private IEnumerator AnimateBlurSize()
    {
        float initialSize = blurMaterial.GetFloat("_Size");
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime * animationSpeed;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float currentSize = Mathf.Lerp(initialSize, targetSize, t);
            blurMaterial.SetFloat("_Size", currentSize);
            yield return null;
        }
    }
}
