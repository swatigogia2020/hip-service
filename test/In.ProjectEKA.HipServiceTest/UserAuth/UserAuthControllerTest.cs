using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using Hl7.Fhir.Model;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Gateway;
using In.ProjectEKA.HipService.Link.Model;
using In.ProjectEKA.HipService.UserAuth;
using In.ProjectEKA.HipService.UserAuth.Model;
using In.ProjectEKA.HipServiceTest.Common.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Xunit;
using static In.ProjectEKA.HipService.Common.Constants;
using Meta = In.ProjectEKA.HipService.Link.Model.Meta;
using Task = System.Threading.Tasks.Task;

namespace In.ProjectEKA.HipServiceTest.UserAuth
{
    public class UserAuthControllerTest
    {
        private readonly UserAuthController userAuthController;

        private readonly Mock<ILogger<UserAuthController>> logger =
            new Mock<ILogger<UserAuthController>>();

        private readonly Mock<GatewayClient> gatewayClient = new Mock<GatewayClient>(MockBehavior.Strict, null, null);
        private readonly Mock<IUserAuthService> userAuthService = new Mock<IUserAuthService>();

        private readonly GatewayConfiguration gatewayConfiguration = new GatewayConfiguration
        {
            CmSuffix = "ncg"
        };

        public UserAuthControllerTest()
        {
            userAuthController = new UserAuthController(gatewayClient.Object,
                logger.Object,
                gatewayConfiguration,
                userAuthService.Object);
        }

        [Fact]
        private void ShouldFetchPatientsAuthModes()
        {
            var purpose = "KYC_AND_LINK";
            var request = new FetchRequest("hina_patel@ncg",purpose);
            var requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            var query = new FetchQuery(request.healthId, FETCH_MODE_PURPOSE, requester);
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requestId = Guid.NewGuid();
            var cmSuffix = "ncg";
            var gatewayFetchModesRequestRepresentation =
                new GatewayFetchModesRequestRepresentation(requestId, timeStamp, query, cmSuffix);
            var correlationId = Uuid.Generate().ToString();
            var modes = new List<Mode>
            {
                {Mode.MOBILE_OTP},
                {Mode.AADHAAR_OTP}
            };
            var onFetchAuthModeRequest = new OnFetchAuthModeRequest(requestId, timeStamp,
                new AuthModeFetch("KYC_AND_LINK", modes), null, new Resp(requestId.ToString()));
            var authModes = string.Join(',', onFetchAuthModeRequest.Auth.Modes);
            userAuthService.Setup(a => a.FetchModeResponse(request, gatewayConfiguration))
                .Returns(gatewayFetchModesRequestRepresentation);
            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(PATH_FETCH_AUTH_MODES,
                            gatewayFetchModesRequestRepresentation, cmSuffix, correlationId))
                .Returns(Task.CompletedTask)
                .Callback<string, GatewayFetchModesRequestRepresentation, string, string>
                ((path, gr, suffix, corId)
                    => userAuthController.SetAuthModes(onFetchAuthModeRequest));

            if (userAuthController.GetAuthModes(correlationId, request).Result is OkObjectResult authMode)
            {
                authMode.StatusCode.Should().Be(StatusCodes.Status200OK);
                authMode.Value.Should().BeEquivalentTo(authModes);
            }
        }

        [Fact]
        private void ShouldNotFetchPatientsAuthModes()
        {
            var purpose = "KYC_AND_LINK";
            var request = new FetchRequest("hina_patel@ncg",purpose);
            var requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            var query = new FetchQuery(request.healthId, FETCH_MODE_PURPOSE, requester);
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requestId = Guid.NewGuid();
            var cmSuffix = "ncg";
            var gatewayFetchModesRequestRepresentation =
                new GatewayFetchModesRequestRepresentation(requestId, timeStamp, query, cmSuffix);
            var correlationId = Uuid.Generate().ToString();

            userAuthService.Setup(a => a.FetchModeResponse(request, gatewayConfiguration))
                .Returns(gatewayFetchModesRequestRepresentation);
            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(PATH_FETCH_AUTH_MODES,
                            gatewayFetchModesRequestRepresentation, cmSuffix, correlationId))
                .Returns(Task.FromResult(""));

            if (userAuthController.GetAuthModes(correlationId, request).Result is ObjectResult authMode)
            {
                Log.Information(authMode.ToString());
                authMode.StatusCode.Should().Be((int) HttpStatusCode.GatewayTimeout);
            }
        }

        [Fact]
        private void ShouldSendAuthInitAndOnAuthInit()
        {
            var purpose = "KYC_AND_LINK";
            var request = new AuthInitRequest("hina_patel@ncg", "MOBILE_OTP",purpose);
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            var query = new AuthInitQuery(request.healthId, FETCH_MODE_PURPOSE, request.authMode, requester);
            var requestId = Guid.NewGuid();
            var cmSuffix = "ncg";
            var gatewayAuthInitRequestRepresentation =
                new GatewayAuthInitRequestRepresentation(requestId, timeStamp, query, cmSuffix);
            var correlationId = Uuid.Generate().ToString();
            var transactionId = new Guid().ToString();
            var auth = new Auth(transactionId, new Meta("string", new DateTime()), Mode.MOBILE_OTP);
            var authOnInitRequest =
                new AuthOnInitRequest(requestId, timeStamp, auth, null, new Resp(requestId.ToString()));
            userAuthService.Setup(a => a.AuthInitResponse(request, gatewayConfiguration))
                .Returns(gatewayAuthInitRequestRepresentation);
            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(PATH_AUTH_INIT,
                            gatewayAuthInitRequestRepresentation, cmSuffix, correlationId))
                .Callback<string, GatewayAuthInitRequestRepresentation, string, string>
                ((path, gr, suffix, corId)
                    => userAuthController.SetTransactionId(authOnInitRequest))
                .Returns(Task.FromResult(""));

            if (userAuthController.GetTransactionId(correlationId, request).Result is OkObjectResult tId)
            {
                tId.StatusCode.Should().Be(StatusCodes.Status200OK);
                tId.Value.Should().BeEquivalentTo(transactionId);
            }
        }

        [Fact]
        private void ShouldSendAuthInitAndNotOnAuthInit()
        {
            var request = new AuthInitRequest("123", "12344","KYC_AND_LINK");
            var timeStamp = DateTime.Now.ToUniversalTime();
            var requester = new Requester(gatewayConfiguration.ClientId, FETCH_MODE_REQUEST_TYPE);
            var query = new AuthInitQuery(request.healthId, FETCH_MODE_PURPOSE, request.authMode, requester);
            var requestId = Guid.NewGuid();
            var cmSuffix = "ncg";
            var gatewayAuthInitRequestRepresentation =
                new GatewayAuthInitRequestRepresentation(requestId, timeStamp, query, cmSuffix);
            var correlationId = Uuid.Generate().ToString();

            userAuthService.Setup(a => a.AuthInitResponse(request, gatewayConfiguration))
                .Returns(gatewayAuthInitRequestRepresentation);
            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(PATH_AUTH_INIT,
                            gatewayAuthInitRequestRepresentation, cmSuffix, correlationId))
                .Returns(Task.FromResult(""));

            if (userAuthController.GetTransactionId(correlationId, request).Result is ObjectResult authMode)
            {
                Log.Information(authMode.ToString());
                authMode.StatusCode.Should().Be((int) HttpStatusCode.GatewayTimeout);
            }
        }

        [Fact]
        private void ShouldSendAuthConfirmAndOnAuthConfirm()
        {
            var authConfirmRequest = new AuthConfirmRequest(new Guid().ToString(), "123444");
            AuthConfirmCredential credential = new AuthConfirmCredential(authConfirmRequest.authCode);
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            var transactionId = TestBuilder.Faker().Random.Hash();
            Guid requestId = Guid.NewGuid();
            var requester = new Requester("1000005", "HIP");
            var validity = new Validity("LINK", requester);
            var onAuthConfirm = new OnConfirmAuth("12", validity, new DateTime(), "1");
            var address = new AuthConfirmAddress(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>());
            var identifiers = new Identifiers("MOBILE", "+919800083232");
            var patient = new AuthConfirmPatient("hina_patel@ncg", "Hina Patel", "F", "1998",
                address, identifiers);
            var onAuthConfirmRequest = new OnAuthConfirmRequest(requestId, timeStamp, onAuthConfirm, patient,
                null, new Resp(requestId.ToString()));
            var cmSuffix = "ncg";
            GatewayAuthConfirmRequestRepresentation gatewayAuthConfirmRequestRepresentation =
                new GatewayAuthConfirmRequestRepresentation(requestId, timeStamp, transactionId, credential, cmSuffix);
            var correlationId = Uuid.Generate().ToString();

            userAuthService.Setup(a => a.AuthConfirmResponse(authConfirmRequest))
                .Returns(gatewayAuthConfirmRequestRepresentation);
            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(PATH_AUTH_CONFIRM,
                            gatewayAuthConfirmRequestRepresentation, cmSuffix, correlationId))
                .Returns(Task.FromResult(""))
                .Callback<string, GatewayAuthConfirmRequestRepresentation, string, string>
                ((path, gr, suffix, corId)
                    => userAuthController.SetAccessToken(onAuthConfirmRequest));

            if (userAuthController.GetAccessToken(correlationId, authConfirmRequest).Result is OkObjectResult
                accessToken)
            {
                accessToken.StatusCode.Should().Be(StatusCodes.Status200OK);
                accessToken.Value.Should().BeEquivalentTo("12");
            }
        }

        [Fact]
        private void ShouldSendAuthConfirmAndNotOnAuthConfirm()
        {
            var authConfirmRequest = new AuthConfirmRequest(new Guid().ToString(), "123444");
            AuthConfirmCredential credential = new AuthConfirmCredential(authConfirmRequest.authCode);
            DateTime timeStamp = DateTime.Now.ToUniversalTime();
            var transactionId = TestBuilder.Faker().Random.Hash();
            Guid requestId = Guid.NewGuid();
            var cmSuffix = "ncg";
            GatewayAuthConfirmRequestRepresentation gatewayAuthConfirmRequestRepresentation =
                new GatewayAuthConfirmRequestRepresentation(requestId, timeStamp, transactionId, credential, cmSuffix);
            var correlationId = Uuid.Generate().ToString();

            userAuthService.Setup(a => a.AuthConfirmResponse(authConfirmRequest))
                .Returns(gatewayAuthConfirmRequestRepresentation);
            gatewayClient.Setup(
                    client =>
                        client.SendDataToGateway(PATH_AUTH_CONFIRM,
                            gatewayAuthConfirmRequestRepresentation, cmSuffix, correlationId))
                .Returns(Task.FromResult(""));

            if (userAuthController.GetAccessToken(correlationId, authConfirmRequest).Result is ObjectResult authMode)
            {
                Log.Information(authMode.ToString());
                authMode.StatusCode.Should().Be((int) HttpStatusCode.GatewayTimeout);
            }
        }
    }
}