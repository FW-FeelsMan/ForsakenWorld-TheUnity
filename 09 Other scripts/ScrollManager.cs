using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScrollManager : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public float scrollSpeed = 25.0f; 

    public void ScrollToNewMessage()
    {
        float currentScrollPos = content.anchoredPosition.y / (content.sizeDelta.y - scrollRect.viewport.sizeDelta.y);
        float targetScrollPos = 0.0f; 

        StartCoroutine(ScrollCoroutine(currentScrollPos, targetScrollPos));
    }

    private IEnumerator ScrollCoroutine(float from, float to)
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * scrollSpeed;
            float value = Mathf.Lerp(from, to, t);
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, value * (content.sizeDelta.y - scrollRect.viewport.sizeDelta.y));
            yield return null;
        }
    }
}
