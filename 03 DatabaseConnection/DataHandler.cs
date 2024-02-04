using System;
using System.IO;
using MySql.Data.MySqlClient;
using ForsakenWorld;

public class DataHandler
{
    private readonly string connectionString = "Server=localhost;Database=fw_database;User=root;Password=";
    private string loggedInUserEmail;
    public bool _isUserActive;

    public bool HandleLoginData(string email, string hashedPassword, string hardwareID)
    {
        using MySqlConnection connection = CreateConnection();

        string getUserQuery = "SELECT password FROM user_data WHERE email = @email";
        MySqlCommand getUserCommand = new(getUserQuery, connection);
        getUserCommand.Parameters.AddWithValue("@email", email);
        object result = getUserCommand.ExecuteScalar();

        if (result == null || (string)result != hashedPassword)
        {
            return false;
        }

        UpdateDeviceId(email, hardwareID);

        WriteIDFile(email, hardwareID, DateTime.Now);
        loggedInUserEmail = email;

        return true;
    }

    public bool HandleRegistrationData(string email, string hashedPassword, string hardwareID)
    {
        using MySqlConnection connection = CreateConnection();

        string checkEmailQuery = "SELECT COUNT(*) FROM user_data WHERE email = @Email";
        MySqlCommand checkEmailCommand = new(checkEmailQuery, connection);
        checkEmailCommand.Parameters.AddWithValue("@Email", email);
        int count = Convert.ToInt32(checkEmailCommand.ExecuteScalar());

        if (count > 0)
        {
            return false;
        }

        string insertQuery = "INSERT INTO user_data (email, password, id_device, last_login_date) VALUES (@Email, @Password, @DeviceId, @LastLoginDate)";
        MySqlCommand insertCommand = new(insertQuery, connection);
        insertCommand.Parameters.AddWithValue("@Email", email);
        insertCommand.Parameters.AddWithValue("@Password", hashedPassword);
        insertCommand.Parameters.AddWithValue("@DeviceId", hardwareID);
        insertCommand.Parameters.AddWithValue("@LastLoginDate", DateTime.Now);

        int affectedRowsCount = insertCommand.ExecuteNonQuery();

        if (affectedRowsCount > 0)
        {

            string getIdQuery = "SELECT id FROM user_data WHERE email = @Email";
            MySqlCommand getIdCommand = new(getIdQuery, connection);
            getIdCommand.Parameters.AddWithValue("@Email", email);
            string userId = getIdCommand.ExecuteScalar().ToString();

            CreateIDFile(userId, hardwareID, DateTime.Now);
            SetClientStatus(email, 0);

            return true;
        }
        return false;
    }

    public void SetClientStatus(string email, int status)
    {
        using MySqlConnection connection = CreateConnection();
        string query = "UPDATE user_data SET status = @Status WHERE email = @Email";
        MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@Status", status);
        command.Parameters.AddWithValue("@Email", email);
        command.ExecuteNonQuery();
    }
    private void UpdateDeviceId(string email, string hardwareID)
    {
        using MySqlConnection connection = CreateConnection();
        string query = "UPDATE user_data SET id_device = @id_device WHERE email = @Email";
        MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@id_device", hardwareID);
        command.Parameters.AddWithValue("@Email", email);
        command.ExecuteNonQuery();
    }
    public void IsUserActive(string email)
    {
        using MySqlConnection connection = CreateConnection();
        string getStatusQuery = "SELECT status FROM user_data WHERE email = @Email";
        MySqlCommand getStatusCommand = new(getStatusQuery, connection);
        getStatusCommand.Parameters.AddWithValue("@Email", email);
        object result = getStatusCommand.ExecuteScalar();

        _isUserActive = (bool)result;
    }


    private MySqlConnection CreateConnection()
    {
        MySqlConnection connection = new(connectionString);
        connection.Open();
        return connection;
    }

    private void WriteIDFile(string email, string deviceId, DateTime lastLoginDate)
    {
        using MySqlConnection connection = CreateConnection();
        string getIdQuery = "SELECT id FROM user_data WHERE email = @Email";
        MySqlCommand getIdCommand = new(getIdQuery, connection);
        getIdCommand.Parameters.AddWithValue("@Email", email);
        string userId = getIdCommand.ExecuteScalar().ToString();

        string path = @"C:\xampp\htdocs\UnityBackendLog\PersistentDataPath";
        string fileName = "id_" + userId + ".txt";
        string fullPath = Path.Combine(path, fileName);

        try
        {
            using StreamWriter writer = new(fullPath, true);
            writer.WriteLine($"Device ID: {deviceId}, Last Login Date: {lastLoginDate}");
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"Ошибка при обновлении файла: {ex.Message}");
        }
    }

    private void CreateIDFile(string userId, string deviceId, DateTime lastLoginDate)
    {
        string path = @"C:\xampp\htdocs\UnityBackendLog\PersistentDataPath";
        string fileName = "id_" + userId + ".txt";
        string fullPath = Path.Combine(path, fileName);

        try
        {
            using StreamWriter writer = new(fullPath, true);
            writer.WriteLine($"Device ID: {deviceId}, Last Login Date: {lastLoginDate}");
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"Ошибка при создании файла: {ex.Message}");
        }
    }
    public string GetLoggedInUserEmail()
    {
        return loggedInUserEmail;
    }
}
