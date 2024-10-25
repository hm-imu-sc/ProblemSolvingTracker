using Grpc.Core;
using ProblemSolvingTracker.DataManager;
using ProblemSolvingTracker.Models;

namespace ProblemSolvingTracker.Services
{
    public class StudyMaterialService : StudyMaterialServices.StudyMaterialServicesBase
    {
        private readonly MyDbContext _dbContext;
        
        public StudyMaterialService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public override async Task<CreateStudyMaterialResponse> CreateStudyMaterial(CreateStudyMaterialRequest request, ServerCallContext context)
        {
            try
            {
                var validationResult = ValidateCreateStudyMaterialRequest(request);
                if (!validationResult.IsSuccess)
                {
                    return await Task.FromResult(new CreateStudyMaterialResponse { Id = -1, GeneralResponse = validationResult });
                }

                var newStudyMaterial = new StudyMaterial
                {
                    Title = request.StudyMaterial.Title,
                    Url = request.StudyMaterial.Url
                };
                await _dbContext.AddAsync(newStudyMaterial);
                await _dbContext.SaveChangesAsync();

                return await Task.FromResult(new CreateStudyMaterialResponse
                {
                    Id = newStudyMaterial.Id,
                    GeneralResponse = new GeneralResponse
                    {
                        IsSuccess = true,
                        Message = string.Empty
                    }
                });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new CreateStudyMaterialResponse
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

        public override async Task<GetAllStudyMaterialsResponse> GetAllStudyMaterials(GetAllStudyMaterialsRequest request, ServerCallContext context)
        {
            try
            {
                var studyMaterials = _dbContext.StudyMaterials.Select(sm => new StudyMaterialItem
                {
                    Id = sm.Id,
                    Title = sm.Title,
                    Url = sm.Url
                }).ToList();

                var getAllStudyMaterialsResponse = new GetAllStudyMaterialsResponse();
                getAllStudyMaterialsResponse.StudyMaterials.AddRange(studyMaterials);
                getAllStudyMaterialsResponse.GeneralResponse = new GeneralResponse
                {
                    IsSuccess = true,
                    Message = string.Empty
                };

                return await Task.FromResult(getAllStudyMaterialsResponse);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new GetAllStudyMaterialsResponse
                {
                    GeneralResponse = new GeneralResponse
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    }
                });
            }
        }

        public override async Task<DeleteStudyMaterialResponse> DeleteStudyMaterial(DeleteStudyMaterialRequest request, ServerCallContext context)
        {
            try
            {
                if (_dbContext.StudyMaterials.Where(sm => sm.Id == request.Id).Any())
                {
                    var studyMateriaToRemove = _dbContext.StudyMaterials.Where(sm => sm.Id == request.Id).First();
                    _dbContext.Remove(studyMateriaToRemove);
                    await _dbContext.SaveChangesAsync();
                    return await Task.FromResult(new DeleteStudyMaterialResponse { GeneralResponse = new GeneralResponse { IsSuccess = true } });
                }

                return await Task.FromResult(new DeleteStudyMaterialResponse
                {
                    GeneralResponse = new GeneralResponse
                    {
                        IsSuccess = false,
                        Message = $"Material doesn't exists !!!"
                    }
                });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new DeleteStudyMaterialResponse
                {
                    GeneralResponse = new GeneralResponse
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    }
                });
            }
        }

        public override async Task<UpdateStudyMaterialResponse> UpdateStudyMaterial(UpdateStudyMaterialRequest request, ServerCallContext context)
        {
            try
            {
                var validationResult = ValidateUpdateStudyMaterialRequest(request);

                if (validationResult.IsSuccess)
                {
                    _dbContext.StudyMaterials.Where(sm => sm.Id == request.StudyMaterial.Id).First().Title = request.StudyMaterial.Title;
                    _dbContext.StudyMaterials.Where(sm => sm.Id == request.StudyMaterial.Id).First().Url = request.StudyMaterial.Url;
                    await _dbContext.SaveChangesAsync();
                }

                return await Task.FromResult(new UpdateStudyMaterialResponse { GeneralResponse = validationResult });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new UpdateStudyMaterialResponse
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

        public GeneralResponse ValidateCreateStudyMaterialRequest(CreateStudyMaterialRequest request)
        {
            var isValid = true;
            var message = string.Empty;

            if (string.IsNullOrEmpty(request.StudyMaterial.Title))
            {
                isValid = false;
                message = "Title cannot be empty";
            }
            else if (_dbContext.StudyMaterials.Any(sm => !string.IsNullOrEmpty(sm.Title) && sm.Title == request.StudyMaterial.Title))
            {
                isValid = false;
                message = $"A study material already exists with title \"{request.StudyMaterial.Title}\"";
            }
            else if (_dbContext.StudyMaterials.Any(sm => !string.IsNullOrEmpty(sm.Url) && sm.Url.Equals(request.StudyMaterial.Url)))
            {
                isValid = false;
                message = $"A study material already exists with url \"{request.StudyMaterial.Url}\"";
            }

            if (!isValid)
            {
                return new GeneralResponse { IsSuccess = false, Message = message };
            }

            return new GeneralResponse { IsSuccess = true, Message = string.Empty };
        }

        public GeneralResponse ValidateUpdateStudyMaterialRequest(UpdateStudyMaterialRequest request)
        {
            var isValid = true;
            var message = string.Empty;

            if (!_dbContext.StudyMaterials.Any(sm => sm.Id == request.StudyMaterial.Id))
            {
                isValid = false;
                message = "The material doesn't exists !";
            }
            else if (string.IsNullOrEmpty(request.StudyMaterial.Title) || string.IsNullOrEmpty(request.StudyMaterial.Url))
            {
                isValid = false;
                message = "Title and URL cannot be empty";
            }
            else if (_dbContext.StudyMaterials.Any(sm => sm.Id != request.StudyMaterial.Id && !string.IsNullOrEmpty(sm.Title) && sm.Title == request.StudyMaterial.Title))
            {
                isValid = false;
                message = $"A study material already exists with title \"{request.StudyMaterial.Title}\"";
            }
            else if (_dbContext.StudyMaterials.Any(sm => sm.Id != request.StudyMaterial.Id && !string.IsNullOrEmpty(sm.Url) && sm.Url.Equals(request.StudyMaterial.Url)))
            {
                isValid = false;
                message = $"A study material already exists with url \"{request.StudyMaterial.Url}\"";
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
