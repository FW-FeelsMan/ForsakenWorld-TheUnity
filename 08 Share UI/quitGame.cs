using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class QuitGame : MonoBehaviour
{
    public Canvas canvasBlurScreenLoader;
    public TextMeshProUGUI textResult;
    public GameObject btnConfirm;
    private string thisClassName = "QuitGame";

    public void QuitGameClick()
    {
        try
        {
            canvasBlurScreenLoader.enabled = true;
            btnConfirm.SetActive(false);
            textResult.text = "Выключение...";
            StartCoroutine(WaitingEnvironmentActive());
        }
        catch (System.Exception ex)
        {
            LogProcessor.ProcessLog(thisClassName, $"An error occurred:  + {ex.Message}");
        }
    }

    IEnumerator WaitingEnvironmentActive()
    {
        yield return new WaitForSeconds(3);
        try
        {
            Application.Quit();
        }
        catch (System.Exception ex)
        {
             LogProcessor.ProcessLog(thisClassName, $"An error occurred:  + {ex.Message}");
        }
    }
}
