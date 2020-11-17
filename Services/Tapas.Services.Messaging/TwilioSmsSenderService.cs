namespace Tapas.Services.Messaging
{
    using System;
    using System.Threading.Tasks;

    using Twilio;

    // using Twilio.AspNet.Mvc;
    using Twilio.Exceptions;
    using Twilio.Rest.Api.V2010.Account;
    using Twilio.TwiML;
    using Twilio.Types;

    public class TwilioSmsSenderService : /*TwilioController,*/ ITwilioSmsSenderService
    {
        private readonly string accountSid;
        private readonly string authToken;
        private readonly string twilioNumber;

        public TwilioSmsSenderService(string accountSid, string authToken, string twilioNumber)
        {
            this.accountSid = accountSid;
            this.authToken = authToken;
            this.twilioNumber = twilioNumber;
        }

        public async Task SendSms(string userPhone, int code)
        {
            TwilioClient.Init(this.accountSid, this.authToken);
            try
            {
                var message = await MessageResource.CreateAsync(
                    body: $"Tapas sent you code: {code}",
                    from: new Twilio.Types.PhoneNumber(this.twilioNumber),
                    to: new Twilio.Types.PhoneNumber(userPhone));
                Console.WriteLine(message.Sid);
            }
            catch (ApiException e)
            {
                if (e.Code == 21614)
                {
                    Console.WriteLine("Uh oh, looks like this caller can't receive SMS messages.");
                }
            }
        }

        public async Task VoiseCall(string userPhone)
        {
            var response = new VoiceResponse();
            response.Say("Thanks for calling! We just sent you a text with a clue.", voice: "Alice");

            // this.TwiML(response);
        }
    }
}
