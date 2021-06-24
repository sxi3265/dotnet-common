namespace EasyNow.Service.Db
{
    public interface IUserResolver
    {
        T GetUserIdentity<T>(string user);
    }
}