using UnityEngine;
using TMPro;
using System;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText;  
    private float deltaTime = 0.0f;

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        fpsText.text = Math.Round(fps).ToString();
    }
}
