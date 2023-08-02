namespace PeakLims.SharedTestHelpers.Fakes.Container;

using PeakLims.Domain.Containers;
using PeakLims.Domain.Containers.Models;

public class FakeContainerBuilder
{
    private ContainerForCreation _creationData = new FakeContainerForCreation().Generate();

    public FakeContainerBuilder WithModel(ContainerForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakeContainerBuilder WithUsedFor(string usedFor)
    {
        _creationData.UsedFor = usedFor;
        return this;
    }
    
    public FakeContainerBuilder WithType(string type)
    {
        _creationData.Type = type;
        return this;
    }
    
    public Container Build()
    {
        var result = Container.Create(_creationData);
        return result;
    }
}