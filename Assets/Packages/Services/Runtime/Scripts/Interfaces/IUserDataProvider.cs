using Xennial.Services;

namespace Xennial.API
{
    public interface IUserDataProvider<T> : IService
    {
        T GetData();
        void SetData(T data);
    }
}