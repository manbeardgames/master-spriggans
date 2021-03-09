namespace MasterSpriggans.Data
{
    public interface IDBFactory
    {
        MasterSpriggansDatabaseContext GetConnection();
    }
}