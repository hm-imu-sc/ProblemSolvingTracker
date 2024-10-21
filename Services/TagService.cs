using Google.Protobuf;
using Grpc.Core;
using ProblemSolvingTracker.DataManager;
using ProblemSolvingTracker.Models;
using System.ComponentModel.DataAnnotations;

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
            try
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
            catch (Exception ex)
            {
                return await Task.FromResult(new CreateTagResponse
                {
                    Id = -1,
                    GeneralResponse = new GeneralResponse
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    }
                });
            }
        }

        public override async Task<GetAllTagsResponse> GetAllTags(GetAllTagsRequest request, ServerCallContext context)
        {
            try
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
            catch (Exception ex)
            {
                return await Task.FromResult(new GetAllTagsResponse
                {
                    GeneralResponse = new GeneralResponse
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    }
                });
            }
        }

        public override async Task<DeleteTagResponse> DeleteTag(DeleteTagRequest request, ServerCallContext context)
        {
            try
            {
                if (_dbContext.Tags.Where(t => t.Id == request.Id).Any())
                {
                    var tagToRemove = _dbContext.Tags.Where(t => t.Id == request.Id).First();
                    _dbContext.Remove(tagToRemove);
                    await _dbContext.SaveChangesAsync();
                    return await Task.FromResult(new DeleteTagResponse { GeneralResponse = new GeneralResponse { IsSuccess = true } });
                }

                return await Task.FromResult(new DeleteTagResponse
                {
                    GeneralResponse = new GeneralResponse
                    {
                        IsSuccess = false,
                        Message = $"Tag doesn't exists !!!"
                    }
                });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new DeleteTagResponse
                {
                    GeneralResponse = new GeneralResponse
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    }
                });
            }
        }

        public override async Task<UpdateTagResponse> UpdateTag(UpdateTagRequest request, ServerCallContext context)
        {
            try
            {
                var validationResult = ValidateUpdateTagRequest(request);   
        
                if (validationResult.IsSuccess)
                {
                    _dbContext.Tags.Where(t => t.Id == request.Tag.Id).First().Name = request.Tag.Name;
                    await _dbContext.SaveChangesAsync();
                }

                return await Task.FromResult(new UpdateTagResponse { GeneralResponse = validationResult });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new UpdateTagResponse
                {
                    GeneralResponse = new GeneralResponse
                    {
                        IsSuccess = false,
                        Message = ex.Message,
                    }
                });
            }
        }

        #region Validators

        public GeneralResponse ValidateCreateTagRequest(CreateTagRequest request)
        {
            var isValid = true;
            var message = string.Empty;

            if (string.IsNullOrEmpty(request.Name))
            {
                isValid = false;
                message = "Tag name cannot be empty";
            }

            if (_dbContext.Tags.Where(t => t.Name == request.Name).Any())
            {
                isValid = false;
                message = $"A tag already exists with name \"{request.Name}\"";
            }

            if (!isValid)
            {
                return new GeneralResponse { IsSuccess = false, Message = message };
            }

            return new GeneralResponse { IsSuccess = true, Message = string.Empty };
        }

        public GeneralResponse ValidateUpdateTagRequest(UpdateTagRequest request)
        {
            var isValid = true;
            var message = string.Empty;

            if (!_dbContext.Tags.Where(t => t.Id == request.Tag.Id).Any())
            {
                isValid = false;
                message = "The requested tag doesn't exists !";
            }
            else if (string.IsNullOrEmpty(request.Tag.Name)) 
            {
                isValid = false;
                message = "The new tag name cannot be empty !";
            }
            else if (_dbContext.Tags.Where(t => t.Name == request.Tag.Name).Any())
            {
                isValid = false;
                message = $"A tag already exists with name '{request.Tag.Name}'";
            }

            return new GeneralResponse
            {
                IsSuccess = isValid,
                Message = message
            };
        }

        #endregion
    }
}
