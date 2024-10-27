using Google.Protobuf.Collections;
using Grpc.Core;
using ManagerClasses;
using Microsoft.EntityFrameworkCore;
using ProblemSolvingTracker.DataManager;
using ProblemSolvingTracker.Models;

namespace ProblemSolvingTracker.Services
{
    public class TopicService : TopicServices.TopicServicesBase
    {
        private readonly MyDbContext _dbContext;

        public TopicService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<CreateTopicResponse> CreateTopic(CreateTopicRequest request, ServerCallContext context)
        {
            try
            {
                var validationResult = ValidateCreateTopicRequest(request);
                if (!validationResult.IsSuccess)
                {
                    return await Task.FromResult(new CreateTopicResponse { Id = -1, GeneralResponse = validationResult });
                }

                var newTopic = new Topic
                {
                    Name = request.Topic.Name,
                    StudyMaterials = _dbContext.StudyMaterials.Get(request.Topic.StudyMaterials.Select(sm => sm.Id).ToList()),
                    Tags = _dbContext.Tags.Get(request.Topic.Tags.Select(t => t.Id).ToList())
                };
                await _dbContext.AddAsync(newTopic);
                await _dbContext.SaveChangesAsync();

                return await Task.FromResult(new CreateTopicResponse
                {
                    Id = newTopic.Id,
                    GeneralResponse = new GeneralResponse
                    {
                        IsSuccess = true,
                        Message = string.Empty
                    }
                });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new CreateTopicResponse
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

        public override async Task<GetAllTopicsResponse> GetAllTopics(GetAllTopicsRequest request, ServerCallContext context)
        {
            try
            {
                var topics = new RepeatedField<TopicItem>();
                await _dbContext.Topics.ForEachAsync(t =>
                {
                    var topicItem = new TopicItem
                    {
                        Id = t.Id,
                        Name = t.Name,
                    };
                    topicItem.StudyMaterials.AddRange(t.StudyMaterials.Select(sm => new StudyMaterialItem
                    {
                        Id = sm.Id,
                        Title = sm.Title,
                        Url = sm.Url
                    }));
                    topicItem.Tags.AddRange(t.Tags.Select(tag => new TagItem
                    {
                        Id = tag.Id,
                        Name = tag.Name
                    }));
                    topics.Add(topicItem);
                });

                var getAllTopicsResponse = new GetAllTopicsResponse();
                getAllTopicsResponse.Topics.AddRange(topics);
                getAllTopicsResponse.GeneralResponse = new GeneralResponse
                {
                    IsSuccess = true,
                    Message = string.Empty
                };

                return await Task.FromResult(getAllTopicsResponse);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new GetAllTopicsResponse
                {
                    GeneralResponse = new GeneralResponse
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    }
                });
            }
        }

        public override async Task<DeleteTopicResponse> DeleteTopic(DeleteTopicRequest request, ServerCallContext context)
        {
            try
            {
                if (_dbContext.Topics.Contains(request.Id))
                {
                    var studyMateriaToRemove = _dbContext.Topics.Get(request.Id);
                    _dbContext.Remove(studyMateriaToRemove);
                    await _dbContext.SaveChangesAsync();
                    return await Task.FromResult(new DeleteTopicResponse { GeneralResponse = new GeneralResponse { IsSuccess = true } });
                }

                return await Task.FromResult(new DeleteTopicResponse
                {
                    GeneralResponse = new GeneralResponse
                    {
                        IsSuccess = false,
                        Message = $"Topic doesn't exist !!!"
                    }
                });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new DeleteTopicResponse
                {
                    GeneralResponse = new GeneralResponse
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    }
                });
            }
        }

        public override async Task<UpdateTopicResponse> UpdateTopic(UpdateTopicRequest request, ServerCallContext context)
        {
            try
            {
                var validationResult = ValidateUpdateTopicRequest(request);

                if (validationResult.IsSuccess)
                {
                    var topic = _dbContext.Topics.Get(request.Topic.Id);
                    topic.Name = request.Topic.Name;
                    topic.StudyMaterials = _dbContext.StudyMaterials.Get(request.Topic.StudyMaterials.Select(sm => sm.Id).ToList());
                    topic.Tags = _dbContext.Tags.Get(request.Topic.Tags.Select(sm => sm.Id).ToList());
                    await _dbContext.SaveChangesAsync();
                }

                return await Task.FromResult(new UpdateTopicResponse { GeneralResponse = validationResult });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new UpdateTopicResponse
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

        public GeneralResponse ValidateCreateTopicRequest(CreateTopicRequest request)
        {
            var isValid = true;
            var message = string.Empty;

            if (request.Topic.Name.IsNullOrEmpty())
            {
                isValid = false;
                message = "Topic name cannot be empty";
            }
            else if (_dbContext.Topics.ToList().Any(t => !t.Name.IsNullOrEmpty() && t.Name.IsEqualTo(request.Topic.Name)))
            {
                isValid = false;
                message = $"A topic already exists with name \"{request.Topic.Name}\"";
            }
            else if (!_dbContext.StudyMaterials.Contains(request.Topic.StudyMaterials.Select(sm => sm.Id).ToList()))
            {
                isValid = false;
                message = $"Some of the study materials you passed do not exist !";
            }
            else if (!_dbContext.Tags.Contains(request.Topic.Tags.Select(t => t.Id).ToList()))
            {
                isValid = false;
                message = $"Some of the tags you passed do not exist !";
            }

            if (!isValid)
            {
                return new GeneralResponse { IsSuccess = false, Message = message };
            }

            return new GeneralResponse { IsSuccess = true, Message = string.Empty };
        }

        public GeneralResponse ValidateUpdateTopicRequest(UpdateTopicRequest request)
        {
            var isValid = true;
            var message = string.Empty;

            if (!_dbContext.Topics.Contains(request.Topic.Id))
            {
                isValid = false;
                message = "The topic doesn't exists !";
            }
            else if (request.Topic.Name.IsNullOrEmpty())
            {
                isValid = false;
                message = "Topic name cannot be empty";
            }
            else if (_dbContext.Topics.Any(t => !t.Name.IsNullOrEmpty() && t.Name.IsEqualTo(request.Topic.Name)))
            {
                isValid = false;
                message = $"A topic already exists with name \"{request.Topic.Name}\"";
            }
            else if (!_dbContext.StudyMaterials.Contains(request.Topic.StudyMaterials.Select(sm => sm.Id).ToList()))
            {
                isValid = false;
                message = $"Some of the study materials you passed do not exist !";
            }
            else if (!_dbContext.Tags.Contains(request.Topic.Tags.Select(t => t.Id).ToList()))
            {
                isValid = false;
                message = $"Some of the tags you passed do not exist !";
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
