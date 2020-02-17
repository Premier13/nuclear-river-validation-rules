using LinqToDB.Data;

namespace NuClear.ValidationRules.SingleCheck.Tenancy
{
    public interface IDataConnectionProvider
    {
        DataConnection CreateErmConnection();
        DataConnection CreateVrConnection();
    }
}
