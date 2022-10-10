using AdsMarketSharing.Data;
using AdsMarketSharing.DTOs.Review;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using AdsMarketSharing.DTOs.Review;
using AdsMarketSharing.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace AdsMarketSharing.Hubs
{
    public class ReviewHub : Hub
    {
        private readonly SQLExpressContext _dbContext;
        private readonly IMapper _mapper;

        public ReviewHub(SQLExpressContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        // Mean: Join to see new review of product as real-time (Include: logged-user or un-joined user)
        [HubMethodName("joinReviewGroup")]
        public Task JoinReviewGroup(int productId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"Review-{productId}");
        }
        [HubMethodName("leaveReviewGroup")]
        public Task LeaveReviewGroup(int productId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Review-{productId}");
        }

        [Authorize]
        [HubMethodName("sendReviewToMerchant")]
        public Task SendReviewToMerchant(ReviewProductCreationDTO request)
        {
            var updatedReview = CreateNewReviewProduct(request);

            return Clients.Group($"Review-{request.ProductId}").SendAsync(ReviewBroadCastConstraint.RECEIVE_REVIEW, updatedReview);
        }
        [Authorize]
        [HubMethodName("sendReplyReviewToMerchant")]
        public Task SendReplyReview(ReplyReviewCreationDTO request, int accountId)
        {
            string accountIdStr = Context.User.Claims.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            int.TryParse(accountIdStr, out int accountIdNumber);

            var updatedReview = CreateNewReplyReviewProduct(request);

            return Clients.All.SendAsync(ReviewBroadCastConstraint.RECEIVE_REVIEW, _mapper.Map<ReviewProductResponseDTO>(updatedReview));
        }

        private ReviewProductResponseDTO CreateNewReviewProduct(ReviewProductCreationDTO request) {
            var foundReviewFromUser = _dbContext.Reviews
                                                .Include(p => p.User)
                                                    .ThenInclude(p => p.Avatar)
                                                .FirstOrDefault(p => p.UserId == request.UserId && p.ProductId == request.ProductId);

            var updatedReview = _mapper.Map(request, foundReviewFromUser);

            _dbContext.Reviews.Update(updatedReview);

            _dbContext.SaveChanges();

            if(foundReviewFromUser == null)
            {
                updatedReview.User = _dbContext.Users.FirstOrDefault(p => p.Id == request.UserId);
            }

            return _mapper.Map<ReviewProductResponseDTO>(updatedReview);
        }

        private ReplyReviewResponseDTO CreateNewReplyReviewProduct(ReplyReviewCreationDTO request)
        {
            var foundReplyReviewFromUser = _dbContext.Reviews.FirstOrDefault(p => p.Id == request.ReviewId);

            if (foundReplyReviewFromUser is null)
            {
                return null;
            }

            var newReplyReview = _mapper.Map<Reply>(request);

            _dbContext.Replies.Add(newReplyReview);

            _dbContext.SaveChanges();

            return _mapper.Map<ReplyReviewResponseDTO>(newReplyReview);
        }
    }
}
