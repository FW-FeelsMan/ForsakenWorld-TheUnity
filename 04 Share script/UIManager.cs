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

    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void Initialization(){
        CanvasLoadingScreen.enabled = false;
        Menu_Environment.SetActive(false);
    }
    
    public void DisplayError(string errorMsg)
    {
        SetCurrentIndex(0);
        CanvasLoadingScreen.enabled = true;
        LoadingScreenResult.text = errorMsg;
        LoadingScreenButton.SetActive(true);
        if (errorMsg == "���������� � �������� ��������")
        {
            LoadingScreenButton.GetComponent<Button>().onClick.AddListener(QuitApplication);
        }
        try
        {
            graphicCanvasEnabledLogin.enabled = false;
        }
        catch
        {

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
        LoadingScreenResult.text = "�����������...";
        LoadingScreenButton.SetActive(false);
        graphicCanvasEnabledLogin.enabled = false;
    }

    public void DisplayAnswer(int _index, string _message)
    {
        LoadingScreenResult.text = "";
        SetCurrentIndex(_index);
        CanvasLoadingScreen.enabled = true;
        LoadingScreenButton.SetActive(true);
        LoadingScreenResult.text = _message;
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