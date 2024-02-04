using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class GlobalStrings
{
    public const string ErrorConnectingToServer = "������ ����������� � �������: ����� �������� ������ �������.";
    public const string ErrorMessageConnectionLost = "���������� � �������� ��������.";
    public const string ErrorMessageFailedReadData = "������ ��� ������ ������ �� �������.";
    public const string FailedLogin = "�������� ����� ��� ������/������� ������ �� ����������";
    public const string FailedLoginMessage = "��������� ������� �����. ��������� ����� � ������";
    public const string IncorrectData = "������������ ������/������ �������� ������������ �������";
    public const string IncorrectEmail = "������������ �����!";
    public const string IncorrectPassword = "������������ ������! ������ ������ ��������� ������ ��������� ����� � �����, � ���� �� ����� 6 ��������.";
    public const string PasswordMismatch = "������ �� ���������!";
    public const string RegistrationErrorEmailExists = "������ �����������: Email ��� ���������������";
    public const string ServerStarted = "������ ������� ";
    public const string SuccessfulLogin = "���� �������!";
    public const string SuccessfulRegistration = "����������� �������!";
    public const string UnknownPackage = "����������� �����!";
    public const string UnknownRequestType = "����������� ��� �������!";
    public const string UserAgreementFail = "���������������� ���������� �� �������!";
    public const string UserIsAlreadyOnline = "������������ ��� ������";
    public const string WelcomeMessage = "����� ����������!";

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
        return "file://" + Application.streamingAssetsPath + "/��������� ������.txt";
    }
}
