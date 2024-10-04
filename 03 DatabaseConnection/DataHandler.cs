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

            if (forceLoginRequested)
            {
                await SetClientStatusAsync(email, 0);
                
            }

            await UpdateDeviceIdAsync(email, hardwareID);
            await WriteIDFileAsync(email, hardwareID, DateTime.Now);
            loggedInUserEmail = email;

            
            return true;
        }
        catch (Exception ex)
        {
            ThreadSafeLogger.Log(ex.ToString());
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
            ThreadSafeLogger.Log(ex.ToString());
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
            ThreadSafeLogger.Log(ex.ToString());
        }
    }

    public async Task SetSocketClientAsync(string email, int socketClient)
    {
        try
        {
            using MySqlConnection connection = await CreateConnectionAsync();
            string query = "UPDATE user_data SET socketClient = @SocketClient WHERE email = @Email";
            MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@SocketClient", socketClient);
            command.Parameters.AddWithValue("@Email", email);
            await command.ExecuteNonQueryAsync();
            
        }
        catch (Exception ex)
        {
            ThreadSafeLogger.Log(ex.ToString());
        }
    }

    private async Task UpdateDeviceIdAsync(string email, string hardwareID)
    {
        try
        {
            using MySqlConnection connection = await CreateConnectionAsync();
            string query = "UPDATE user_data SET id_device = @IdDevice WHERE email = @Email";
            MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@IdDevice", hardwareID);
            command.Parameters.AddWithValue("@Email", email);
            await command.ExecuteNonQueryAsync();
            
        }
        catch (Exception ex)
        {
            ThreadSafeLogger.Log(ex.ToString());
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
            string status = result != null ? (Convert.ToInt32(result) == 1 ? "1" : "0") : "-1";
            
            return status;
        }
        catch (Exception ex)
        {
            ThreadSafeLogger.Log(ex.ToString());
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
            ThreadSafeLogger.Log(ex.ToString());
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
           ThreadSafeLogger.Log(ex.ToString()); 
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
            int socketClient = result != null && result != DBNull.Value ? Convert.ToInt32(result) : -1;
            
            return socketClient;
        }
        catch (Exception ex)
        {
            ThreadSafeLogger.Log(ex.ToString());
            return -1;
        }
    }

    public async Task<List<Character>> GetUserCharactersAsync(string email)
    {
        List<Character> characters = new();

        try
        {
            using MySqlConnection connection = await CreateConnectionAsync();
            string query = @"SELECT c.character_id, c.user_id, c.class_id, c.gender_id, c.race_id, c.level, c.appearance, c.currency, c.gold_currency, c.character_status
                         FROM characters c
                         JOIN user_data u ON c.user_id = u.id
                         WHERE u.email = @Email";
            MySqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                characters.Add(new Character
                {
                    CharacterId = reader.GetInt32("character_id"),
                    UserId = reader.GetInt32("user_id"),
                    ClassId = reader.GetInt32("class_id"),
                    GenderId = reader.GetInt32("gender_id"),
                    RaceId = reader.GetInt32("race_id"),
                    Level = reader.GetInt32("level"),
                    Appearance = reader.GetString("appearance"),
                    Currency = reader.GetInt32("currency"),
                    GoldCurrency = reader.GetInt32("gold_currency"),
                    CharacterStatus = reader.GetInt32("character_status"),
                    ClassName = Character.GetClassName(reader.GetInt32("class_id")),
                    GenderName = Character.GetGenderName(reader.GetInt32("gender_id")),
                    RaceName = Character.GetRaceName(reader.GetInt32("race_id"))
                });
            }
        }
        catch (Exception ex)
        {
            ThreadSafeLogger.Log(ex.ToString());
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
            ThreadSafeLogger.Log(ex.ToString());
        }

        return userId;
    }
}
