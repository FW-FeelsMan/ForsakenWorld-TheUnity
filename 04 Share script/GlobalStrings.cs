using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class GlobalStrings
{
    public const string ErrorConnectingToServer = "Ошибка подключения к серверу: время ожидания ответа истекло.";
    public const string ErrorMessageConnectionLost = "Соединение с сервером потеряно.";
    public const string ErrorMessageFailedReadData = "Ошибка при чтении данных от сервера.";
    public const string FailedLogin = "Неверный емейл или пароль/Учетная запись не существует";
    public const string FailedLoginMessage = "Неудачная попытка входа. Проверьте емейл и пароль";
    public const string IncorrectData = "Некорректные данные/Данные содержат недопустимые символы";
    public const string IncorrectEmail = "Некорректный емейл!";
    public const string IncorrectPassword = "Некорректный пароль! Пароль должен содержать только латинские буквы и цифры, и быть не менее 6 символов.";
    public const string PasswordMismatch = "Пароли не совпадают!";
    public const string RegistrationErrorEmailExists = "Ошибка регистрации: Email уже зарегистрирован";
    public const string ServerStarted = "Сервер запущен ";
    public const string SuccessfulLogin = "Вход успешен!";
    public const string SuccessfulRegistration = "Регистрация успешна!";
    public const string UnknownPackage = "Неизвестный пакет!";
    public const string UnknownRequestType = "Неизвестный тип запроса!";
    public const string UserAgreementFail = "Пользовательское соглашение не принято!";
    public const string UserIsAlreadyOnline = "Пользователь уже онлайн";
    public const string WelcomeMessage = "Добро пожаловать!";

    public static string GetHardwareID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    public static string Hashing(string data)
    {
        SHA256 sha256 = SHA256.Create();
        string salt = "!@#$%^&";
        byte[] bytes = Encoding.UTF8.GetBytes(data + salt);
        byte[] hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public static string HyperlinkDeal()
    {
        return "file://" + Application.streamingAssetsPath + "/Пиратский кодекс.txt";
    }
}
