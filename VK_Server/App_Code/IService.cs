using System.Runtime.Serialization;
using System.ServiceModel;

// ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Реструктуризация" можно использовать для одновременного изменения имени интерфейса "IService" в коде и файле конфигурации.
[ServiceContract]
public interface IService
{

    [OperationContract]
    string GetData(int value);

}