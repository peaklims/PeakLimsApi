namespace PeakLims.Resources;

using System.Reflection;

public static class Consts
{
    public static class Testing
    {
        public const string IntegrationTestingEnvName = "LocalIntegrationTesting";
        public const string FunctionalTestingEnvName = "LocalFunctionalTesting";
    }

    public static class DatabaseSequences
    {
        public const string PatientInternalIdPrefix = "PAT";
        public const string AccessionNumberPrefix = "ACC";
        public const string SampleNumberPrefix = "SAM";
    }

    public static class DefaultTurnAroundTimes
    {
        public const int NormalTat = 14;
        public const int StatTat = 5;
    }

    public static class HttpClients
    {
        public const string KeycloakAdmin = "KeycloakAdmin";
    }

    public const string SuperHangfireUser = "job-user-346f9812-16da-4a72-9db2-f066661d6593";
    
    public static class HangfireQueues
    {
        // public const string MyFirstQueue = "my-first-queue";
        
        public static string[] List()
        {
            return typeof(HangfireQueues)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(x => (string)x.GetRawConstantValue())
                .ToArray();
        }
    }

    public static class S3Buckets
    {
        public const string AccessionAttachments = "accession-attachments";
        
        public static string[] List()
        {
            return typeof(S3Buckets)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(x => (string)x.GetRawConstantValue())
                .ToArray();
        }
    }
}