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
                    Name = request.Topic.Name   
                };
                await _dbContext.AddAsync(newTopic);

                foreach (var studyMaterial in request.Topic.StudyMaterials)
                {
                    var newTopicSM = new TopicStudyMaterial
                    {
                        Topic = newTopic,
                        StudyMaterial = _dbContext.StudyMaterials.First(sm => sm.Id == studyMaterial.Id)
                    };
                    await _dbContext.AddAsync(newTopicSM);
                }

                foreach (var tag in request.Topic.Tags)
                {
                    var newTopicTag = new TopicTag
                    {
                        Topic = newTopic,
                        Tag = _dbContext.Tags.First(t => t.Id == tag.Id)
                    };
                    await _dbContext.AddAsync(newTopicTag);
                }

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
                await _dbContext.Topics.ForEachAsync(topic =>
                {
                    var topicItem = new TopicItem
                    {
                        Id = topic.Id,
                        Name = topic.Name,
                    };
                    topicItem.StudyMaterials.AddRange(_dbContext.TopicStudyMaterials.Where(tsm => tsm.Topic != null && tsm.Topic.Id == topic.Id).Select(tsm => new StudyMaterialItem
                    {
                        Id = tsm.StudyMaterial.Id,
                        Title = tsm.StudyMaterial.Title,
                        Url = tsm.StudyMaterial.Url
                    }));
                    topicItem.Tags.AddRange(_dbContext.TopicTags.Where(tt => tt.Topic != null && tt.Topic.Id == topic.Id).Select(tt => new TagItem
                    {
                        Id = tt.Tag.Id,
                        Name = tt.Tag.Name
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
                    var topicToRemove = _dbContext.Topics.Get(request.Id);
                    var topicStudyMaterialsToRemove = _dbContext.TopicStudyMaterials.Where(tsm => tsm.Topic != null && tsm.Topic.Id == topicToRemove.Id);
                    var topicTagsToRemove = _dbContext.TopicTags.Where(tt => tt.Topic != null && tt.Topic.Id == topicToRemove.Id);

                    _dbContext.RemoveRange(topicStudyMaterialsToRemove);
                    _dbContext.RemoveRange(topicTagsToRemove);
                    _dbContext.Remove(topicToRemove);

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

                    var studyMaterialToInsert = request.Topic.StudyMaterials
                        .AsEnumerable()
                        .Where(sm =>
                        {
                            return !_dbContext.TopicStudyMaterials
                                .Where(tsm => tsm.Topic != null && tsm.Topic.Id == request.Topic.Id)
                                .Select(tsm => tsm.StudyMaterial.Id)
                                .Contains(sm.Id);
                        })
                        .Select(sm => new TopicStudyMaterial
                        {
                            Topic = topic,
                            StudyMaterial = _dbContext.StudyMaterials.First(ssm => ssm.Id == sm.Id)
                        })
                        .ToList();

                    var studyMaterialsToDelete = _dbContext.TopicStudyMaterials
                        .AsEnumerable()
                        .Where(tsm =>
                        {
                            return tsm.Topic.Id == topic.Id && !request.Topic.StudyMaterials
                                .Select(sm => sm.Id)
                                .Contains(tsm.StudyMaterial.Id);
                        })
                        .ToList();

                    var tagsToInsert = request.Topic.Tags
                        .AsEnumerable()
                        .Where(t =>
                        {
                            return !_dbContext.TopicTags
                                .Where(tt => tt.Topic != null && tt.Topic.Id == request.Topic.Id)
                                .Select(tt => tt.Tag.Id)
                                .Contains(t.Id);
                        })
                        .Select(sm => new TopicTag
                        {
                            Topic = topic,
                            Tag = _dbContext.Tags.First(ssm => ssm.Id == sm.Id)
                        })
                        .ToList();

                    var tagsToDelete = _dbContext.TopicTags
                        .AsEnumerable()
                        .Where(tt =>
                        {
                            return tt.Topic.Id == topic.Id && !request.Topic.Tags
                                .Select(t => t.Id)
                                .Contains(tt.Tag.Id);
                        })
                        .ToList();

                    _dbContext.RemoveRange(studyMaterialsToDelete);
                    _dbContext.RemoveRange(tagsToDelete);

                    await _dbContext.AddRangeAsync(studyMaterialToInsert);
                    await _dbContext.AddRangeAsync(tagsToInsert);

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
            else if (_dbContext.Topics.Where(t => t.Id != request.Topic.Id).ToList().Any(t => !t.Name.IsNullOrEmpty() && t.Name.IsEqualTo(request.Topic.Name)))
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
