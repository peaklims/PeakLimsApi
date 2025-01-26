namespace PeakLims.Domain.WorldBuildingPhases;

using System.ComponentModel.DataAnnotations;
using PeakLims.Domain.WorldBuildingAttempts;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using Destructurama.Attributed;
using PeakLims.Exceptions;
using PeakLims.Domain.WorldBuildingPhases.DomainEvents;
using PeakLims.Domain.WorldBuildingStatuses;
using Utilities;
using WorldBuildingPhaseNames;
using ValidationException = Exceptions.ValidationException;

public class WorldBuildingPhase : BaseEntity
{
   public WorldBuildingPhaseName Name { get; private set; }

   public WorldBuildingStatus Status { get; private set; }

    public string ResultData { get; private set; }

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? EndedAt { get; private set; }

    public int StepNumber { get; private set; }

    public string Comments { get; private set; }

    public string ErrorMessage { get; private set; }

    public string SpecialRequests { get; private set; }

    public int AttemptNumber { get; private set; }

    public WorldBuildingAttempt WorldBuildingAttempt { get; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static WorldBuildingPhase Create(string name)
    {
        var newWorldBuildingPhase = new WorldBuildingPhase();

        newWorldBuildingPhase.Name = WorldBuildingPhaseName.Of(name);
        newWorldBuildingPhase.Status = WorldBuildingStatus.Pending();
        newWorldBuildingPhase.AttemptNumber = 1;

        newWorldBuildingPhase.QueueDomainEvent(new WorldBuildingPhaseCreated(){ WorldBuildingPhase = newWorldBuildingPhase });
        
        return newWorldBuildingPhase;
    }
    
    public WorldBuildingPhase SetResultData(object resultData)
    {
        if(resultData.GetType() != Name.ResultDataType)
            throw new ValidationException($"Result Data Type for {resultData} is not of type {Name.ResultDataType}");

        var resultDataString = SerializeResultData(resultData);
        
        ResultData = resultDataString;
        return this;
    }

    private string SerializeResultData(object resultData)
    {
        try
        {
            return JsonSerializer.Serialize(resultData, new JsonSerializerOptions
            {
                IncludeFields = true,
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 128
            });
        }
        catch (Exception e)
        {
            throw new ValidationException($"Failed to serialize result data for {Name.Value}. {e.Message}");
        }
    }
    
    public TDataType GetResultData<TDataType>()
    {
        try
        {
            if(typeof(TDataType) != Name.ResultDataType)
                throw new ValidationException($"Result Data Type for {typeof(TDataType)} is not of type {Name.ResultDataType}");
            
            // return JsonHelpers.DeserializeWithReflection(Name.ResultDataType, ResultData);
            return JsonSerializer.Deserialize<TDataType>(ResultData, new JsonSerializerOptions
            {
                IncludeFields = true
            });
        }
        catch (Exception e)
        {
            throw new ValidationException($"Failed to deserialize result data for {Name.Value}. {e.Message}");
        }
    }
    
    public WorldBuildingPhase SetStartedAt(DateTimeOffset startedAt)
    {
        StartedAt = startedAt;
        return this;
    }
    
    public WorldBuildingPhase SetStepNumber(int stepNumber)
    {
        StepNumber = stepNumber;
        return this;
    }
    
    public WorldBuildingPhase Start()
    {
        Status = WorldBuildingStatus.Processing();
        StartedAt = DateTimeOffset.UtcNow;
        return this;
    }
    
    public WorldBuildingPhase MarkSuccessful()
    {
        Status = WorldBuildingStatus.Successful();
        EndedAt = DateTimeOffset.UtcNow;
        return this;
    }
    
    public WorldBuildingPhase Fail(string errorMessage)
    {
        Status = WorldBuildingStatus.Failed();
        EndedAt = DateTimeOffset.UtcNow;
        ErrorMessage = errorMessage;
        return this;
    }
    
    public WorldBuildingPhase SetComments(string comments)
    {
        Comments = comments;
        return this;
    }
    
    public WorldBuildingPhase SetSpecialRequests(string specialRequests)
    {
        SpecialRequests = specialRequests;
        return this;
    }
    
    public WorldBuildingPhase Reset()
    {
        Status = WorldBuildingStatus.Pending();
        AttemptNumber++;
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected WorldBuildingPhase() { } // For EF + Mocking
}
