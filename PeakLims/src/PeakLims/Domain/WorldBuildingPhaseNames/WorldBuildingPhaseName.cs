namespace PeakLims.Domain.WorldBuildingPhaseNames;

using System.Text.Json;
using Accessions;
using Ardalis.SmartEnum;
using Containers;
using Gaia.Models;
using HealthcareOrganizationContacts;
using HealthcareOrganizations;
using Patients;
using PeakLims.Domain.WorldBuildingPhases;
using PeakLims.Exceptions;
using PeakOrganizations;
using Users;

public sealed class WorldBuildingPhaseName : ValueObject
{
    private NameEnum _name;
    public string Value
    {
        get => _name.Name;
        private set
        {
            if (!NameEnum.TryFromName(value, true, out var parsed))
                throw new ValidationException($"Invalid Name. Please use one of the following: {string.Join(", ", ListNames())}");

            _name = parsed;
        }
    }
    
    public WorldBuildingPhaseName(string value)
    {
        Value = value;
    }

    public static WorldBuildingPhaseName Of(string value) => new WorldBuildingPhaseName(value);
    public static implicit operator string(WorldBuildingPhaseName value) => value.Value;
    public static List<string> ListNames() => NameEnum.List.Select(x => x.Name).ToList();

    public static WorldBuildingPhaseName CreateOrganization() => new WorldBuildingPhaseName(NameEnum.CreateOrganization.Name);
    public static WorldBuildingPhaseName GenerateUsers() => new WorldBuildingPhaseName(NameEnum.GenerateUsers.Name);
    public static WorldBuildingPhaseName GenerateHealthcareOrganizations() => new WorldBuildingPhaseName(NameEnum.GenerateHealthcareOrganizations.Name);
    public static WorldBuildingPhaseName GenerateHealthcareOrganizationContacts() => new WorldBuildingPhaseName(NameEnum.GenerateHealthcareOrganizationContacts.Name);
    public static WorldBuildingPhaseName GeneratePanelsAndTests() => new WorldBuildingPhaseName(NameEnum.GeneratePanelsAndTests.Name);
    public static WorldBuildingPhaseName AddDefaultContainers() => new WorldBuildingPhaseName(NameEnum.AddDefaultContainers.Name);
    public static WorldBuildingPhaseName GeneratePatients() => new WorldBuildingPhaseName(NameEnum.GeneratePatients.Name);
    public static WorldBuildingPhaseName GenerateAccessions() => new WorldBuildingPhaseName(NameEnum.GenerateAccessions.Name);
    public static WorldBuildingPhaseName FinalizeInDatabase() => new WorldBuildingPhaseName(NameEnum.FinalizeInDatabase.Name);
    

    public WorldBuildingPhase CreateInitialPhaseState() => _name.CreateInitialPhaseState();
    public string DisplayName => _name.DisplayName;
    public Type? ResultDataType => _name.ResultDataType;
    

    private WorldBuildingPhaseName() { } // EF Core

    private abstract class NameEnum(string name, int value)
        : SmartEnum<NameEnum>(name, value)
    {
        public static readonly NameEnum CreateOrganization = new CreateOrganizationType();
        public static readonly NameEnum GenerateUsers = new GenerateUsersType();
        public static readonly NameEnum GenerateHealthcareOrganizations = new GenerateHealthcareOrganizationsType();
        public static readonly NameEnum GenerateHealthcareOrganizationContacts = new GenerateHealthcareOrganizationContactsType();
        public static readonly NameEnum GeneratePanelsAndTests = new GeneratePanelsAndTestsType();
        public static readonly NameEnum AddDefaultContainers = new AddDefaultContainersType();
        public static readonly NameEnum GeneratePatients = new GeneratePatientsType();
        public static readonly NameEnum GenerateAccessions = new GenerateAccessionsType();
        public static readonly NameEnum FinalizeInDatabase = new FinalizeInDatabaseType();

        public abstract WorldBuildingPhase CreateInitialPhaseState();
        public abstract string DisplayName { get; }
        public abstract Type? ResultDataType { get; }
        
        private class CreateOrganizationType() : NameEnum("Create Organization", 0)
        {
            public override WorldBuildingPhase CreateInitialPhaseState() 
                => WorldBuildingPhase.Create(Name);
            public override string DisplayName 
                => "Create Organization";
            public override Type? ResultDataType => typeof(PeakOrganization);
        }

        private class GenerateUsersType() : NameEnum("Generate Users", 1)
        {
            public override WorldBuildingPhase CreateInitialPhaseState() 
                => WorldBuildingPhase.Create(Name);
            public override string DisplayName 
                => "Generate Users";
            public override Type? ResultDataType => typeof(List<User>);
        }

        private class GenerateHealthcareOrganizationsType() : NameEnum("Generate Healthcare Organizations", 2)
        {
            public override WorldBuildingPhase CreateInitialPhaseState() 
                => WorldBuildingPhase.Create(Name);
            public override string DisplayName 
                => "Generate Healthcare Organizations";
            public override Type? ResultDataType => typeof(List<HealthcareOrganization>);
        }

        private class GenerateHealthcareOrganizationContactsType() : NameEnum("Generate Healthcare Organization Contacts", 3)
        {
            public override WorldBuildingPhase CreateInitialPhaseState() 
                => WorldBuildingPhase.Create(Name);
            public override string DisplayName 
                => "Generate Healthcare Organization Contacts";
            
            // orgs w/ contacts
            public override Type? ResultDataType => typeof(List<HealthcareOrganization>);
        }

        private class GeneratePanelsAndTestsType() : NameEnum("Generate Panels and Tests", 4)
        {
            public override WorldBuildingPhase CreateInitialPhaseState() 
                => WorldBuildingPhase.Create(Name);
            public override string DisplayName 
                => "Generate Panels and Tests";
            public override Type? ResultDataType => typeof(PanelTestResponse);
        }

        private class AddDefaultContainersType() : NameEnum("Add Default Containers", 5)
        {
            public override WorldBuildingPhase CreateInitialPhaseState() 
                => WorldBuildingPhase.Create(Name);
            public override string DisplayName 
                => "Add Default Containers";
            public override Type? ResultDataType => typeof(List<Container>);
        }

        private class GeneratePatientsType() : NameEnum("Generate Patients", 6)
        {
            public override WorldBuildingPhase CreateInitialPhaseState() 
                => WorldBuildingPhase.Create(Name);
            public override string DisplayName 
                => "Generate Patients";
            public override Type? ResultDataType => typeof(List<Patient>);
        }

        private class GenerateAccessionsType() : NameEnum("Generate Accessions", 7)
        {
            public override WorldBuildingPhase CreateInitialPhaseState() 
                => WorldBuildingPhase.Create(Name);
            public override string DisplayName 
                => "Generate Accessions";
            public override Type? ResultDataType => typeof(List<Accession>);
        }

        private class FinalizeInDatabaseType() : NameEnum("Finalize in Database", 8)
        {
            public override WorldBuildingPhase CreateInitialPhaseState() 
                => WorldBuildingPhase.Create(Name);
            public override string DisplayName 
                => "Finalize in Database";
            public override Type? ResultDataType => null;
        }
    }
}