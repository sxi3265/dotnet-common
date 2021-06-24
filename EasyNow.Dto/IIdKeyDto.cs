using System;

namespace EasyNow.Dto
{
    public interface IIdKeyDto:IDto
    {
        Guid Id { get; set; }
    }
}