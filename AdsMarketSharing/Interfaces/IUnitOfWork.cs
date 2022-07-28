using System.Threading.Tasks;

namespace AdsMarketSharing.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IAttachmentRepository AttachmentRepository { get; }
        Task CompleteAsync();
    }
}
