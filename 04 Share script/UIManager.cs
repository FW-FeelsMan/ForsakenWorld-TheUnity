using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Canvas CanvasLoadingScreen;
    public TextMeshProUGUI LoadingScreenResult;
    public GameObject LoadingScreenButton;
    public GameObject[] ImageResult;
    private int currentIndex = 0;
    public Canvas[] SplashScreen;

    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }
    public void Initialization()
    {
        CanvasLoadingScreen.enabled = false;

        foreach (var screen in SplashScreen)
        {
            screen.enabled = true;
        }
    }
    public void DisplayError(string errorMsg)
    {
        SetCurrentIndex(0);
        CanvasLoadingScreen.enabled = true;
        LoadingScreenResult.text = errorMsg;
        LoadingScreenButton.SetActive(true);
        if (errorMsg == "Соединение с сервером потеряно")
            Debug.Log(errorMsg);
        {
            foreach (var screen in SplashScreen)
            {
                screen.enabled = false;
            }
            LoadingScreenButton.GetComponent<Button>().onClick.AddListener(Application.Quit);
        }
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
    public void DisplayConnecting()
    {
        SetCurrentIndex(1);
        CanvasLoadingScreen.enabled = true;
        LoadingScreenResult.text = "Подключение...";
        LoadingScreenButton.SetActive(false);
        foreach (var screen in SplashScreen)
        {
            screen.enabled = false;
        }
    }

    public void DisplayAnswer(int _index, string _message)
    {
        LoadingScreenResult.text = "";
        SetCurrentIndex(_index);
        CanvasLoadingScreen.enabled = true;
        LoadingScreenButton.SetActive(true);
        LoadingScreenResult.text = _message;
        foreach (var screen in SplashScreen)
        {
            screen.enabled = false;
        }
    }

    public void HideConnecting()
    {
        CanvasLoadingScreen.enabled = false;
        LoadingScreenButton.SetActive(true);
        foreach (var screen in SplashScreen)
        {
            screen.enabled = true;
        }
    }
}