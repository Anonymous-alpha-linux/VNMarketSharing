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

            // Address
            CreateMap<AddAddressRequestDTO, ReceiverAddress>();
            CreateMap<UpdateAddressRequestDTO, ReceiverAddress>();

            // Product
            CreateMap<AddProductRequestDTO, Product>()
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
                .ForMember(dto => dto.Urls, ent => ent.MapFrom(p => p.Attachments.Select(a => a.PublicPath)))
                .ForMember(dto => dto.ProductDetails, ent => ent.MapFrom(p => p.ProductClassifies
                    .SelectMany(pc => pc.ProductClassifyTypes
                        .Where(type => type.ProductClassifyValues.Count != 0)
                            .SelectMany(type => type.ProductClassifyValues)
                )))
                .ForMember(dto => dto.ReviewAmount, ent => ent.MapFrom(p => p.Reviews.Count));

            // Classify
            CreateMap<AddProductClassifyRequestDTO, ProductClassify>()
                .ForMember(ent => ent.ProductClassifyTypes, 
                    dto => dto.MapFrom(prop => prop.ClassifyTypes.Select(type => new ProductClassifyType()
                    {
                        Name = prop.Name
                    })
                ));
                //.ForMember(ent => ent.ProductClassifyTypes.Select(type => type.ProductClassifyValues), dto => dto.MapFrom(prop => ))
                //.ForMember(ent => ent.ProductClassifyTypes.Select(type => type.ProductClassifyKeys), dto => dto.MapFrom(prop => prop));
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


            // Payment
            CreateMap<InvoiceCreationDTO, Invoice>()
                .ForMember(ent => ent.CashAmount, dto => dto.MapFrom(p => p.Orders.Sum(p => p.Total)));
            
            CreateMap<OrderCreationDTO, Order>();

            CreateMap<PaymentCreationDTO, Payment>()
                .ForMember(ent => ent.Last4Digits, dto=> dto.MapFrom(p => p.CardNumber.Substring(p.CardNumber.Length - 5, 4)));

            CreateMap<Invoice, InvoiceResponseDTO>()
                //.ForMember(dto => dto.Shipping, ent => ent.MapFrom(p => p.Shipping.ToString()))
                .ForMember(dto => dto.Bank, ent => ent.MapFrom(p => p.Payment.BankCode.ToString()));

            CreateMap<Order, OrderResponseDTO>()
                .ForMember(dto => dto.ProductName, ent => ent.MapFrom(p => p.Product.Name))
                .ForMember(dto => dto.Merchant, ent => ent.MapFrom(p => p.Merchant.Name))
                .ForMember(dto => dto.Address, ent => ent.MapFrom(p => $"{p.BuyerFullName} - {p.Address.StreetAddress} - {p.Address.Ward} - {p.Address.District} - {p.Address.City}"))
                .ForMember(dto => dto.InvoiceRef, ent => ent.MapFrom(p => p.Invoice.OnlineRef));


            // Review
            CreateMap<ReviewProductCreationDTO, Review>();
            CreateMap<Review, ReviewProductResponseDTO>()
                .ForMember(dto => dto.ReplyAmount, ent => ent.MapFrom(p => p.Replies != null ? p.Replies.Count : 0));

            //Reply
            CreateMap<ReplyReviewCreationDTO, Reply>();
            CreateMap<Reply, ReplyReviewResponseDTO>();
        }
    }
}
