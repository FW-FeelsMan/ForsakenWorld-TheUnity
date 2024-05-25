using System;
using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ForsakenWorld;
using System.Collections.Generic;

public class DataHandler
{
    private readonly string connectionString = "Server=localhost;Database=fw_database;User=root;Password=";
    private string loggedInUserEmail;
    public bool _isUserActive;

    public async Task<bool> HandleLoginDataAsync(string email, string hashedPassword, string hardwareID, bool forceLoginRequested = false)
    {
        try
        {
            using MySqlConnection connection = await CreateConnectionAsync();
            string getUserQuery = "SELECT password FROM user_data WHERE email = @Email";
            MySqlCommand getUserCommand = new(getUserQuery, connection);
            getUserCommand.Parameters.AddWithValue("@Email", email);
            object result = await getUserCommand.ExecuteScalarAsync();

            if (result == null || (string)result != hashedPassword)
            {
                return false;
            }

            // ��������� ��������������� �����
            if (forceLoginRequested)
            {
                await SetClientStatusAsync(email, 0); // ������������� ������ ������� �� �������� (0)
            }

            await UpdateDeviceIdAsync(email, hardwareID);
            await WriteIDFileAsync(email, hardwareID, DateTime.Now);
            loggedInUserEmail = email;

            return true;
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"������ ��� ��������� ������ �����: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> HandleRegistrationDataAsync(string email, string hashedPassword, string hardwareID)
    {
        try
        {
            using MySqlConnection connection = await CreateConnectionAsync();
            string checkEmailQuery = "SELECT COUNT(*) FROM user_data WHERE email = @Email";
            MySqlCommand checkEmailCommand = new(checkEmailQuery, connection);
            checkEmailCommand.Parameters.AddWithValue("@Email", email);
            int count = Convert.ToInt32(await checkEmailCommand.ExecuteScalarAsync());

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

            int affectedRowsCount = await insertCommand.ExecuteNonQueryAsync();

            if (affectedRowsCount > 0)
            {
                string getIdQuery = "SELECT id FROM user_data WHERE email = @Email";
                MySqlCommand getIdCommand = new(getIdQuery, connection);
                getIdCommand.Parameters.AddWithValue("@Email", email);
                string userId = (await getIdCommand.ExecuteScalarAsync()).ToString();

                await CreateIDFileAsync(userId, hardwareID, DateTime.Now);
                await SetClientStatusAsync(email, 0);

                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"������ ��� ����������� ������: {ex.Message}");
            return false;
        }
    }

    public async Task SetClientStatusAsync(string email, int status)
    {
        try
        {
            using MySqlConnection connection = await CreateConnectionAsync();
            string query = "UPDATE user_data SET status = @Status WHERE email = @Email";
            MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@Email", email);
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"������ ��� ���������� ������� �������: {ex.Message}");
        }
    }

    public async Task SetSocketClientAsync(string email, int socketClient)
    {
        try
        {
            using MySqlConnection connection = await CreateConnectionAsync();
            string query = "UPDATE user_data SET socketClient = @socketClient WHERE email = @Email";
            MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@socketClient", socketClient);
            command.Parameters.AddWithValue("@Email", email);
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"������ ��� ���������� ������ �������: {ex.Message}");
        }
    }

    private async Task UpdateDeviceIdAsync(string email, string hardwareID)
    {
        try
        {
            using MySqlConnection connection = await CreateConnectionAsync();
            string query = "UPDATE user_data SET id_device = @id_device WHERE email = @Email";
            MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@id_device", hardwareID);
            command.Parameters.AddWithValue("@Email", email);
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"������ ��� ���������� ID ����������: {ex.Message}");
        }
    }

    public async Task<string> IsUserActiveAsync(string email)
    {
        try
        {
            using MySqlConnection connection = await CreateConnectionAsync();
            string getStatusQuery = "SELECT status FROM user_data WHERE email = @Email";
            MySqlCommand getStatusCommand = new(getStatusQuery, connection);
            getStatusCommand.Parameters.AddWithValue("@Email", email);

            object result = await getStatusCommand.ExecuteScalarAsync();
            return result != null ? (Convert.ToInt32(result) == 1 ? "1" : "0") : "-1";
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"������ ��� �������� ���������� ������������: {ex.Message}");
            return "-1";
        }
    }

    private async Task<MySqlConnection> CreateConnectionAsync()
    {
        MySqlConnection connection = new(connectionString);
        await connection.OpenAsync();
        return connection;
    }

    private async Task WriteIDFileAsync(string email, string deviceId, DateTime lastLoginDate)
    {
        try
        {
            using MySqlConnection connection = await CreateConnectionAsync();
            string getIdQuery = "SELECT id FROM user_data WHERE email = @Email";
            MySqlCommand getIdCommand = new(getIdQuery, connection);
            getIdCommand.Parameters.AddWithValue("@Email", email);
            string userId = (await getIdCommand.ExecuteScalarAsync()).ToString();

            string path = @"C:\xampp\htdocs\UnityBackendLog\PersistentDataPath";
            string fileName = "id_" + userId + ".txt";
            string fullPath = Path.Combine(path, fileName);

            using StreamWriter writer = new(fullPath, true);
            await writer.WriteLineAsync($"Device ID: {deviceId}, Last Login Date: {lastLoginDate}");
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"������ ��� ���������� �����: {ex.Message}");
        }
    }

    private async Task CreateIDFileAsync(string userId, string deviceId, DateTime lastLoginDate)
    {
        try
        {
            string path = @"C:\xampp\htdocs\UnityBackendLog\PersistentDataPath";
            string fileName = "id_" + userId + ".txt";
            string fullPath = Path.Combine(path, fileName);

            using StreamWriter writer = new(fullPath, true);
            await writer.WriteLineAsync($"Device ID: {deviceId}, Last Login Date: {lastLoginDate}");
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"������ ��� �������� �����: {ex.Message}");
        }
    }

    public string GetLoggedInUserEmail()
    {
        return loggedInUserEmail;
    }

    public async Task<int> GetClientSocketNumAsync(string email)
    {
        try
        {
            using MySqlConnection connection = await CreateConnectionAsync();
            string query = "SELECT socketClient FROM user_data WHERE email = @Email";
            MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            object result = await command.ExecuteScalarAsync();
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : -1;
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"������ ��� ��������� ������ ������: {ex.Message}");
            return -1;
        }
    }
    public async Task<List<Character>> GetUserCharactersAsync(int userId)
    {
        List<Character> characters = new();

        try
        {
            using MySqlConnection connection = await CreateConnectionAsync();
            string query = "SELECT character_id, class_id, gender_id, race_id, level, appearance, currency FROM characters WHERE user_id = @UserId";
            MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                characters.Add(new Character
                {
                    CharacterId = reader.GetInt32("character_id"),
                    ClassId = reader.GetInt32("class_id"),
                    GenderId = reader.GetInt32("gender_id"),
                    RaceId = reader.GetInt32("race_id"),
                    Level = reader.GetInt32("level"),
                    Appearance = reader.GetString("appearance"),
                    Currency = reader.GetInt32("currency")
                });
            }
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"Ошибка при загрузке персонажей: {ex.Message}");
        }

        return characters;
    }
    public async Task<int> GetUserIdByEmailAsync(string email)
    {
        int userId = -1;

        try
        {
            using MySqlConnection connection = await CreateConnectionAsync();
            string query = "SELECT id FROM user_data WHERE email = @Email";
            MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            object result = await command.ExecuteScalarAsync();
            if (result != null && result != DBNull.Value)
            {
                userId = Convert.ToInt32(result);
            }
        }
        catch (Exception ex)
        {
            LogProcessor.ProcessLog(FWL.GetClassName(), $"Ошибка при получении ID пользователя: {ex.Message}");
        }

        return userId;
    }

}
public class Character
{
    public int CharacterId { get; set; }
    public int ClassId { get; set; }
    public int GenderId { get; set; }
    public int RaceId { get; set; }
    public int Level { get; set; }
    public string Appearance { get; set; }
    public int Currency { get; set; }
}
