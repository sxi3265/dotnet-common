namespace EasyNow.Dal
{
    public interface IUserResolver<T>
    {
        T GetUserIdentity(string user);
    }
}