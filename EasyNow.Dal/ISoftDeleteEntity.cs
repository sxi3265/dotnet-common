namespace EasyNow.Dal
{
    public interface ISoftDeleteEntity : IEntity
    {
        bool IsDeleted{ get; set; }
    }
}