namespace Tapas.Services.Messaging
{
    using System;
    using System.Threading.Tasks;

    using Twilio;
    using Twilio.Rest.Api.V2010.Account;

    public class TwilioSmsSenderService : ITwilioSmsSenderService
    {
        private readonly string accountSid;
        private readonly string authToken;

        public TwilioSmsSenderService(string accountSid, string authToken)
        {
            this.accountSid = accountSid;
            this.authToken = authToken;
        }

        public async Task SendSms()
        {
            TwilioClient.Init(this.accountSid, this.authToken);

            var message = await MessageResource.CreateAsync(
                body: "This is the ship that made the Kessel Run in fourteen parsecs?",
                from: new Twilio.Types.PhoneNumber("+359886562257"),
                to: new Twilio.Types.PhoneNumber("+359890321780"));

            Console.WriteLine(message.Sid);
        }
    }
}
