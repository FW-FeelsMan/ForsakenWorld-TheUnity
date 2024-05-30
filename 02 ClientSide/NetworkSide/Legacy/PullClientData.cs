using System.Threading.Tasks;
using UnityEngine;

public class PullClientData : Singleton<PullClientData>
{
    private void Awake()
    {
        instance = this;
    }
    
    public string playerEmail;
    public string lastEquipmentID;
    public string currentEquipmentID;

    public void RequestClientData()
    {

        Task<byte[]> emailTask = RequestToServer.RequestTypeAsync(CommandKeys.GetPlayerEmail, null);
        Task<byte[]> equipmentIDTask = RequestToServer.RequestTypeAsync(CommandKeys.GetDevId, null);

        Task.WhenAll(emailTask, equipmentIDTask).ContinueWith(t =>
        {
            // ���������, �� ��������� �� ������ ��� ��������� ������
            if (t.Exception != null)
            {
                Debug.LogError("������ ��� ��������� ������ � �������.");
                // ��������� ����������
                var aggregateException = t.Exception.Flatten();
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    Debug.LogError(innerException);
                }
            }else{
                
                byte[] emailData = emailTask.Result;
                byte[] equipmentIDData = equipmentIDTask.Result;

                GlobalDataClasses.UserDataObject userData = DataDeserializer.Deserialize<GlobalDataClasses.UserDataObject>(emailData);
                Debug.Log(userData);

                currentEquipmentID = GlobalStrings.GetHardwareID();
                if(currentEquipmentID != lastEquipmentID){
                    Debug.Log("ID �� ���������");
                }
            }
        });
    }
}
