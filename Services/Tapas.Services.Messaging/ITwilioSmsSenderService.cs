namespace Tapas.Services.Messaging
{
    using System.Threading.Tasks;

    public interface ITwilioSmsSenderService
    {
        Task SendSms(string userPhone, int code);
    }
}
