using Microsoft.Data.Sqlite;
using SharedModelUnloader.Models;
using System;
using System.Globalization;


namespace SharedModelUnloader.Infrastructure.Helpers
{
    /// <summary>
    /// Класс для работы с БД SQLite
    /// </summary>
    internal class DataBaseHelper
    {
        #region Свойства
        private readonly string _connectionString;
        #endregion


        #region Конструктор
        public DataBaseHelper(string pathToDB)
        {
            _connectionString = $"Data Source={pathToDB};";
        }
        #endregion


        #region
        /// <summary>
        /// Получение последней версии модели из БД
        /// </summary>
        /// <param name="modelName">Имя модели</param>
        /// <returns>Экземпляр элемента записи в БД</returns>
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


        
        public bool WriteChangesToDB(OutputModel model, string Description)
        {
            bool flag = false;

            // Проверка на пустую строку и отсутствие модели
            if (string.IsNullOrEmpty(Description) || model is null)
                return flag;

            // SQL-запрос для вставки новой записи
            string insertQuery = @"
                INSERT INTO records (fileName, fileVersion, fileDescription, userName, userEmail, publishDate)
                VALUES (@fileName, @fileVersion, @fileDescription, @userName, @userEmail, @publishDate);";

            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = insertQuery;

                        // Добавляем параметры с соответствующими значениями
                        command.Parameters.AddWithValue("@fileName", model.Name);
                        command.Parameters.AddWithValue("@fileVersion", model.Version + 1);
                        command.Parameters.AddWithValue("@fileDescription", Description);
                        command.Parameters.AddWithValue("@userName", model.Author);
                        command.Parameters.AddWithValue("@userEmail", model.AuthorEmail);
                        command.Parameters.AddWithValue("@publishDate", GetCurrentDateTime());

                        // Выполняем команду и проверяем, сколько строк было затронуто
                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows > 0)
                            flag = true;
                    }
                }
            }
            catch (SqliteException ex)
            {
                // Добавить логгирование
            }
            catch (Exception ex)
            {
                // Добавить логгирование
            }

            return flag;
        }


        /// <summary>
        /// Генерация текущей даты
        /// </summary>
        /// <returns>Текущая дата в виде строки в формате yy.MM.dd HH:mm:ss</returns>
        private string GetCurrentDateTime()
        {
            DateTime now = DateTime.Now;
            string formattedDate = now.ToString("yy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
            return formattedDate;
        }


        #endregion
    }
}
