using UnityEngine;
using MySql.Data.MySqlClient;
using TMPro;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    [Header("Canvas Group")]
    public CanvasGroup canvasGroup;
    [System.Serializable]
    public class CanvasGroup
    {
        public Canvas CanvasEnabledLogin;
        public Canvas CanvasEnabledMenu;
        public Canvas CanvasCreateChar;
        public Canvas CanvasAdminPanel;
        public Canvas CanvasLoadingScreen;
    }

    [Header("InputFields Group")]
    public InputFields InputFieldsGroup;
    [System.Serializable]
    public class InputFields
    {
        public TMP_InputField emailLoginField;
        public TMP_InputField passwordLoginField;
        public TMP_InputField emailRegisterField;
        public TMP_InputField passwordRegisterField;
    }

    [Header("CanvasLoadingScreen Group")]
    public LoadingScreen gLoadingScreen;
    [System.Serializable]
    public class LoadingScreen 
    {
        public TextMeshProUGUI LoadingScreenResult;
        public GameObject LoadingScreenButton;
    }

    public bool isServer = false;
    public GameObject enviroment;
    public DatabaseManager databaseManager;

    private string thisClassName;

    void Start(){
        thisClassName = GetType().Name;
    }

    public void Login()
    {
        string email = InputFieldsGroup.emailLoginField.text;
        string password = InputFieldsGroup.passwordLoginField.text;
        MySqlConnection connection = databaseManager.GetConnection();
        
        string id_device = HardwareID.GetHardwareID();

        StartCoroutine(LoginCoroutine(email, password, id_device, connection));
    }

    private IEnumerator LoginCoroutine(string email, string password, string id_device, MySqlConnection connection)
    {
        canvasGroup.CanvasLoadingScreen.enabled = true;
        gLoadingScreen.LoadingScreenButton.SetActive(false);
        gLoadingScreen.LoadingScreenResult.text = "Подключение...";
        yield return new WaitForSeconds(3);

        if (isServer)
        {
            try
            {
                MySqlCommand command = new MySqlCommand("SELECT * FROM user_data WHERE email = @email AND password = @password", connection);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);

                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    canvasGroup.CanvasLoadingScreen.enabled = false;
                    enviroment.SetActive(true);
                    canvasGroup.CanvasEnabledLogin.enabled = false;
                    canvasGroup.CanvasEnabledMenu.enabled = true;
                    canvasGroup.CanvasAdminPanel.enabled = true;

                    reader.Close();
                    MySqlCommand updateLastLoginDateCommand = new MySqlCommand("UPDATE user_data SET last_login_date = NOW(), id_device = @id_device WHERE email = @email", connection);
                    updateLastLoginDateCommand.Parameters.AddWithValue("@email", email);
                    updateLastLoginDateCommand.Parameters.AddWithValue("@id_device", id_device);
                    updateLastLoginDateCommand.ExecuteNonQuery();

                }
                else
                {
                    gLoadingScreen.LoadingScreenResult.text = "Неверный логин или пароль.";
                    gLoadingScreen.LoadingScreenButton.SetActive(true);
                }

                reader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
            {                
                LogProcessor.ProcessLog(thisClassName, $"Error connecting to MySQL: " + ex.Message);
                gLoadingScreen.LoadingScreenResult.text = "Нет соединения с БД";
            }
        }
    }
    public void Register()
    {
        string email = InputFieldsGroup.emailRegisterField.text;
        string password = InputFieldsGroup.passwordRegisterField.text;
        MySqlConnection connection = databaseManager.GetConnection();

        StartCoroutine(RegistrationCoroutine(email, password, connection));
    }

    private IEnumerator RegistrationCoroutine(string email, string password, MySqlConnection connection)
    {
        canvasGroup.CanvasLoadingScreen.enabled = true;
        gLoadingScreen.LoadingScreenButton.SetActive(false);
        gLoadingScreen.LoadingScreenResult.text = "Регистрация...";
        yield return new WaitForSeconds(3);

        if (isServer)
        {
            try
            {
                MySqlCommand checkEmailCommand = new MySqlCommand("SELECT * FROM user_data WHERE email = @email", connection);
                checkEmailCommand.Parameters.AddWithValue("@email", email);

                MySqlDataReader reader = checkEmailCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    gLoadingScreen.LoadingScreenResult.text = "Email уже занят";
                    gLoadingScreen.LoadingScreenButton.SetActive(true);
                }
                else
                {
                    reader.Close();
                    MySqlCommand createUserCommand = new MySqlCommand("INSERT INTO user_data (email, password) VALUES (@newEmail, @newPassword)", connection);
                    createUserCommand.Parameters.AddWithValue("@newEmail", email);
                    createUserCommand.Parameters.AddWithValue("@newPassword", password);

                    int rowsAffected = createUserCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        gLoadingScreen.LoadingScreenResult.text = "Пользователь успешно создан";
                        gLoadingScreen.LoadingScreenButton.SetActive(true);
                    }
                    else
                    {
                        gLoadingScreen.LoadingScreenResult.text = "Ошибка создания пользователя";
                        gLoadingScreen.LoadingScreenButton.SetActive(true);
                    }
                }

                reader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                LogProcessor.ProcessLog(thisClassName, $"Error connecting to MySQL: " + ex.Message);
                gLoadingScreen.LoadingScreenResult.text = "Нет соединения с БД";
            }
        }
    }
}

