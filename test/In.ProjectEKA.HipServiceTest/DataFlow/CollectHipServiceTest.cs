using System.Linq;
using FluentAssertions;
using Hl7.Fhir.Model;
using In.ProjectEKA.HipService.DataFlow;
using Moq;
using Optional.Unsafe;

namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;
    using Xunit;

    [Collection("Collect Tests")]
    public class CollectHipServiceTest
    {
        [Fact]
        private async void ReturnEntriesForCorrectRequest()
        {
            var openMrsPatientData = new Mock<IOpenMrsPatientData>();
            var collectHipService = new CollectHipService(openMrsPatientData.Object);
            const string consentId = "ConsentId";
            const string consentManagerId = "ConsentManagerId";
            var grantedContexts = new List<GrantedContext>
            {
                new GrantedContext("RVH1003", "BI-KTH-12.05.0024"),
            };

            var dateRange = new DateRange("2017-12-01T15:43:00.818234", "2021-12-31T15:43:00.818234");
            var hiTypes = new List<HiType>
            {
                HiType.Prescription,
            };
            var traceableDataRequest = new TraceableDataRequest(grantedContexts,
                dateRange,
                "/someUrl",
                hiTypes,
                "someTxnId",
                null,
                consentManagerId,
                consentId,
                "sometext",
                Uuid.Generate().ToString(),
                "patientUuid"
            );
            var listOfDataFiles = new List<string>();
            listOfDataFiles.Add("{\"resourceType\": \"Bundle\"}");
            openMrsPatientData
                .Setup(x => x.GetPatientData("patientUuid", grantedContexts[0].CareContextReference,
                    dateRange.To,
                    dateRange.From,
                    HiType.Prescription.ToString().ToLower()))
                .ReturnsAsync(listOfDataFiles)
                .Verifiable();
            var entries = await collectHipService.CollectData(traceableDataRequest);
            entries.ValueOrDefault().CareBundles.Count().Should().Be(1);
        }

        [Fact]
        private async void ReturnZeroEntriesForWrongRequest()
        {
            var openMrsPatientData = new Mock<IOpenMrsPatientData>();
            var collectHipService = new CollectHipService(openMrsPatientData.Object);
            const string consentId = "ConsentId";
            const string consentManagerId = "ConsentManagerId";
            var grantedContexts = new List<GrantedContext>
            {
                new GrantedContext("RVH1003", "BI-KTH-12.05.0024"),
            };

            var dateRange = new DateRange("2017-12-01T15:43:00.818234", "2021-12-31T15:43:00.818234");
            var hiTypes = new List<HiType>
            {
                HiType.Prescription,
            };
            var traceableDataRequest = new TraceableDataRequest(grantedContexts,
                dateRange,
                "/someUrl",
                hiTypes,
                "someTxnId",
                null,
                consentManagerId,
                consentId,
                "sometext",
                Uuid.Generate().ToString(),
                "patientUuid"
            );
            openMrsPatientData
                .Setup(x => x.GetPatientData("patientUuid", grantedContexts[0].CareContextReference,
                    dateRange.To,
                    dateRange.From,
                    HiType.Prescription.ToString().ToLower()))
                .ReturnsAsync((List<string>) null)
                .Verifiable();
            var entries = await collectHipService.CollectData(traceableDataRequest);
            entries.ValueOrDefault().CareBundles.Count().Should().Be(0);
        }
    }
}