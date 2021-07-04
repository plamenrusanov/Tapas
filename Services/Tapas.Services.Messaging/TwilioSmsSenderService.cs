namespace Tapas.Services.Messaging
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Twilio;

    // using Twilio.AspNet.Mvc;
    using Twilio.Exceptions;
    using Twilio.Rest.Api.V2010.Account;
    using Twilio.TwiML;
    using Twilio.TwiML.Voice;

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

        public async System.Threading.Tasks.Task SendSms(string userPhone, int code)
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

        public VoiceResponse VoiseCall(string userPhone)
        {
            // response.Say("Thanks for calling! We just sent you a text with a clue.", voice: "Alice");
            // TwiML(response);
            var callerId = this.accountSid;

            var response = new VoiceResponse();

            if (!string.IsNullOrEmpty(userPhone))
            {
                var dial = new Dial(callerId: callerId);
                if (Regex.IsMatch(userPhone, "^[\\d\\+\\-\\(\\) ]+$"))
                {
                    dial.Number(userPhone);
                }
                else
                {
                    dial.Client(userPhone);
                }

                response.Append(dial);
            }
            else
            {
                response.Say("Thanks for calling!", voice: Say.VoiceEnum.Alice);
            }

            return response; // Content(response.ToString(), "text/xml");
        }
    }
}
