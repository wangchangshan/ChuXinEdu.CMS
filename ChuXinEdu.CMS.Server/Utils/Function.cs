using Microsoft.AspNetCore.Http;

namespace ChuXinEdu.CMS.Server.Utils 
{
    public class Function { }

    public enum ResponseCode 
    {
        Outmode = 1000,
        LoginFailed = 1101,
        AccountLocked = 1103,
        OK = 1200,
        RecordAlreadyExist = 1222,
        AuthenticationError = 1401,
        NoPermission = 1403,
        NoWxRegisterInfo = 1404,
        NoTargetData = 1409,
        InnerError = 1500,
        NoMoreAction = 1600,
        WrongKeyRequest = 1700,
        AlreadyLogin = 1701,
    }
}