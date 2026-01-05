namespace MRP.Endpoints.Media{
    public static class SearchMedia
    {
        private static readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Username=mrp;Password=mrp1234;Database=mrp_database");

        public static async Task HandleSearchMedia(HttpListenerRequest req, HttpListenerResponse resp)
        {
            bool isAuthorized = await TokenValidation.IsTokenValid(req.Headers["Authorization"] ?? "");
            if (!isAuthorized)
            {
                await ClientError.Handle401(req, resp, "Unauthorized access. Please provide a valid token.");
                return;
            }

            string[] textFilters = { "title", "media_type", "genre", "created_by" };
            string[] intFilters = ["release_year", "age_restriction"];

            int argumentsIndex = 0;

            string sql = "SELECT * FROM media";

            foreach (var filter in textFilters)
            {
                if (req.QueryString[filter] != null)
                {
                    if (argumentsIndex > 0)
                    {
                        sql += $" AND {filter} ILIKE @{filter}";
                    }
                    else
                    {
                        sql += $" WHERE {filter} ILIKE @{filter}";
                    }
                    argumentsIndex++;
                }
            }

            foreach (var filter in intFilters)
            {
                if (req.QueryString[filter] != null)
                {
                    if (argumentsIndex > 0)
                    {
                        sql += $" AND {filter} = @{filter}";
                    }
                    else
                    {
                        sql += $" WHERE {filter} = @{filter}";
                    }
                    argumentsIndex++;
                }
            }

            if (req.QueryString["sortBy"] != null)
            {
                string sortBy = req.QueryString["sortBy"]!;
                string sortOrder = req.QueryString["sortOrder"] != null && req.QueryString["sortOrder"]!.ToUpper() == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {sortBy} {sortOrder}";
            }

            try {
                await conn.OpenAsync();
                await using var cmd = new NpgsqlCommand(sql, conn);
                
                foreach (var filter in textFilters)
                {
                    if (req.QueryString[filter] != null)
                    {
                        cmd.Parameters.AddWithValue($"@{filter}", $"%{req.QueryString[filter]}%");
                    }
                }

                foreach (var filter in intFilters)
                {
                    if (req.QueryString[filter] != null)
                    {
                        if (int.TryParse(req.QueryString[filter], out int intValue))
                        {
                            cmd.Parameters.AddWithValue($"@{filter}", intValue);
                        }
                    }
                }

                await using var reader = await cmd.ExecuteReaderAsync();
                
                List<MRP.Models.Media> mediaList = new List<MRP.Models.Media>();
                
                while (await reader.ReadAsync())
                {
                    var media = new MRP.Models.Media
                    {
                        id = reader.GetInt32(0),
                        title = reader.GetString(1),
                        description = reader.GetString(2),
                        media_type = reader.GetString(3),
                        release_year = reader.GetInt32(4),
                        genre = JsonSerializer.Deserialize<List<string>>(reader.GetString(5)) ?? new List<string>(),
                        age_restriction = reader.GetInt32(6),
                        created_by = reader.GetString(7)
                    };
                    mediaList.Add(media);
                }

                await Success.Handle200(req, resp, JsonSerializer.Serialize(mediaList));
            }catch (Exception ex)
            {
                await ServerError.Handle500(req, resp, "An error occurred while processing the request.");
                Console.WriteLine(ex.Message);
            }finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
            
        }
    
    }
}