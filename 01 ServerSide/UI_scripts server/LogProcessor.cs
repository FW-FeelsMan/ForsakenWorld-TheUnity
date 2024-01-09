using System;
using System.Text;
using UnityEngine;

public class LogProcessor : MonoBehaviour
{
    public static  event Action<string> OnLogProcessed;
    public static  void ProcessLog(string className, object logData)
    {
        StringBuilder stringBuilder = new();

        if (logData is string debugMessage)
        {
            stringBuilder.Append(debugMessage);
        }
        else if (logData is Exception exception)
        {
            stringBuilder.Append(exception);
            stringBuilder.Append($"�������� ������: class {className};\nException {exception};");
        }
        else
        {
            stringBuilder.Append("������������ ������ ��� �����������.");
        }

        OnLogProcessed?.Invoke(stringBuilder.ToString());
    }
}
