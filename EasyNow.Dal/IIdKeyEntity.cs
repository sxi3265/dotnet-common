using System;

namespace EasyNow.Dal
{
    public interface IIdKeyEntity:IEntity
    {
        Guid Id { get; set; }
    }
}