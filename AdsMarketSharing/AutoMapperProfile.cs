using AutoMapper;
using AdsMarketSharing.Entities;
using AdsMarketSharing.DTOs.Account;
using AdsMarketSharing.DTOs.File;
using AdsMarketSharing.DTOs.User;
using AdsMarketSharing.DTOs.Product;
using System.Linq;

namespace AdsMarketSharing
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            // Account 
            CreateMap<RegisterNewAccountDTO, Account>();
            CreateMap<Account, GetAccountInfoDTO>();
            CreateMap<AssignRoleToAccountDTO, AccountRole>();

            // Attachment
            CreateMap<AttachmentResponseDTO, Attachment>();

            // User
            CreateMap<GenerateUserRequestDTO, User>();

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
                .ForMember(dto => dto.UserPageName, ent => ent.MapFrom(p => p.UserPage.Name))
                .ForMember(dto => dto.UserPageAvatar, ent => ent.MapFrom(p => p.UserPage.BannerUrl))
                .ForMember(dto => dto.Urls, ent => ent.MapFrom(p => p.Attachments.Select(a => a.PublicPath)))
                .ForMember(dto => dto.ProductDetails, ent => ent.MapFrom(p => p.ProductClassifies
                    .SelectMany(pc => pc.ProductClassifyTypes
                        .Where(type => type.ProductClassifyValues.Count != 0)
                            .SelectMany(type => type.ProductClassifyValues)
                )));

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
            CreateMap<UserPage, GetUserPageResponseDTO>();
        }
    }
}
