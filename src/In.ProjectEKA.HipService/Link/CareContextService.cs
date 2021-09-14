using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Common.Model;
using In.ProjectEKA.HipService.Link.Model;
using In.ProjectEKA.HipService.UserAuth;
using In.ProjectEKA.HipService.UserAuth.Model;
using Newtonsoft.Json;
using Optional.Unsafe;
using HiType = In.ProjectEKA.HipService.Common.Model.HiType;
using Identifier = In.ProjectEKA.HipService.UserAuth.Model.Identifier;

namespace In.ProjectEKA.HipService.Link
{
    using static Constants;

    public class CareContextService : ICareContextService
    {
        private readonly HttpClient httpClient;
        private readonly IUserAuthRepository userAuthRepository;
        private readonly BahmniConfiguration bahmniConfiguration;
        private readonly ILinkPatientRepository linkPatientRepository;

        public CareContextService(HttpClient httpClient, IUserAuthRepository userAuthRepository,
            BahmniConfiguration bahmniConfiguration, ILinkPatientRepository linkPatientRepository)
        {
            this.httpClient = httpClient;
            this.userAuthRepository = userAuthRepository;
            this.bahmniConfiguration = bahmniConfiguration;
            this.linkPatientRepository = linkPatientRepository;
        }

        public Tuple<GatewayAddContextsRequestRepresentation, ErrorRepresentation> AddContextsResponse(
            AddContextsRequest addContextsRequest)
        {
            var accessToken = GetAccessToken(addContextsRequest.ReferenceNumber).Result;
            var referenceNumber = addContextsRequest.ReferenceNumber;
            var careContexts = addContextsRequest.CareContexts;
            var display = addContextsRequest.Display;
            var patient = new AddCareContextsPatient(referenceNumber, display, careContexts);
            var link = new AddCareContextsLink(accessToken, patient);
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requestId = Guid.NewGuid();
            return new Tuple<GatewayAddContextsRequestRepresentation, ErrorRepresentation>
                (new GatewayAddContextsRequestRepresentation(requestId, timeStamp, link), null);
        }

        private bool IsExpired(string accessToken)
        {
            var token = new JwtSecurityTokenHandler().ReadToken(accessToken) as JwtSecurityToken;
            var expInUnixTimeStamp = token?.Claims.First(c => c.Type == "exp").Value;
            var exp = DateTimeOffset
                .FromUnixTimeSeconds(long.Parse(expInUnixTimeStamp ?? throw new InvalidOperationException()))
                .LocalDateTime;
            return DateTime.Compare(exp, DateTime.Now.ToLocalTime()) < 0;
        }

        public async Task SetAccessToken(string patientReferenceNumber)
        {
            var (healthId, exception) =
                await linkPatientRepository.GetHealthID(patientReferenceNumber);
            if (!UserAuthMap.HealthIdToAccessToken.TryGetValue(healthId, out _))
            {
                var (accessToken, error) = await userAuthRepository.GetAccessToken(healthId);
                UserAuthMap.HealthIdToAccessToken.Add(healthId, accessToken);
            }

            if (IsExpired(UserAuthMap.HealthIdToAccessToken[healthId]))
            {
                await CallAuthInit(healthId);
                await CallAuthConfirm(healthId);
            }
        }

        private async Task<string> GetAccessToken(string patientReferenceNumber)
        {
            var (healthId, exception) =
                await linkPatientRepository.GetHealthID(patientReferenceNumber);
            return UserAuthMap.HealthIdToAccessToken[healthId];
        }

        private async Task CallAuthConfirm(string healthId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, HIP_URL + PATH_HIP_AUTH_CONFIRM);
                var ndhmDemographics = (userAuthRepository.GetDemographics(healthId).Result).ValueOrDefault();
                var identifier = new Identifier(MOBILE, ndhmDemographics.PhoneNumber);
                var demographics = new Demographics(ndhmDemographics.Name, ndhmDemographics.Gender,
                    ndhmDemographics.DateOfBirth, identifier);
                var authConfirmRequest = new AuthConfirmRequest(null, healthId, demographics);
                request.Content = new StringContent(JsonConvert.SerializeObject(authConfirmRequest),
                    Encoding.UTF8, "application/json");
                await httpClient.SendAsync(request).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async Task CallAuthInit(string healthId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, HIP_URL + PATH_HIP_AUTH_INIT);
                var authInitRequest = new AuthInitRequest(healthId, "DEMOGRAPHICS", "KYC_AND_LINK");
                request.Content = new StringContent(JsonConvert.SerializeObject(authInitRequest), Encoding.UTF8,
                    "application/json");
                await httpClient.SendAsync(request).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public Tuple<GatewayNotificationContextRepresentation, ErrorRepresentation> NotificationContextResponse(
            NotifyContextRequest notifyContextRequest)
        {
            var id = notifyContextRequest.PatientId;
            var patientReference = notifyContextRequest.PatientReference;
            var careContextReference = notifyContextRequest.CareContextReference;
            var hiTypes = notifyContextRequest.HiTypes;
            var hipId = notifyContextRequest.HipId;
            var patient = new NotificationPatientContext(id);
            var careContext = new NotificationCareContext(patientReference, careContextReference);
            var hip = new NotificationContextHip(hipId);
            var date = notifyContextRequest.Date;
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requestId = Guid.NewGuid();
            var notification = new NotificationContext(patient, careContext, hiTypes, date, hip);
            return new Tuple<GatewayNotificationContextRepresentation, ErrorRepresentation>
                (new GatewayNotificationContextRepresentation(requestId, timeStamp, notification), null);
        }

        public async Task CallNotifyContext(NewContextRequest newContextRequest, CareContextRepresentation context)
        {
            var request =
                new HttpRequestMessage(HttpMethod.Get, HIP_URL + PATH_NOTIFY_CONTEXTS);
            var notifyContext = new NotifyContextRequest(newContextRequest.HealthId,
                newContextRequest.PatientReferenceNumber,
                context.ReferenceNumber,
                Enum.GetValues(typeof(HiType))
                    .Cast<HiType>()
                    .Select(v => v.ToString())
                    .ToList(),
                DateTime.Now,
                bahmniConfiguration.Id
            );
            request.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(notifyContext),
                Encoding.UTF8, "application/json");

            await httpClient.SendAsync(request).ConfigureAwait(false);
        }

        public async Task CallAddContext(NewContextRequest newContextRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, HIP_URL + PATH_ADD_CONTEXTS);
            var (accessToken, error) = await userAuthRepository.GetAccessToken(newContextRequest.HealthId);
            var addContextRequest = new AddContextsRequest(
                accessToken,
                newContextRequest.PatientReferenceNumber,
                newContextRequest.CareContexts,
                newContextRequest.PatientName);

            request.Content = new StringContent(JsonConvert.SerializeObject(addContextRequest),
                Encoding.UTF8, "application/json");
            await httpClient.SendAsync(request).ConfigureAwait(false);
        }

        public bool IsLinkedContext(List<string> careContexts, string context)
        {
            return careContexts.Any(careContext => careContext.Equals(context));
        }
    }
}