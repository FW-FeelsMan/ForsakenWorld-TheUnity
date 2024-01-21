using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class GlobalStrings
{
    public const string ErrorMessageConnectionLost = "���������� � �������� ��������.";
    public const string ErrorMessageFailedReadData = "������ ��� ������ ������ �� �������.";
    public const string ErrorConnectingToServer = "������ ����������� � �������: ����� �������� ������ �������.";
    public const string UnknownPackage = "����������� �����!";
    public const string RegistrationErrorEmailExists = "������ �����������: Email ��� ���������������";
    public const string SuccessfulRegistration = "����������� �������!";
    public const string FailedLogin = "�������� ����� ��� ������/������� ������ �� ����������";
    public const string SuccessfulLogin = "���� �������!";
    public const string ServerStarted = "������ ������� ";
    public const string IncorrectData = "������������ ������/������ �������� ������������ �������";
    public const string UserIsAlreadyOnline = "������������ ��� ������";
    public const string HelloUserTest = "����� ����������!";
    public static string HyperlinkDeal()
    {
        return "file://" + Application.streamingAssetsPath + "/��������� ������.txt";
    }
    public static string GetHardwareID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
    static string salt = "!@#$%^&";
    public static string Hashing(string data)
    {
        SHA256 sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(data + salt);
        byte[] hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
