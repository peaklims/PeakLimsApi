namespace PeakLims.Domain;

using System.Reflection;

public static class Permissions
{
    // Permissions marker - do not delete this comment
    public const string CanDeleteHealthcareOrganizationContacts = nameof(CanDeleteHealthcareOrganizationContacts);
    public const string CanUpdateHealthcareOrganizationContacts = nameof(CanUpdateHealthcareOrganizationContacts);
    public const string CanAddHealthcareOrganizationContacts = nameof(CanAddHealthcareOrganizationContacts);
    public const string CanReadHealthcareOrganizationContacts = nameof(CanReadHealthcareOrganizationContacts);
    public const string CanDeleteHealthcareOrganizations = nameof(CanDeleteHealthcareOrganizations);
    public const string CanUpdateHealthcareOrganizations = nameof(CanUpdateHealthcareOrganizations);
    public const string CanAddHealthcareOrganizations = nameof(CanAddHealthcareOrganizations);
    public const string CanReadHealthcareOrganizations = nameof(CanReadHealthcareOrganizations);
    public const string CanActivateHealthcareOrganizations = nameof(CanActivateHealthcareOrganizations);
    public const string CanDeactivateHealthcareOrganizations = nameof(CanDeactivateHealthcareOrganizations);
    public const string CanDeleteTests = nameof(CanDeleteTests);
    public const string CanUpdateTests = nameof(CanUpdateTests);
    public const string CanAddTests = nameof(CanAddTests);
    public const string CanReadTests = nameof(CanReadTests);
    public const string CanDeletePanels = nameof(CanDeletePanels);
    public const string CanUpdatePanels = nameof(CanUpdatePanels);
    public const string CanAddPanels = nameof(CanAddPanels);
    public const string CanReadPanels = nameof(CanReadPanels);
    public const string CanActivatePanels = nameof(CanActivatePanels);
    public const string CanDeactivatePanels = nameof(CanDeactivatePanels);
    public const string CanCancelTestOrders = nameof(CanCancelTestOrders);
    public const string CanRemoveTestFromPanel = nameof(CanRemoveTestFromPanel);
    public const string CanAddTestToPanel = nameof(CanAddTestToPanel);
    public const string CanAddTestToAccessions = nameof(CanAddTestToAccessions);
    public const string CanAddTestOrders = nameof(CanAddTestOrders);
    public const string CanReadTestOrders = nameof(CanReadTestOrders);
    public const string CanDeleteContainers = nameof(CanDeleteContainers);
    public const string CanUpdateContainers = nameof(CanUpdateContainers);
    public const string CanAddContainers = nameof(CanAddContainers);
    public const string CanReadContainers = nameof(CanReadContainers);
    public const string CanDeleteSamples = nameof(CanDeleteSamples);
    public const string CanUpdateSamples = nameof(CanUpdateSamples);
    public const string CanAddSamples = nameof(CanAddSamples);
    public const string CanReadSamples = nameof(CanReadSamples);
    public const string CanDeleteAccessionComments = nameof(CanDeleteAccessionComments);
    public const string CanUpdateAccessionComments = nameof(CanUpdateAccessionComments);
    public const string CanAddAccessionComments = nameof(CanAddAccessionComments);
    public const string CanReadAccessionComments = nameof(CanReadAccessionComments);
    public const string CanDeleteAccessions = nameof(CanDeleteAccessions);
    public const string CanUpdateAccessions = nameof(CanUpdateAccessions);
    public const string CanAddAccessions = nameof(CanAddAccessions);
    public const string CanReadAccessions = nameof(CanReadAccessions);
    public const string CanDeletePatients = nameof(CanDeletePatients);
    public const string CanUpdatePatients = nameof(CanUpdatePatients);
    public const string CanAddPatients = nameof(CanAddPatients);
    public const string CanReadPatients = nameof(CanReadPatients);
    public const string CanDeleteUsers = nameof(CanDeleteUsers);
    public const string CanUpdateUsers = nameof(CanUpdateUsers);
    public const string CanAddUsers = nameof(CanAddUsers);
    public const string CanReadUsers = nameof(CanReadUsers);
    public const string CanDeleteRolePermissions = nameof(CanDeleteRolePermissions);
    public const string CanUpdateRolePermissions = nameof(CanUpdateRolePermissions);
    public const string CanAddRolePermissions = nameof(CanAddRolePermissions);
    public const string CanReadRolePermissions = nameof(CanReadRolePermissions);
    public const string CanRemoveUserRoles = nameof(CanRemoveUserRoles);
    public const string CanAddUserRoles = nameof(CanAddUserRoles);
    public const string CanGetRoles = nameof(CanGetRoles);
    public const string CanGetPermissions = nameof(CanGetPermissions);
    public const string CanSetSampleOnTestOrder = nameof(CanSetSampleOnTestOrder);
    public const string CanRemoveSampleOnTestOrder = nameof(CanRemoveSampleOnTestOrder);
    public const string CanActivateTests = nameof(CanActivateTests);
    public const string CanDeactivateTests = nameof(CanDeactivateTests);
    public const string CanAddPanelToAccession = nameof(CanAddPanelToAccession);
    public const string CanSetAccessionStatusToReadyForTesting = nameof(CanSetAccessionStatusToReadyForTesting);
    public const string CanDeleteTestOrders = nameof(CanDeleteTestOrders);
    public const string CanRemovePanelOrders = nameof(CanRemovePanelOrders);
    
    public static List<string> List()
    {
        return typeof(Permissions)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .Select(x => (string)x.GetRawConstantValue())
            .ToList();
    }
}
