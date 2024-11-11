using Microsoft.Data.Sqlite;
using SharedModelUnloader.Models;
using System;
using System.Threading.Tasks;


namespace SharedModelUnloader.Infrastructure.Helpers
{
    internal class DataBaseHelper
    {
        private readonly string _connectionString;

        public DataBaseHelper(string pathToDB)
        {
            _connectionString = pathToDB;
        }

        public async Task<ModelRecord> GetLatestModelRecordAsync(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
                throw new ArgumentException("Имя модели не может быть пустым.", nameof(modelName));

            // SQL-запрос для выборки записи с максимальным fileVersion
            string query = @"
                SELECT fileName, fileVersion, fileDescription, userName, userEmail, publishDate
                FROM Files
                WHERE fileName = @modelName
                ORDER BY fileVersion DESC
                LIMIT 1;
            ";

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@fileName", modelName);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var modelRecord = new ModelRecord
                            {
                                ModelName = reader.GetString(reader.GetOrdinal("fileName")),
                                ModelVersion = reader.GetInt32(reader.GetOrdinal("fileVersion")),
                                ModelDescription = reader.GetString(reader.GetOrdinal("fileDescription")),
                                UserName = reader.GetString(reader.GetOrdinal("userName")),
                                UserEmail = reader.GetString(reader.GetOrdinal("userName")),
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
