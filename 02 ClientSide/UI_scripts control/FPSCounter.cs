using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float fpsDeltaTime;
    private float fps;
    private UIManager uiManager;
    void Start()
    {
        uiManager = GetComponent<UIManager>();
    }
    private void Update()
    {
        fpsDeltaTime += (Time.unscaledDeltaTime - fpsDeltaTime) * 0.1f;
        fps = Mathf.RoundToInt(1.0f / fpsDeltaTime);
        uiManager.SetFPS(fps);
    }
}
