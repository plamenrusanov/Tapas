namespace Tapas.Services.Messaging
{
    using System;
    using System.Threading.Tasks;

    using Twilio;
    using Twilio.Rest.Api.V2010.Account;

    public class TwilioSmsSenderService : ITwilioSmsSenderService
    {
        public async Task SendSms()
        {
            // Find your Account Sid and Token at twilio.com/console
            const string accountSid = "ACf17a92ab091ad97b1065091430edf62f";
            const string authToken = "a178a1f56cb474af73097c4cca584ff3";

            TwilioClient.Init(accountSid, authToken);

            var message = await MessageResource.CreateAsync(
                body: "This is the ship that made the Kessel Run in fourteen parsecs?",
                from: new Twilio.Types.PhoneNumber("+359886562257"),
                to: new Twilio.Types.PhoneNumber("+359890321780"));

            Console.WriteLine(message.Sid);
        }
    }
}
