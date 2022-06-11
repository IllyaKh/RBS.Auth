using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RBS.Auth.Common;
using RBS.Auth.Common.Temp;
using RBS.Auth.Db;
using RBS.Auth.Db.Domain;
using RBS.Auth.Services.Interfaces.Verification;
using RestSharp;

namespace RBS.Auth.Services.Verification
{
    public class VerificationService : IVerificationService
    {
        private const int VerificationCodeLifetime = 1800;

        private readonly AuthContext _db;
        private readonly ServicesOptions _servicesOptions;
        private readonly Random _random;
        private string Code => _random.Next(100000000, 999999999).ToString();

        public VerificationService(AuthContext db, IOptions<ServicesOptions> servicesOptions)
        {
            _db = db;
            _servicesOptions = servicesOptions.Value;
            _random = new Random();
        }

        public async Task<bool> IsCodeValid(Guid userId, string code)
        {
            var dbCode = await _db.UserVerificationCodes
                  .FirstOrDefaultAsync(c => c.UserId == userId &&
                                            c.Code == code &&
                                            c.ExpireDate > DateTime.Now);

            return dbCode != null;
        }

        public async Task<string> GenerateCode(Guid userId)
        {
            var generatedCode = Code;

            await _db.UserVerificationCodes.AddAsync(new UserVerificationCodes()
            {
                UserId = userId,
                ExpireDate = DateTime.Now.AddSeconds(VerificationCodeLifetime),
                Code = generatedCode
            });

            await _db.SaveChangesAsync();

            return generatedCode;
        }

        public async Task VerifyUser(Guid userId)
        {
            var user = await _db.UserCredentials
                .SingleOrDefaultAsync(u => u.Id == userId);

            user.IsVerified = true;

            await _db.SaveChangesAsync();
        }


        // TODO: Refactor, move to another service + templates
        public async Task SendVerifyCode(string email, string code)
        {
            var client = new RestClient(_servicesOptions.EmailSender);
            var request = new RestRequest("api/Sender/SendMessage", Method.Post);


            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Json to post.
            string jsonToSend = JsonConvert.SerializeObject(new EmailModel()
            {
                ToAddresses = new List<string>() { email },
                Subject = "System verification code",
                Content = $"Your verification code is {code}."
            }, serializerSettings);

            request.AddParameter("application/json", jsonToSend, ParameterType.RequestBody);

            try
            {
                var resp = await client.ExecuteAsync(request);
            }
            catch
            {
                throw;
            }
        }

    }
}
