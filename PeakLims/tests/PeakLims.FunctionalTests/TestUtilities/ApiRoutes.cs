namespace PeakLims.FunctionalTests.TestUtilities;
public class ApiRoutes
{
    public const string Base = "api";
    public const string Health = Base + "/health";

    // new api route marker - do not delete

    public static class HealthcareOrganizationContacts
    {
        public static string GetList => $"{Base}/healthcareOrganizationContacts";
        public static string GetRecord(Guid id) => $"{Base}/healthcareOrganizationContacts/{id}";
        public static string Delete(Guid id) => $"{Base}/healthcareOrganizationContacts/{id}";
        public static string Put(Guid id) => $"{Base}/healthcareOrganizationContacts/{id}";
        public static string Create => $"{Base}/healthcareOrganizationContacts";
        public static string CreateBatch => $"{Base}/healthcareOrganizationContacts/batch";
    }

    public static class HealthcareOrganizations
    {
        public static string GetList => $"{Base}/healthcareOrganizations";
        public static string GetRecord(Guid id) => $"{Base}/healthcareOrganizations/{id}";
        public static string Delete(Guid id) => $"{Base}/healthcareOrganizations/{id}";
        public static string Put(Guid id) => $"{Base}/healthcareOrganizations/{id}";
        public static string Create => $"{Base}/healthcareOrganizations";
        public static string CreateBatch => $"{Base}/healthcareOrganizations/batch";
    }

    public static class Tests
    {
        public static string GetList => $"{Base}/tests";
        public static string GetRecord(Guid id) => $"{Base}/tests/{id}";
        public static string Delete(Guid id) => $"{Base}/tests/{id}";
        public static string Put(Guid id) => $"{Base}/tests/{id}";
        public static string Create => $"{Base}/tests";
        public static string CreateBatch => $"{Base}/tests/batch";
    }

    public static class Panels
    {
        public static string GetList => $"{Base}/panels";
        public static string GetRecord(Guid id) => $"{Base}/panels/{id}";
        public static string Delete(Guid id) => $"{Base}/panels/{id}";
        public static string Put(Guid id) => $"{Base}/panels/{id}";
        public static string Create => $"{Base}/panels";
        public static string CreateBatch => $"{Base}/panels/batch";
    }

    public static class TestOrders
    {
        public static string GetList => $"{Base}/testOrders";
        public static string GetRecord(Guid id) => $"{Base}/testOrders/{id}";
        public static string Delete(Guid id) => $"{Base}/testOrders/{id}";
        public static string Put(Guid id) => $"{Base}/testOrders/{id}";
        public static string Create => $"{Base}/testOrders";
        public static string CreateBatch => $"{Base}/testOrders/batch";
    }

    public static class Containers
    {
        public static string GetList => $"{Base}/containers";
        public static string GetRecord(Guid id) => $"{Base}/containers/{id}";
        public static string Delete(Guid id) => $"{Base}/containers/{id}";
        public static string Put(Guid id) => $"{Base}/containers/{id}";
        public static string Create => $"{Base}/containers";
        public static string CreateBatch => $"{Base}/containers/batch";
    }

    public static class Samples
    {
        public static string GetList => $"{Base}/samples";
        public static string GetRecord(Guid id) => $"{Base}/samples/{id}";
        public static string Delete(Guid id) => $"{Base}/samples/{id}";
        public static string Put(Guid id) => $"{Base}/samples/{id}";
        public static string Create => $"{Base}/samples";
        public static string CreateBatch => $"{Base}/samples/batch";
    }

    public static class AccessionComments
    {
        public static string GetList => $"{Base}/accessionComments";
        public static string GetRecord(Guid id) => $"{Base}/accessionComments/{id}";
        public static string Delete(Guid id) => $"{Base}/accessionComments/{id}";
        public static string Put(Guid id) => $"{Base}/accessionComments/{id}";
        public static string Create => $"{Base}/accessionComments";
        public static string CreateBatch => $"{Base}/accessionComments/batch";
    }

    public static class Accessions
    {
        public static string GetList => $"{Base}/accessions";
        public static string GetRecord(Guid id) => $"{Base}/accessions/{id}";
        public static string Delete(Guid id) => $"{Base}/accessions/{id}";
        public static string Put(Guid id) => $"{Base}/accessions/{id}";
        public static string Create => $"{Base}/accessions";
        public static string CreateBatch => $"{Base}/accessions/batch";
    }

    public static class Patients
    {
        public static string GetList => $"{Base}/patients";
        public static string GetRecord(Guid id) => $"{Base}/patients/{id}";
        public static string Delete(Guid id) => $"{Base}/patients/{id}";
        public static string Put(Guid id) => $"{Base}/patients/{id}";
        public static string Create => $"{Base}/patients";
        public static string CreateBatch => $"{Base}/patients/batch";
    }

    public static class Users
    {
        public static string GetList => $"{Base}/users";
        public static string GetRecord(Guid id) => $"{Base}/users/{id}";
        public static string Delete(Guid id) => $"{Base}/users/{id}";
        public static string Put(Guid id) => $"{Base}/users/{id}";
        public static string Create => $"{Base}/users";
        public static string CreateBatch => $"{Base}/users/batch";
        public static string AddRole(Guid id) => $"{Base}/users/{id}/addRole";
        public static string RemoveRole(Guid id) => $"{Base}/users/{id}/removeRole";
    }

    public static class RolePermissions
    {
        public static string GetList => $"{Base}/rolePermissions";
        public static string GetRecord(Guid id) => $"{Base}/rolePermissions/{id}";
        public static string Delete(Guid id) => $"{Base}/rolePermissions/{id}";
        public static string Put(Guid id) => $"{Base}/rolePermissions/{id}";
        public static string Create => $"{Base}/rolePermissions";
        public static string CreateBatch => $"{Base}/rolePermissions/batch";
    }
}
