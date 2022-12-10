using AutoMapper;
using AdsMarketSharing.Entities;
using AdsMarketSharing.DTOs.Account;
using AdsMarketSharing.DTOs.File;
using AdsMarketSharing.DTOs.User;
using AdsMarketSharing.DTOs.UserPage;
using AdsMarketSharing.DTOs.Product;
using System.Linq;
using AdsMarketSharing.DTOs.Payment;
using AdsMarketSharing.DTOs.Review;
using AdsMarketSharing.DTOs.Notification;

namespace AdsMarketSharing
{
    public class AutoMapperProfile: Profile
    {
        private const string defaultAvatar = "https://cdn.sforum.vn/sforum/wp-content/uploads/2021/07/cute-astronaut-wallpaperize-amoled-clean-scaled.jpg";
        public AutoMapperProfile()
        {
            // Account 
            CreateMap<RegisterNewAccountDTO, Account>();
            CreateMap<Account, GetAccountInfoDTO>();
            CreateMap<AssignRoleToAccountDTO, AccountRole>();


            //Role
            CreateMap<Role, GetRoleDTO>()
                .ForMember(dto=>dto.RoleName, ent => ent.MapFrom(p => p.Name));

            // Attachment
            CreateMap<AttachmentResponseDTO, Attachment>();

            // User
            CreateMap<GenerateUserRequestDTO, User>()
                .ForMember(ent => ent.OrganizationName, dto => dto.MapFrom(u => u.OrganizationName))
                .ForMember(ent => ent.Biography, dto => dto.MapFrom(u => u.Biography));
            CreateMap<User, GetUserResponseDTO>()
                .ForMember(dto => dto.Avatar, ent => ent.MapFrom(p => p.AttachmentId != 0 ? p.Avatar.PublicPath : defaultAvatar));
            CreateMap<User, GetUserWithoutBiographyDTO>()
                .ForMember(dto => dto.Avatar, ent => { 
                    ent.AllowNull();
                    ent.MapFrom(p => p.AttachmentId > 0 ? p.Avatar.PublicPath : defaultAvatar);
                });
            CreateMap<User, GetUserByAdminDTO>()
                .ForMember(dto => dto.Avatar, ent => ent.MapFrom(p => p.Avatar.PublicPath))
                .ForMember(dto => dto.Email, ent => ent.MapFrom(p => p.Account.Email))
                .ForMember(dto => dto.Enabled, ent => ent.MapFrom(p => p.Account.Enabled))
                .ForMember(dto => dto.IsActive, ent => ent.MapFrom(p => p.Account.IsActive))
                .ForMember(dto => dto.RegisteredTime, ent => ent.MapFrom(p => p.Account.RegisteredTime))
                .ForMember(dto => dto.UpdatedTime, ent => ent.MapFrom(p => p.Account.UpdatedTime));
            CreateMap<RegisterAccountWithUserDTO, User>()
                .ForMember(dto => dto.Avatar, ent => ent.MapFrom(p => p.Avatar))
                .ForMember(dto => dto.Account, ent => ent.MapFrom(p => p.Account));

            // Address
            CreateMap<AddAddressRequestDTO, ReceiverAddress>();
            CreateMap<UpdateAddressRequestDTO, ReceiverAddress>();

            // Product
            CreateMap<AddProductRequestDTO, Product>()
                .ForMember(ent => ent.Attachments, dto =>
                {
                    dto.AllowNull();
                    dto.MapFrom(p => p.Attachments.Select(p => new AddProductAttachmentRequestDTO() { Attachment = p }));
                })
                .ForMember(ent => ent.ProductCategories, dto => dto.MapFrom(p => p.CategoryIds.Select(id => new ProductCategory() { CategoryId = id })));
                
                //.ForMember(ent => ent.ProductClassifies.SelectMany(pc => pc.ProductClassifyTypes)
                //        , dto => dto.MapFrom(p => p.ProductDetails
                //            .Select(pd => new AddProductClassifyDetailRequestDTO()
                //            {
                //                Price = pd.Price,
                //                Image = pd.Image,
                //                Inventory = pd.Inventory,
                //                PresentImage = pd.PresentImage,
                //                ClassifyTypeKey = pd.ClassifyIndexes.Select((i, index) =>
                //                {
                //                    if (index == 0) return p.ProductClassifies.SelectMany(pc => pc.);
                //                    return null;
                //                }),
                //                ClassifyTypeValue = pd.ClassifyIndexes.Select((i, index) =>
                //                {
                //                    if (index == 1) return p.ProductClassifies;
                //                    return null;
                //                })
                //            })));
            CreateMap<AssignCategoryToProductDTO, ProductCategory>();
            CreateMap<Product, GetProductResponseDTO>()
                .ForMember(dto => dto.ProductCategories, ent => ent.MapFrom(p => p.ProductCategories.OrderBy(pc => pc.Category.Level).Select(pc => pc.Category)))
                .ForMember(dto => dto.Urls, ent => ent.MapFrom(p => p.Attachments.Select(a => a.Attachment.PublicPath)))
                .ForMember(dto => dto.ProductDetails, ent => ent.MapFrom(p => p.ProductClassifies
                    .SelectMany(pc => pc.ProductClassifyTypes
                        .Where(type => type.ProductClassifyValues.Count != 0)
                            .SelectMany(type => type.ProductClassifyValues)
                )))
                .ForMember(dto => dto.ReviewAmount, ent => ent.MapFrom(p => p.Reviews.Count));
            CreateMap<Product, RecentProductResponseDTO>()
                .ForMember(dto => dto.ProductName, ent => ent.MapFrom(p => p.Name))
                .ForMember(dto => dto.ProductImage, ent => ent.MapFrom(p => p.Attachments[0].Attachment.PublicPath))
                .ForMember(dto => dto.SellerName, ent => ent.MapFrom(p => p.UserPage.Name))
                .ForMember(dto => dto.SellerAvatar, ent => ent.MapFrom(p => p.UserPage.PageAvatar.PublicPath))
                .ForMember(dto => dto.OrderAmount, ent => ent.MapFrom(p => (long)p.Orders.Count));

            // ProductAttachment
            CreateMap<AddProductAttachmentRequestDTO, ProductAttachment>()
                .ForMember(ent => ent.Product, dto => dto.MapFrom(p => p.Product))
                .ForMember(ent => ent.Attachment, dto => dto.MapFrom(p => p.Attachment));
            // Classify
            CreateMap<AddProductClassifyRequestDTO, ProductClassify>()
                .ForMember(ent => ent.ProductClassifyTypes, 
                    dto => dto.MapFrom(prop => prop.ClassifyTypes.Select(type => new ProductClassifyType()
                    {
                        Name = type
                    })
                ));
            CreateMap<ProductClassify, GetProductClassifyResponseDTO>()
                .ForMember(dto => dto.ClassifyTypes, ent => ent.MapFrom(prop => prop.ProductClassifyTypes));
               
            // Classify Type
            CreateMap<AddProductClassifyTypeKeyRequestDTO, ProductClassifyType>();
            CreateMap<AddProductClassifyTypeValueRequestDTO, ProductClassifyType>();
            CreateMap<AddProductClassifyTypeRequestDTO, ProductClassifyType>();
            CreateMap<ProductClassifyType, GetProductClassifyTypeResponseDTO>();

            // Classify Type Detail
            CreateMap<AddProductClassifyDetailRequestDTO, ProductClassfiyDetail>();
            CreateMap<ProductClassfiyDetail, GetProductClassifyDetailResponseDTO>()
                .ForMember(dto => dto.PresentImage, dto => dto.MapFrom(p => p.PresentImage.PublicPath))
                .ForMember(dto => dto.ProductClassifyKeyId, ent => ent.MapFrom(prop => prop.ClassifyTypeKeyId))
                .ForMember(dto => dto.ProductClassifyValueId, ent => ent.MapFrom(prop => prop.ClassifyTypeValueId))
                .ForMember(dto => dto.ProductClassifyKey, ent => ent.MapFrom(prop => prop.ClassifyTypeKey.Name))
                .ForMember(dto => dto.ProductClassifyValue, ent => ent.MapFrom(prop => prop.ClassifyTypeValue.Name));

            // Category
            CreateMap<AddCategoryRequestDTO, Category>();
            CreateMap<UpdateCategoryRequestDTO, Category>();
            CreateMap<Category, GetCategoryResponseDTO>()
                .ForMember(dto => dto.ParentId, ent => ent.MapFrom(c => c.ParentCategoryId))
                .ForMember(dto => dto.SubCategoryCount, ent => ent.MapFrom(c => c.SubCategories.Count));
                
            // UserPage
            CreateMap<UserPage, GetUserPageResponseDTO>()
                .ForMember(dto => dto.BannerUrl, ent => ent.MapFrom(up => up.BannerUrl != null ? up.BannerUrl.PublicPath: defaultAvatar))
                .ForMember(dto => dto.PageAvatar, ent => ent.MapFrom(up => up.PageAvatar != null ? up.PageAvatar.PublicPath: defaultAvatar));
            CreateMap<UserPageCreationDTO, UserPage>();
            CreateMap<UserPage, GetUserPageWithoutDescriptionDTO>()
                .ForMember(dto => dto.BannerUrl, ent => ent.MapFrom(up => up.BannerUrl != null ? up.BannerUrl.PublicPath : defaultAvatar))
                .ForMember(dto => dto.PageAvatar, ent => ent.MapFrom(up => up.PageAvatar != null ? up.PageAvatar.PublicPath : defaultAvatar));


            // Invoice Payment 
            CreateMap<InvoiceCreationDTO, Invoice>()
                .ForMember(ent => ent.CashAmount, dto => dto.MapFrom(p => p.Orders.Sum(p => p.Total)));
            
            CreateMap<PaymentCreationDTO, Payment>()
                .ForMember(ent => ent.Last4Digits, dto=> dto.MapFrom(p => p.CardNumber.Substring(p.CardNumber.Length - 5, 4)));

            CreateMap<Invoice, InvoiceResponseDTO>()
                //.ForMember(dto => dto.Shipping, ent => ent.MapFrom(p => p.Shipping.ToString()))
                .ForMember(dto => dto.Bank, ent => ent.MapFrom(p => p.Payment.BankCode.ToString()));

            // Order

            CreateMap<OrderCreationDTO, Order>();

            CreateMap<Order, OrderResponseDTO>()
                .ForMember(dto => dto.ProductName, ent => ent.MapFrom(p => p.Product.Name))
                .ForMember(dto => dto.Merchant, ent => ent.MapFrom(p => p.Merchant.Name))
                .ForMember(dto => dto.Address, ent => ent.MapFrom(p => $"{p.BuyerFullName} - {p.Address.StreetAddress} - {p.Address.Ward} - {p.Address.District} - {p.Address.City}"))
                .ForMember(dto => dto.InvoiceRef, ent => ent.MapFrom(p => p.Invoice.OnlineRef));
                //.ForMember(dto => dto.ProductImage, ent =>
                //{
                //    ent.AllowNull();
                //    ent.MapFrom(p => p.Product != null && p.Product.Attachments != null && p.Product.Attachments.ElementAt(0).Attachment != null ? p.Product.Attachments.ElementAt(0).Attachment.PublicPath : defaultAvatar);
                //});;


            // Review
            CreateMap<ReviewProductCreationDTO, Review>();
            CreateMap<Review, ReviewProductResponseDTO>()
                .ForMember(dto => dto.ReplyAmount, ent => ent.MapFrom(p => p.Replies != null ? p.Replies.Count : 0));

            //Reply
            CreateMap<ReplyReviewCreationDTO, Reply>();
            CreateMap<Reply, ReplyReviewResponseDTO>();

            //Notification
            CreateMap<CreateNotificationRequestDTO, Notification>()
                .ForMember(ent => ent.Notifytrackers, dto => dto.MapFrom(p => p.ToUsers.Select(np => new CreateNotificationTrackerDTO()
                {
                    UserId = np,
                    HasSeen = false
                })));
            CreateMap<CreateNotificationTrackerDTO, Notifytracker>();
            CreateMap<Notification, NotificationResponseDTO>()
                .ForMember(dto => dto.UserName, ent => ent.MapFrom(p => p.FromUser.OrganizationName));
            CreateMap<Notifytracker, NotificationTrackerResponseDTO>()
                .ForMember(dto => dto.Notification, ent => ent.MapFrom(p => p.Notification));
        }
    }
}
