using System;

namespace EasyNow.Service.Db
{
    public interface IUserResolver<T>
    {
        T GetUserIdentity(string user);
    }
}