using UnityEngine;
using TMPro;
using System;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText;  
    public TextMeshProUGUI pingText; 
    private float deltaTime = 0.0f;

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        fpsText.text = Math.Round(fps).ToString();
        
        if(GlobalSettings.CurrentPing <= 0){
             pingText.text = "no connection";
        }else pingText.text = GlobalSettings.CurrentPing.ToString();
    }
}
