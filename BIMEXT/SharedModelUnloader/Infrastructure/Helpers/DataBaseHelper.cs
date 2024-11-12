using Microsoft.Data.Sqlite;
using SharedModelUnloader.Models;


namespace SharedModelUnloader.Infrastructure.Helpers
{
    internal class DataBaseHelper
    {
        private readonly string _connectionString;

        public DataBaseHelper(string pathToDB)
        {
            _connectionString = $"Data Source={pathToDB};";
        }

        public ModelRecord GetLatestModelRecord(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
                return null;

            // SQL-запрос для выборки записи с максимальным fileVersion
            string query = @"
                SELECT fileName, fileVersion, fileDescription, userName, userEmail, publishDate
                FROM records
                WHERE fileName = @modelName
                ORDER BY fileVersion DESC
                LIMIT 1;
            ";

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@modelName", modelName);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var modelRecord = new ModelRecord
                            {
                                ModelName = reader.GetString(reader.GetOrdinal("fileName")),
                                ModelVersion = reader.GetInt32(reader.GetOrdinal("fileVersion")),
                                ModelDescription = reader.GetString(reader.GetOrdinal("fileDescription")),
                                UserName = reader.GetString(reader.GetOrdinal("userName")),
                                UserEmail = reader.GetString(reader.GetOrdinal("userEmail")),
                                PublishDate = reader.GetString(reader.GetOrdinal("publishDate"))
                            };

                            return modelRecord;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
    }
}
