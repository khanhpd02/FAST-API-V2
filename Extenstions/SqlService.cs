    using Core.Exceptions;
    using System.Data;
    using System.Data.SqlClient;
    using System.Net;

    namespace FAST_API_V2.Extenstions
    {
        public static class SqlService
        {
            public static async Task<Dictionary<string, object>[][]> ReadDataSet(string query, string connStr, params ParametersVM[] updates)
            {
                var con = new SqlConnection(connStr);
                var sqlCmd = new SqlCommand(query, con)
                {
                    CommandType = CommandType.Text
                };
                if (updates != null)
                {
                    foreach (var item in updates)
                    {
                        sqlCmd.Parameters.AddWithValue($"@{item.Field}", item.Value is null ? DBNull.Value : item.Value);
                    }
                }
                SqlDataReader reader = null;
                var tables = new List<Dictionary<string, object>[]>();
                try
                {
                    await con.OpenAsync();
                    reader = await sqlCmd.ExecuteReaderAsync();
                    while (true)
                    {
                        var table = new List<Dictionary<string, object>>();
                        while (await reader.ReadAsync())
                        {
                            table.Add(ReadSqlRecord(reader));
                        }
                        tables.Add([.. table]);
                        var next = await reader.NextResultAsync();
                        if (!next) break;
                    }
                    return [.. tables];
                }
                catch (Exception e)
                {
                    var message = $"{e.Message} {query}";
                    throw new ApiException(message, e)
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                    };
                }
                finally
                {
                    if (reader is not null) await reader.DisposeAsync();
                    await sqlCmd.DisposeAsync();
                    await con.DisposeAsync();
                }
            }

            public static async Task<K> ReadDsAs<K>(string query, string connStr, params ParametersVM[] updates) where K : class
            {
                var ds = await ReadDataSet(query, connStr, updates);
                if (ds.Length == 0 || ds[0].Length == 0) return null;
                return ds[0][0].MapTo<K>();
            }

            public static async Task<List<K>> ReadDsAsList<K>(string query, string connStr, params ParametersVM[] updates) where K : class
            {
                var ds = await ReadDataSet(query, connStr, updates);
                if (ds.Length == 0 || ds[0].Length == 0) return new List<K>();
                return ds[0].Select(x => x.MapTo<K>()).ToList();
            }

            public static async Task<bool> ExeNonQuery(string query, string connStr)
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand command = new SqlCommand(query, connection, transaction))
                            {
                                var affectedRows = await command.ExecuteNonQueryAsync();
                                transaction.Commit();
                                return affectedRows > 0;
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                }
            }
        public static async Task<int> ExeScalar(string query, string connStr)
        {
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                await connection.OpenAsync();
                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Sử dụng ExecuteScalarAsync để thực hiện truy vấn SELECT COUNT
                        var result = await command.ExecuteScalarAsync();

                        // Chuyển đổi kết quả về int, nếu null trả về 0
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
                catch (Exception ex)
                {
                    // Ghi log hoặc ném lại ngoại lệ
                    Console.WriteLine($"Error: {ex.Message}"); // Hoặc ghi log vào một file log
                    return 0; // Hoặc ném lại ngoại lệ nếu cần
                }
            }
        }



        private static Dictionary<string, object> ReadSqlRecord(IDataRecord reader)
            {
                var row = new Dictionary<string, object>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var val = reader[i];
                    row[reader.GetName(i)] = val == DBNull.Value ? null : val;
                }
                return row;
            }
        }

        public class ParametersVM
        {
            public string Field { get; set; }
            public object Value { get; set; }
        }
    }
