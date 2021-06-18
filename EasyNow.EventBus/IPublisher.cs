using System.Threading.Tasks;

namespace EasyNow.EventBus
{
    public interface IPublisher
    {
        void Publish<T>(string name, T data);

        Task PublishAsync<T>(string name, T data);
    }
}