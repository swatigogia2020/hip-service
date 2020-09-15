using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using In.ProjectEKA.HipService.OpenMrs;
using Moq;
using Xunit;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    [Collection("OpenMrs Data flow Repository Tests")]
    public class OpenMrsDataFlowRepositoryTest
    {
        OpenMrsDataFlowRepository dataFlowRepository;

        private readonly Mock<IOpenMrsClient> openmrsClientMock;

        public OpenMrsDataFlowRepositoryTest()
        {
            openmrsClientMock = new Mock<IOpenMrsClient>();
            dataFlowRepository = new OpenMrsDataFlowRepository(openmrsClientMock.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldReturnBundle()
        {
            //Given
            var patientId = "123";
            var careContextReference = "OPD";
            SetupOpenMrsClient();

            //When
            var bundle = await dataFlowRepository.GetBundleForCareContext(patientId, careContextReference);

            //Then
            Assert.NotNull(bundle);
        }

        private void SetupOpenMrsClient()
        {
            var openMrsBundleJsonResponse = @"{
                ""resourceType"": ""Bundle"",
                ""entry"": [
                    {
                        ""resource"": {
                            ""resourceType"": ""MedicationRequest"",
                        ""dosageInstruction"": [
                            {
                                ""doseAndRate"": [
                                    {
                                    ""doseQuantity"": {
                                        ""value"": 3.0
                                        }
                                }
                                ]
                            }
                        ],
                        ""dispenseRequest"": {
                                ""quantity"": {
                                    ""value"": 30.0
                                }
                            }
                        }
                    }
                ]
            }";
            openmrsClientMock
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(openMrsBundleJsonResponse)
                })
                .Verifiable();
        }
    }
}
