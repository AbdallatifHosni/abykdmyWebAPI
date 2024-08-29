namespace AbyKhedma.Core.Common
{
    public static class Constants
    {
        public const string AdminRole = "admin";
        public const string RequesterRole = "requester";
        public const string EmployeeRole = "employee";
        public const string SystemRole = "system";
        public const int SystemUserId = 1;
        public const int PaginationPageSize = 12;
        public const string FlowStepStatusPending = "pending";
        public const string FlowStepStatusCompleted = "completed";
        public const int LockedRequestTimeInMin = 20;
        public const int PassCodeExpiryInSeconds = 60;
        public const int Subscription3 = 91;
        public const int Subscription6 = 182;
        public const int Subscription12 = 364;
        public static List<string> GetSystemRoles() { return new List<string>() { AdminRole, EmployeeRole, SystemRole }; } 
        public static List<int> GetInCompletedRequestStatusList() { return new List<int>() { (int)RequestStatus.TamEldaf3WaGaryEltanfeez,
        (int)RequestStatus.FeEntizarEldaf3,
        (int)RequestStatus.TamEldaf3WaGaryEltanfeez,
        (int)RequestStatus.CreateNewRequest
        }; }


        }
        public enum Subscriptions
    {
        S3Months = 1,
        S6Months = 2,
        S12Months = 3
    }
    public enum StatementTypes
    {
        OpeningStatement = 1,
        ClosingingStatement = 2
    }
    public enum RequirementTypes
    {
        Chat = 1,
        Choice = 2,
        HyperLink = 3
    }
    public enum RequestStatus
    {
        InProgress = 1,
        TamElelghaa = 2,
        FeEntizarEldaf3 = 3,
        CreateNewRequest = 4,
        TamEldaf3WaGaryEltanfeez = 5,
        TamEltanfeez = 6
    }
    public enum RequestFlowStatus
    {
        NotCompleted = 1,
        Completed = 2
    }
}
