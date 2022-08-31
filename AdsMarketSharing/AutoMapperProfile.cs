using AutoMapper;
using AdsMarketSharing.Entities;
using AdsMarketSharing.DTOs.Account;
using AdsMarketSharing.DTOs.File;
using AdsMarketSharing.DTOs.User;
using AdsMarketSharing.DTOs.Product;
using System.Linq;
using Microsoft.AspNetCore.Http;

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
            CreateMap<AssignCategoryToProductDTO, ProductCategory>();
            CreateMap<Product, GetProductResponseDTO>()
                .ForMember(dto => dto.ProductCategories, ent => ent.MapFrom(p => p.ProductCategories.OrderBy(pc => pc.Category.Level).Select(pc => pc.Category)))
                .ForMember(dto => dto.UserPageName, ent => ent.MapFrom(p => p.UserPage.Name))
                .ForMember(dto => dto.UserPageAvatar, ent => ent.MapFrom(p=>p.UserPage.BannerUrl))
                .ForMember(dto => dto.Urls, ent => ent.MapFrom(p => p.Attachments.Select(a => a.PublicPath)));
            
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
