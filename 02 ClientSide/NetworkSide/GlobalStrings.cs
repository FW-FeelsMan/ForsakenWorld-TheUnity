using UnityEngine;

public static class GlobalStrings
{
    public const string ErrorMessageConnectionLost = "Соединение с сервером потеряно.";
    public const string ErrorMessageFailedReadData = "Ошибка при чтении данных от сервера.";
    public const string ErrorConnectingToServer = "Ошибка подключения к серверу: время ожидания ответа истекло.";
    public const string UnknownPackage = "Неизвестный пакет!";
    public const string RegistrationErrorEmailExists = "Ошибка регистрации: Email уже зарегистрирован";
    public const string SuccessfulRegistration = "Регистрация успешна!";
    public const string FailedLogin = "Неверный емейл или пароль/Учетная запись не существует";
    public const string SuccessfulLogin = "Вход успешен!";
    public const string ServerStarted = "Сервер запущен на IP localhost и порту 26950";
    public static string HyperlinkDeal()
    {
        return "file://" + Application.streamingAssetsPath + "/Пиратский кодекс.txt";
    }
}
