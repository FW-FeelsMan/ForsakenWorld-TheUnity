using Michsky.UI.Frost;
using TMPro;
using UnityEngine;

public class RememberManager : MonoBehaviour
{
    public TMP_InputField loginField;
    public TMP_InputField passwordField;
    public SwitchManager switchManager;

    private string loadedUsername;
    private string loadedPassword;

    void Start()
    {
        LoadCredentials();
    }

    void Update()
    {
        if (switchManager.isOn)
        {
            if (loginField.text != loadedUsername || passwordField.text != loadedPassword)
            {
                SaveCredentials();
            }
        }
    }

    public void SaveCredentials()
    {
        var hashedPassword = HashPassword(passwordField.text);
        PlayerPrefs.SetString("Username", loginField.text);
        PlayerPrefs.SetString("Password", hashedPassword);
        loadedUsername = loginField.text;
        loadedPassword = hashedPassword;
    }

    public void ForgetMe()
    {
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("Password");
        loadedUsername = "";
        loadedPassword = "";
    }

    public void LoadCredentials()
    {
        loadedUsername = PlayerPrefs.GetString("Username", "");
        loadedPassword = PlayerPrefs.GetString("Password", "");
        loginField.text = loadedUsername;
        passwordField.text = loadedPassword;
    }

    private string HashPassword(string password)
    {
        return password;
    }
}
