using Core;

namespace Sms.Fake;

public  class BaseInfo
{
    public static string Name = "fake";
    public static string Type = "sms";
    public static string EventName = NotificationSendEvent.EventName + "_" + Name;
}