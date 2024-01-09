using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoaderController : MonoBehaviour
{
    public RectTransform loaderRectTransform; 
    public Image imageCircle;
    public Image additionalImageCircle;
    public float fillSpeed = 0.5f;
    public float pauseDuration = 1f;
    public float rotationSpeed = 10f; 

    private void Start()
    {
        StartCoroutine(AnimateLoader());
    }

    private IEnumerator AnimateLoader()
    {
        imageCircle.GetComponent<Image>().enabled = true;
        imageCircle.fillAmount = 0f;

        additionalImageCircle.GetComponent<Image>().enabled = false;
        additionalImageCircle.fillAmount = 1f;

        while (true)
        {
            while (imageCircle.fillAmount < 1f)
            {
                imageCircle.fillAmount += Time.deltaTime * fillSpeed;
                yield return null;
            }

            imageCircle.GetComponent<Image>().enabled = false;
            additionalImageCircle.GetComponent<Image>().enabled = true;

            while (additionalImageCircle.fillAmount > 0f)
            {
                additionalImageCircle.fillAmount -= Time.deltaTime * fillSpeed;
                yield return null;
            }

            yield return new WaitForSeconds(pauseDuration);
            imageCircle.fillAmount = 0f;
            additionalImageCircle.fillAmount = 1f;
            imageCircle.GetComponent<Image>().enabled = true;
            additionalImageCircle.GetComponent<Image>().enabled = false;
        }
    }

    private void Update()
    {
        // Вращение родительского объекта вокруг своей оси Z
        loaderRectTransform.Rotate(-Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
