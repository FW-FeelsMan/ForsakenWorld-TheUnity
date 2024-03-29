using UnityEngine;
using MySql.Data.MySqlClient;

public class DatabaseManager : MonoBehaviour
{
    private readonly string connectionString = "Server=localhost;Database=fw_database;User=root;Password=;";
    private MySqlConnection connection;

    private void Start()
    {
        connection = new MySqlConnection(connectionString);
        connection.Open();
    }

    public MySqlConnection GetConnection()
    {
        return connection;
    }

    private void OnDestroy()
    {
        connection?.Close();
    }
}
