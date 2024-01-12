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
    public const string ServerStarted = "������ ������� �� IP localhost � ����� 26950";
    public static string HyperlinkDeal()
    {
        return "file://" + Application.streamingAssetsPath + "/��������� ������.txt";
    }
}
