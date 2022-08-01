using System.Threading.Tasks;

namespace AdsMarketSharing.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IAttachmentRepository AttachmentRepository { get; }
        IReceiverAddressRepository ReceiverAddressRepository { get; }
        IProductionCategoryRepository ProductionCategoryRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        Task CompleteAsync();
    }
}
