using Grpc.Core;
using ProblemSolvingTracker.DataManager;
using ProblemSolvingTracker.Models;

namespace ProblemSolvingTracker.Services
{
    public class TagService : TagServices.TagServicesBase
    {
        private readonly MyDbContext _dbContext;

        public TagService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<CreateTagResponse> CreateTag(CreateTagRequest request, ServerCallContext context)
        {
            var validationResult = ValidateCreateTagRequest(request);
            if (!validationResult.IsSuccess)
            {
                return await Task.FromResult(new CreateTagResponse { Id = -1, GeneralResponse = validationResult });
            }

            var newTag = new Tag
            {
                Name = request.Name
            };
            await _dbContext.AddAsync(newTag);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new CreateTagResponse
            {
                Id = newTag.Id,
                GeneralResponse = new GeneralResponse
                {
                    IsSuccess = true,
                    Message = string.Empty
                }
            });
        }

        public override async Task<GetAllTagsResponse> GetAllTags(GetAllTagsRequest request, ServerCallContext context)
        {
            var tags = _dbContext.Tags.Select(t => new TagItem
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();

            var getAllTagsResponse = new GetAllTagsResponse();
            getAllTagsResponse.Tags.AddRange(tags);
            getAllTagsResponse.GeneralResponse = new GeneralResponse
            {
                IsSuccess = true,
                Message = string.Empty
            };

            return await Task.FromResult(getAllTagsResponse);
        }

        #region Validators

        public GeneralResponse ValidateCreateTagRequest(CreateTagRequest request)
        {
            var isValid = true;
            var message = String.Empty;

            if (string.IsNullOrEmpty(request.Name))
            {
                isValid = false;
                message = "Tag name cannot be empty";
            }

            if (_dbContext.Tags.Where(t => t.Name == request.Name).Any())
            {
                isValid = false;
                message = $"A tag already exists with name '{request.Name}'";
            }

            if (!isValid)
            {
                return new GeneralResponse { IsSuccess = false, Message = message };
            }

            return new GeneralResponse { IsSuccess = true, Message = string.Empty };
        }

        #endregion
    }
}
