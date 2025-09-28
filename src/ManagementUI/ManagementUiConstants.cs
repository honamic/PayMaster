using Honamic.PayMaster.Application;

namespace Honamic.Paymaster.ManagementUi;

public static class ManagementUiConstants
{
    public const string ModuleName = PayMasterConstants.ModuleName;

    public class ModuleRoutes
    {
        public const string ReceiptRequests = $"module/{ModuleName}/ReceiptRequests";
        public const string ReceiptRequestsList = $"module/{ModuleName}/ReceiptRequests/list"; 

    }
}