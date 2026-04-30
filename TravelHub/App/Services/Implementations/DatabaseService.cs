using App.Models;
using App.Services.Interfaces;
using SQLite;
using System.Diagnostics;

namespace App.Services.Implementations;

public class DatabaseService : IDatabaseService
{
    private readonly SemaphoreSlim _sync = new(1, 1);
    private SQLiteAsyncConnection? _dbSQL;

    public SQLiteAsyncConnection? DBSQL => _dbSQL;

    public async Task<SQLiteAsyncConnection> InitSQL()
    {
        if (_dbSQL != null)
        {
            return _dbSQL;
        }

        await _sync.WaitAsync();
        try
        {
            if (_dbSQL != null)
            {
                return _dbSQL;
            }

            var dbDir = FileSystem.AppDataDirectory;
            var dbPath = Path.Combine(dbDir, "TravelHub.db3");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath) ?? dbDir);

            _dbSQL = new SQLiteAsyncConnection(dbPath);
            await _dbSQL.CreateTableAsync<Country>();
            await _dbSQL.CreateTableAsync<City>();

            return _dbSQL;
        }
        finally
        {
            _sync.Release();
        }
    }

    public async Task DeleteAllDatabaseFiles()
    {
        await _sync.WaitAsync();
        try
        {
            if (_dbSQL != null)
            {
                await _dbSQL.CloseAsync();
                _dbSQL = null;
            }

            var directoryPath = FileSystem.AppDataDirectory;
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            var dbFiles = Directory.GetFiles(directoryPath, "*.db3");
            foreach (var file in dbFiles)
            {
                try
                {
                    File.Delete(file);
                    Debug.WriteLine($"Base de datos eliminada: {file}");
                }
                catch (IOException ex)
                {
                    Debug.WriteLine($"No se pudo eliminar {file}. Error: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.WriteLine($"No se pudo eliminar {file}. Error: {ex.Message}");
                }
            }
        }
        finally
        {
            _sync.Release();
        }
    }
}
