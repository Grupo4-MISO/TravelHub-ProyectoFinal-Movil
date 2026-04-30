using SQLite;

namespace App.Services.Interfaces;

public interface IDatabaseService
{
    SQLiteAsyncConnection? DBSQL { get; }
    Task<SQLiteAsyncConnection> InitSQL();
    Task DeleteAllDatabaseFiles();
}
