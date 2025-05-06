namespace Teammato.Utils;


using Akavache.Sqlite3;
public static class LinkerPreserve
{
    static LinkerPreserve()
    {
        var persistentName = typeof(SQLitePersistentBlobCache).FullName;
        var encryptedName = typeof(SQLiteEncryptedBlobCache).FullName;
    }
}