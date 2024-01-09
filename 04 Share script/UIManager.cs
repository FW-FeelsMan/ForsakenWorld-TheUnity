using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Canvas CanvasLoadingScreen;
    public Canvas CanvasEnabledMenu;
    public Canvas CanvasEnabledLogin;
    public TextMeshProUGUI LoadingScreenResult;
    public GameObject LoadingScreenButton;
    public GameObject Menu_Environment;
    public GraphicRaycaster graphicCanvasEnabledLogin;
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI pingText;
    public GameObject[] ImageResult;
    private int currentIndex = 0;


    public void DisplayError(string errorMsg)
    {
        SetCurrentIndex(0);
        CanvasLoadingScreen.enabled = true;
        LoadingScreenResult.text = errorMsg;
        LoadingScreenButton.SetActive(true);

        LoadingScreenButton.SetActive(true);
        if (errorMsg == "Соединение с сервером потеряно")
        {
            LoadingScreenButton.GetComponent<Button>().onClick.AddListener(QuitApplication);
        }
        graphicCanvasEnabledLogin.enabled = false;
    }
    private void SetCurrentIndex(int index)
    {
        currentIndex = index;
        for (int i = 0; i < ImageResult.Length; i++)
        {
            ImageResult[i].SetActive(false);
        }
        ImageResult[currentIndex].SetActive(true);
    }

    public void SetFPS(float fps)
    {
        fpsText.text = fps.ToString();
    }

    public void SetPing(int ping)
    {
        pingText.text = ping.ToString();
    }

    public void ShowLoginScreen()
    {
        CanvasEnabledLogin.enabled = true;
    }

    public void ShowMenu()
    {
        CanvasEnabledLogin.enabled = false;
        CanvasEnabledMenu.enabled = true;
        Menu_Environment.SetActive(true);
    }
    public void DisplayConnecting()
    {
        SetCurrentIndex(1);
        CanvasLoadingScreen.enabled = true;
        LoadingScreenResult.text = "Подключение...";
        LoadingScreenButton.SetActive(false);
        graphicCanvasEnabledLogin.enabled = false;
    }
    public void DisplayConfirmReg()
    {
        SetCurrentIndex(2);
        CanvasLoadingScreen.enabled = true;
        LoadingScreenButton.SetActive(true);
        LoadingScreenResult.text = "Регистрация успешно завершена";
        graphicCanvasEnabledLogin.enabled = false;
    }
    public void DisplayUnknowPacket()
    {
        SetCurrentIndex(0);
        CanvasLoadingScreen.enabled = true;
        LoadingScreenButton.SetActive(true);
        LoadingScreenButton.GetComponent<Button>().onClick.AddListener(QuitApplication);
        LoadingScreenResult.text = "Неизвестный пакет данных!";
        graphicCanvasEnabledLogin.enabled = false;
    }

    public void HideConnecting()
    {
        CanvasLoadingScreen.enabled = false;
        LoadingScreenButton.SetActive(true);
        graphicCanvasEnabledLogin.enabled = true;
    }


    public void QuitApplication()
    {
        Application.Quit();
    }
}