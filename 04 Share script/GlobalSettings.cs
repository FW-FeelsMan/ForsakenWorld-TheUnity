using UnityEngine;

public class GlobalSettings 
{
    public static int ServerResponseTimeout = 5;
    public static void SetServerResponseTimeout(int value)
    {
        if (value >= 1 && value <= 5)
        {
            ServerResponseTimeout = value;
        }
        else
        {
            Debug.LogWarning("������� ��������� ������������� �������� ��� ������� �������� ������ �� �������.");
        }
    }
}
