using System;
using System.Net;
using System.Net.Http;
using System.Web;
using In.ProjectEKA.HipService.Common;
using In.ProjectEKA.HipService.DataFlow;
using In.ProjectEKA.HipService.OpenMrs;
using In.ProjectEKA.HipServiceTest.DataFlow.Builder;
using Moq;

namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using FluentAssertions;
    using Xunit;

    [Collection("Collect Tests")]
    public class OpenMrsPatientDataTest
    {
        private readonly Mock<IOpenMrsClient> openMrsClient = new Mock<IOpenMrsClient>();

        private readonly OpenMrsPatientData openMrsPatientData =
            new OpenMrsPatientData(new Mock<IOpenMrsClient>().Object);

        [Fact]
        private async void ShouldNotReturnDataForPrescription()
        {
            var patientUuid = TestBuilder.Faker().Random.String();
            var grantedContext = TestBuilder.Faker().Random.String();
            var toDate = TestBuilder.Faker().Random.String();
            var fromDate = TestBuilder.Faker().Random.String();
            var hiType = TestBuilder.Faker().Random.String();
            var response = openMrsPatientData.GetPatientData(patientUuid,grantedContext,toDate,fromDate,hiType);
            response.Result.Should().BeEquivalentTo("");
        }
        
        [Fact]
        private async void ShouldReturnDataForPrescription()
        {
            var openmrsClientMock = new Mock<IOpenMrsClient>();
            var openMrsPatientData = new OpenMrsPatientData(openmrsClientMock.Object);
            var patientUuid = "acsa";
            var grantedContext = "OPD";
            var fromDate = "2020-01-01";
            var toDate = "2020-12-12";
            var hiType = "prescription";
            var pathForPrescription = $"{Constants.OPENMRS_PRESCRIPTION}";
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (
                !string.IsNullOrEmpty(patientUuid) &&
                !string.IsNullOrEmpty(grantedContext) &&
                !string.IsNullOrEmpty(toDate) &&
                !string.IsNullOrEmpty(fromDate)
            )
            {
                query["patientId"] = patientUuid;
                query["visitType"] = grantedContext;
                query["fromDate"] = fromDate;
                query["toDate"] = DateTime.Parse(toDate).AddDays(2).ToString("yyyy-MM-dd");
            }
            if (query.ToString() != "")
            {
                pathForPrescription = $"{pathForPrescription}/?{query}";
            }
            openmrsClientMock
                .Setup(x => x.GetAsync(pathForPrescription))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"prescriptions\": [{\"careContext\":{},\"bundle\":{\"resourceType\": \"Bundle\"}}]}")
                })
                .Verifiable();
            var response = openMrsPatientData.GetPatientData(patientUuid,grantedContext,toDate,fromDate,hiType);
            response.Result.Should().BeEquivalentTo("{\"resourceType\": \"Bundle\"}");
        }
    }
}