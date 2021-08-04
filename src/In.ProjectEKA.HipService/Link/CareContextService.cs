using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.Common.Model;
using In.ProjectEKA.HipService.Link.Model;
using In.ProjectEKA.HipService.UserAuth;
using In.ProjectEKA.HipService.UserAuth.Model;
using HiType = In.ProjectEKA.HipService.Common.Model.HiType;

namespace In.ProjectEKA.HipService.Link
{
    using static Constants;

    public class CareContextService : ICareContextService
    {
        private readonly HttpClient httpClient;
        private readonly IUserAuthRepository userAuthRepository;
        private readonly BahmniConfiguration bahmniConfiguration;
        private readonly ILinkPatientRepository linkPatientRepository;
        public CareContextService(HttpClient httpClient, IUserAuthRepository userAuthRepository, BahmniConfiguration bahmniConfiguration, ILinkPatientRepository linkPatientRepository)
        {
            this.httpClient = httpClient;
            this.userAuthRepository = userAuthRepository;
            this.bahmniConfiguration = bahmniConfiguration;
            this.linkPatientRepository = linkPatientRepository;
        }

        public  Tuple<GatewayAddContextsRequestRepresentation, ErrorRepresentation> AddContextsResponse(
            AddContextsRequest addContextsRequest)
        {
            var accessToken = GetAccessToken(addContextsRequest.ReferenceNumber, addContextsRequest).Result;
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

        private async Task<string> GetAccessToken(string patientReferenceNumber, AddContextsRequest addContextsRequest)
        {
            var (healthId, exception) =
                await linkPatientRepository.GetHealthID(addContextsRequest.ReferenceNumber);
            var (accessToken, error) = await userAuthRepository.GetAccessToken(healthId);
            return accessToken;
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

            request.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(addContextRequest),
                Encoding.UTF8, "application/json");
            await httpClient.SendAsync(request).ConfigureAwait(false);
        }

        public bool IsLinkedContext(List<string> careContexts, string context)
        {
            return careContexts.Any(careContext => careContext.Equals(context));
        }
    }
}