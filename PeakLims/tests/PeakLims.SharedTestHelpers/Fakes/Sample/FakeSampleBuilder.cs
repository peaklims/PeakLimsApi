namespace PeakLims.SharedTestHelpers.Fakes.Sample;

using Domain.Containers;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.Models;

public class FakeSampleBuilder
{
    private SampleForCreation _creationData = new FakeSampleForCreation().Generate();
    private Container _container = null;

    public FakeSampleBuilder WithModel(SampleForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakeSampleBuilder WithStatus(string status)
    {
        _creationData.Status = status;
        return this;
    }
    
    public FakeSampleBuilder WithType(string type)
    {
        _creationData.Type = type;
        return this;
    }

    public FakeSampleBuilder WithValidTypeForContainer(Container container)
    {
        _creationData.Type = container.UsedFor.Value;
        return this;
    }
    
    public FakeSampleBuilder WithQuantity(decimal? quantity)
    {
        _creationData.Quantity = quantity;
        return this;
    }
    
    public FakeSampleBuilder WithCollectionDate(DateOnly? collectionDate)
    {
        _creationData.CollectionDate = collectionDate;
        return this;
    }
    
    public FakeSampleBuilder WithReceivedDate(DateOnly? receivedDate)
    {
        _creationData.ReceivedDate = receivedDate;
        return this;
    }
    
    public FakeSampleBuilder WithCollectionSite(string collectionSite)
    {
        _creationData.CollectionSite = collectionSite;
        return this;
    }
    
    public FakeSampleBuilder WithValidContainer(Container container)
    {
        _container = container;
        _creationData.Type = container.UsedFor.Value;
        return this;
    }
    
    public Sample Build()
    {
        var result = Sample.Create(_creationData);
        if (_container != null)
            result.SetContainer(_container);

        return result;
    }
}