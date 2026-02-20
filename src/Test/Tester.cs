using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;
using Testcontainers.MsSql;
using Testcontainers.PostgreSql;

namespace Test {
   public static class Tester {

      private static MsSqlContainer? _sqlContainer;
      private static PostgreSqlContainer? _pgContainer;

      // SQL Server connection properties (populated when container starts)
      public static string SqlServer { get; private set; } = "localhost";
      public static int SqlPort { get; private set; } = 1433;
      public static string SqlUser { get; private set; } = "sa";
      public static string Pw { get; private set; } = "DevDev1!";

      // PostgreSQL connection properties (populated when container starts)
      public static string PgServer { get; private set; } = "localhost";
      public static int PgPort { get; private set; } = 5432;
      public static string PgUser { get; private set; } = "postgres";

      public static string GetSqlConnectionString(string database) {
         return $"server={SqlServer},{SqlPort};database={database};User Id={SqlUser};Password={Pw};Trust Server Certificate=True;Encrypt=true";
      }

      public static string GetPgConnectionString(string database) {
         return $"Host={PgServer};Port={PgPort};Database={database};Username={PgUser};Password={Pw};";
      }

      public static async Task InitializeContainer() {
         // Npgsql 6+ requires UTC DateTimes for timestamptz columns by default.
         // SQL Server returns DateTime values with Kind=Local/Unspecified, so enable
         // legacy behavior to allow writing non-UTC DateTimes to PostgreSQL.
         AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

         Console.WriteLine("Starting containers...");

         _sqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
            .WithPassword(Pw)
            .WithCleanUp(true)
            .Build();

         _pgContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithUsername(PgUser)
            .WithPassword(Pw)
            .WithDatabase("postgres")
            .WithCleanUp(true)
            .Build();

         // Start both containers in parallel
         await Task.WhenAll(_sqlContainer.StartAsync(), _pgContainer.StartAsync());

         SqlServer = _sqlContainer.Hostname;
         SqlPort = _sqlContainer.GetMappedPublicPort(MsSqlBuilder.MsSqlPort);

         PgServer = _pgContainer.Hostname;
         PgPort = _pgContainer.GetMappedPublicPort(PostgreSqlBuilder.PostgreSqlPort);

         Console.WriteLine($"SQL Server container started: {SqlServer}:{SqlPort}");
         Console.WriteLine($"PostgreSQL container started: {PgServer}:{PgPort}");

         // Initialize both databases in parallel
         await Task.WhenAll(InitializeSqlServer(), InitializePostgres());

         Console.WriteLine("All database initialization complete");
      }

      public static async Task DisposeContainer() {
         var tasks = new List<Task>();
         if (_sqlContainer != null) tasks.Add(_sqlContainer.DisposeAsync().AsTask());
         if (_pgContainer != null) tasks.Add(_pgContainer.DisposeAsync().AsTask());
         if (tasks.Count > 0) {
            Console.WriteLine("Stopping containers...");
            await Task.WhenAll(tasks);
            Console.WriteLine("Containers stopped");
         }
      }

      private static async Task InitializeSqlServer() {
         Console.WriteLine("Initializing SQL Server...");

         using (var cn = new SqlConnection(GetSqlConnectionString("master"))) {
            await cn.OpenAsync();
            await cn.ExecuteAsync("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TflNorthWind') CREATE DATABASE TflNorthWind");
            await cn.ExecuteAsync("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Junk') CREATE DATABASE Junk");
         }

         var instnwndPath = Path.Combine(AppContext.BaseDirectory, "files", "instnwnd.sql");
         if (!File.Exists(instnwndPath)) {
            throw new FileNotFoundException($"Northwind installation script not found at {instnwndPath}");
         }

         var instnwndSql = await File.ReadAllTextAsync(instnwndPath);

         using (var cn = new SqlConnection(GetSqlConnectionString("master"))) {
            await cn.OpenAsync();
            foreach (var batch in SplitSqlBatches(instnwndSql)) {
               if (!string.IsNullOrWhiteSpace(batch)) {
                  try {
                     await cn.ExecuteAsync(batch, commandTimeout: 120);
                  } catch (Exception ex) {
                     Console.WriteLine($"Warning executing Northwind setup batch: {ex.Message}");
                  }
               }
            }
         }

         Console.WriteLine("Adding RowVersion columns to Northwind tables...");

         var rowversionPath = Path.Combine(AppContext.BaseDirectory, "files", "northwind-rowversion.sql");
         if (!File.Exists(rowversionPath)) {
            throw new FileNotFoundException($"Northwind RowVersion script not found at {rowversionPath}");
         }

         var rowversionSql = await File.ReadAllTextAsync(rowversionPath);

         using (var cn = new SqlConnection(GetSqlConnectionString("NorthWind"))) {
            await cn.OpenAsync();
            foreach (var batch in SplitSqlBatches(rowversionSql)) {
               if (!string.IsNullOrWhiteSpace(batch)) {
                  try {
                     await cn.ExecuteAsync(batch, commandTimeout: 30);
                  } catch (Exception ex) {
                     Console.WriteLine($"Warning adding RowVersion: {ex.Message}");
                  }
               }
            }
         }

         Console.WriteLine("SQL Server initialization complete");
      }

      private static async Task InitializePostgres() {
         Console.WriteLine("Initializing PostgreSQL...");

         await using var cn = new NpgsqlConnection(GetPgConnectionString("postgres"));
         await cn.OpenAsync();

         foreach (var db in new[] { "northwind2", "junk" }) {
            await using var checkCmd = new NpgsqlCommand($"SELECT COUNT(*) FROM pg_database WHERE datname = '{db}'", cn);
            var count = (long)(await checkCmd.ExecuteScalarAsync())!;
            if (count == 0) {
               await using var createCmd = new NpgsqlCommand($"CREATE DATABASE {db}", cn);
               await createCmd.ExecuteNonQueryAsync();
               Console.WriteLine($"Created PostgreSQL database: {db}");
            }
         }

         Console.WriteLine("PostgreSQL initialization complete");
      }

      private static IEnumerable<string> SplitSqlBatches(string sql) {
         var batches = new List<string>();
         var lines = sql.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
         var currentBatch = new List<string>();

         foreach (var line in lines) {
            if (string.Equals(line.Trim(), "GO", StringComparison.OrdinalIgnoreCase)) {
               if (currentBatch.Count > 0) {
                  batches.Add(string.Join(Environment.NewLine, currentBatch));
                  currentBatch.Clear();
               }
            } else {
               currentBatch.Add(line);
            }
         }

         if (currentBatch.Count > 0) {
            batches.Add(string.Join(Environment.NewLine, currentBatch));
         }

         return batches;
      }
   }
}
