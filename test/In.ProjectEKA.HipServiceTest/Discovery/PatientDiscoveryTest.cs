namespace In.ProjectEKA.HipServiceTest.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using HipLibrary.Matcher;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using HipService.Discovery;
    using HipService.Link;
    using HipService.Link.Model;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Optional;
    using Xunit;
    using Match = HipLibrary.Patient.Model.Match;
    using static Builder.TestBuilders;
    using In.ProjectEKA.HipServiceTest.Discovery.Builder;
    using In.ProjectEKA.HipService.OpenMrs;

    public class PatientDiscoveryTest
    {
        private readonly PatientDiscovery patientDiscovery;

        private readonly Mock<IDiscoveryRequestRepository> discoveryRequestRepository =
            new Mock<IDiscoveryRequestRepository>();
        private readonly Mock<ILinkPatientRepository> linkPatientRepository = new Mock<ILinkPatientRepository>();
        private readonly Mock<IMatchingRepository> matchingRepository = new Mock<IMatchingRepository>();
        private readonly Mock<IPatientRepository> patientRepository = new Mock<IPatientRepository>();
        private readonly Mock<ICareContextRepository> careContextRepository = new Mock<ICareContextRepository>();

        DiscoveryRequestPayloadBuilder discoveryRequestBuilder;

        string openMrsPatientReferenceNumber;
        string openMrsPatientIdentifier;
        string name;
        string phoneNumber;
        string consentManagerUserId;
        string transactionId;
        ushort yearOfBirth;
        Gender gender;

        private readonly Mock<ILogger<PatientDiscovery>> logger = new Mock<ILogger<PatientDiscovery>>();
        public PatientDiscoveryTest()
        {
            patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object,
                careContextRepository.Object,
                logger.Object);

            openMrsPatientReferenceNumber = Faker().Random.String();
            openMrsPatientIdentifier = Faker().Random.String();
            name = Faker().Random.String();
            phoneNumber = Faker().Phone.PhoneNumber();
            consentManagerUserId = Faker().Random.String();
            transactionId = Faker().Random.String();
            yearOfBirth = 2019;
            gender = Gender.M;

            discoveryRequestBuilder = new DiscoveryRequestPayloadBuilder();

            discoveryRequestBuilder
                .WithPatientId(consentManagerUserId)
                .WithPatientName(name)
                .WithPatientGender(gender)
                .WithVerifiedIdentifiers(IdentifierType.MOBILE, phoneNumber)
                .WithUnverifiedIdentifiers(IdentifierType.MR, openMrsPatientIdentifier)
                .WithTransactionId(transactionId);

        }

        [Fact]
        private async void ShouldReturnPatientForAlreadyLinkedPatient()
        {
            var alreadyLinked =
                new CareContextRepresentation(Faker().Random.Uuid().ToString(), Faker().Random.String());
<<<<<<< HEAD
            var unlinkedCareContext = new List<CareContextRepresentation>{
                new CareContextRepresentation(Faker().Random.Uuid().ToString(), Faker().Random.String())
            };
            var expectedPatient = BuildExpectedPatientByExpectedMatchTypes(
                expectedCareContextRepresentation: unlinkedCareContext,
                expectedMatchTypes: Match.ConsentManagerUserId);
            var discoveryRequest = discoveryRequestBuilder.Build();
            SetupPatientRepository(alreadyLinked, unlinkedCareContext.First());
            SetupLinkRepositoryWithLinkedPatient(alreadyLinked, openMrsPatientIdentifier);
=======
            var unlinkedCareContext =
                new CareContextRepresentation(Faker().Random.Uuid().ToString(), Faker().Random.String());
            var expectedPatient = new PatientEnquiryRepresentation(
                patientId,
                name,
                new[] {unlinkedCareContext},
                new[] {Match.ConsentManagerUserId.ToString()});
            var transactionId = Faker().Random.String();
            var patientRequest = new PatientEnquiry(patientId,
                verifiedIdentifiers,
                unverifiedIdentifiers,
                name,
                HipLibrary.Patient.Model.Gender.M,
                2019);
            var discoveryRequest = new DiscoveryRequest(patientRequest, RandomString(), transactionId, DateTime.Now);
            var sessionId = Faker().Random.Hash();
            var linkedCareContext = new[] {alreadyLinked.ReferenceNumber};
            var testLinkAccounts = new LinkedAccounts(patientId,
                sessionId,
                Faker().Random.Hash(),
                It.IsAny<string>(),
                linkedCareContext.ToList());
            var testPatient =
                new HipLibrary.Patient.Model.Patient
                {
                    PhoneNumber = phoneNumber,
                    Identifier = patientId,
                    Gender = Faker().PickRandom<HipLibrary.Patient.Model.Gender>(),
                    Name = name,
                    CareContexts = new[]
                    {
                        alreadyLinked,
                        unlinkedCareContext
                    }
                };
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(patientId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(
                    new List<LinkedAccounts> {testLinkAccounts},
                    null));
            patientRepository.Setup(x => x.PatientWith(testPatient.Identifier))
                .Returns(Option.Some(testPatient));
>>>>>>> 72cd72a... JAS-971 | Mahendra/Meghna/Sangita | Refactors for Sharing the user demo on confirm (#391)

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Patient.Should().BeEquivalentTo(expectedPatient);
            discoveryRequestRepository.Verify(
                x => x.Add(It.Is<HipService.Discovery.Model.DiscoveryRequest>(
                    r => r.TransactionId == transactionId && r.ConsentManagerUserId == consentManagerUserId)),
                Times.Once);
            error.Should().BeNull();
        }

        [Fact]
        private async void ShouldReturnAPatientWhichIsNotLinkedAtAll()
        {
<<<<<<< HEAD
            var expectedPatient = BuildExpectedPatientByExpectedMatchTypes(
                Match.Mobile,
                Match.Name,
                Match.Gender,
                Match.Mr);
            var discoveryRequest = discoveryRequestBuilder.Build();
            SetupLinkRepositoryWithLinkedPatient();
            SetupMatchingRepositoryForDiscoveryRequest(discoveryRequest);
=======
            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object,
                logger.Object);
            var referenceNumber = Faker().Random.String();
            var name = Faker().Random.String();
            var phoneNumber = Faker().Phone.PhoneNumber();
            var consentManagerUserId = Faker().Random.String();
            var transactionId = Faker().Random.String();
            const ushort yearOfBirth = 2019;
            var careContextRepresentations = new[]
            {
                new CareContextRepresentation(Faker().Random.String(), Faker().Random.String()),
                new CareContextRepresentation(Faker().Random.String(), Faker().Random.String())
            };
            var expectedPatient = new PatientEnquiryRepresentation(referenceNumber,
                name,
                careContextRepresentations,
                new List<string>
                {
                    Match.Mobile.ToString(),
                    Match.Name.ToString(),
                    Match.Gender.ToString(),
                    Match.Mr.ToString()
                });
            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MOBILE, phoneNumber)
            };
            var unverifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MR, referenceNumber)
            };
            var patientRequest = new PatientEnquiry(consentManagerUserId,
                verifiedIdentifiers,
                unverifiedIdentifiers,
                name,
                HipLibrary.Patient.Model.Gender.M,
                yearOfBirth);
            var discoveryRequest = new DiscoveryRequest(patientRequest, RandomString(), transactionId, DateTime.Now);
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(consentManagerUserId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(new List<LinkedAccounts>(), null));
            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<HipLibrary.Patient.Model.Patient>
                {
                    new HipLibrary.Patient.Model.Patient
                    {
                        Gender = HipLibrary.Patient.Model.Gender.M,
                        Identifier = referenceNumber,
                        Name = name,
                        CareContexts = careContextRepresentations,
                        PhoneNumber = phoneNumber,
                        YearOfBirth = yearOfBirth
                    }
                }.AsQueryable()));
>>>>>>> 72cd72a... JAS-971 | Mahendra/Meghna/Sangita | Refactors for Sharing the user demo on confirm (#391)

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Patient.Should().BeEquivalentTo(expectedPatient);
            discoveryRequestRepository.Verify(
                x => x.Add(It.Is<HipService.Discovery.Model.DiscoveryRequest>(
                    r => r.TransactionId == transactionId && r.ConsentManagerUserId == consentManagerUserId)),
                Times.Once);
            error.Should().BeNull();
        }

        [Fact]
        private async void ShouldReturnAPatientWhenUnverifiedIdentifierIsNull()
        {
<<<<<<< HEAD
            var expectedPatient = BuildExpectedPatientByExpectedMatchTypes(
                Match.Mobile,
                Match.Name,
                Match.Gender);

            var discoveryRequest = discoveryRequestBuilder.WithUnverifiedIdentifiers(null).Build();
            SetupLinkRepositoryWithLinkedPatient();
            SetupMatchingRepositoryForDiscoveryRequest(discoveryRequest);
=======
            var patientDiscovery = new PatientDiscovery(
                matchingRepository.Object,
                discoveryRequestRepository.Object,
                linkPatientRepository.Object,
                patientRepository.Object,
                logger.Object);
            var referenceNumber = Faker().Random.String();
            var consentManagerUserId = Faker().Random.String();
            var transactionId = Faker().Random.String();
            var name = Faker().Name.FullName();
            const ushort yearOfBirth = 2019;
            var phoneNumber = Faker().Phone.PhoneNumber();
            var careContextRepresentations = new[]
            {
                new CareContextRepresentation(Faker().Random.String(), Faker().Random.String()),
                new CareContextRepresentation(Faker().Random.String(), Faker().Random.String())
            };
            var expectedPatient = new PatientEnquiryRepresentation(
                referenceNumber,
                name,
                careContextRepresentations,
                new List<string>
                {
                    Match.Mobile.ToString(),
                    Match.Name.ToString(),
                    Match.Gender.ToString()
                });
            var verifiedIdentifiers = new[] {new Identifier(IdentifierType.MOBILE, phoneNumber)};
            var patientRequest = new PatientEnquiry(consentManagerUserId,
                verifiedIdentifiers,
                null,
                name,
                HipLibrary.Patient.Model.Gender.M,
                yearOfBirth);
            var discoveryRequest = new DiscoveryRequest(patientRequest, RandomString(), transactionId, DateTime.Now);
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(consentManagerUserId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(new List<LinkedAccounts>(), null));
            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<HipLibrary.Patient.Model.Patient>
                {
                    new HipLibrary.Patient.Model.Patient
                    {
                        Gender = HipLibrary.Patient.Model.Gender.M,
                        Identifier = referenceNumber,
                        Name = name,
                        CareContexts = careContextRepresentations,
                        PhoneNumber = phoneNumber,
                        YearOfBirth = yearOfBirth
                    }
                }.AsQueryable()));
>>>>>>> 72cd72a... JAS-971 | Mahendra/Meghna/Sangita | Refactors for Sharing the user demo on confirm (#391)

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Patient.Should().BeEquivalentTo(expectedPatient);
            discoveryRequestRepository.Verify(
                x => x.Add(It.Is<HipService.Discovery.Model.DiscoveryRequest>(
                    r => r.TransactionId == transactionId && r.ConsentManagerUserId == consentManagerUserId)),
                Times.Once);
            error.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(EmptyIdentifierTestData))]
        private async void ReturnMultiplePatientsErrorWhenUnverifiedIdentifierIs(IEnumerable<Identifier> identifiers)
        {
            var expectedError =
                new ErrorRepresentation(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));
<<<<<<< HEAD

            var discoveryRequest =
                discoveryRequestBuilder
                    .WithUnverifiedIdentifiers(identifiers?.ToList())
                    .Build();

            SetupLinkRepositoryWithLinkedPatient();
            SetupMatchingRepositoryForDiscoveryRequest(discoveryRequest, numberOfPatients: 2);
=======
            var verifiedIdentifiers = new[] {new Identifier(IdentifierType.MOBILE, Faker().Phone.PhoneNumber())};
            var consentManagerUserId = Faker().Random.String();
            const ushort yearOfBirth = 2019;
            HipLibrary.Patient.Model.Gender gender = Faker().PickRandom<HipLibrary.Patient.Model.Gender>();
            var name = Faker().Name.FullName();
            var patientRequest = new PatientEnquiry(consentManagerUserId,
                verifiedIdentifiers,
                identifiers,
                name,
                gender,
                yearOfBirth);
            var discoveryRequest = new DiscoveryRequest(patientRequest, Faker().Random.String(), RandomString(),
                DateTime.Now);
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(consentManagerUserId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(new List<LinkedAccounts>(), null));

            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<HipLibrary.Patient.Model.Patient>
                {
                    new HipLibrary.Patient.Model.Patient
                    {
                        YearOfBirth = yearOfBirth,
                        Gender = gender,
                        Name = name
                    },
                    new HipLibrary.Patient.Model.Patient
                    {
                        YearOfBirth = yearOfBirth,
                        Gender = gender,
                        Name = name
                    }
                }.AsQueryable()));
>>>>>>> 72cd72a... JAS-971 | Mahendra/Meghna/Sangita | Refactors for Sharing the user demo on confirm (#391)

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetMultiplePatientsFoundErrorWhenSameUnverifiedIdentifiersAlsoMatch()
        {
            var expectedError =
                new ErrorRepresentation(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));
<<<<<<< HEAD
            var discoveryRequest = discoveryRequestBuilder.Build();
            SetupLinkRepositoryWithLinkedPatient();
            SetupMatchingRepositoryForDiscoveryRequest(discoveryRequest, numberOfPatients: 2);
=======
            var patientReferenceNumber = Faker().Random.String();
            var consentManagerUserId = Faker().Random.String();
            const ushort yearOfBirth = 2019;
            var gender = Faker().PickRandom<HipLibrary.Patient.Model.Gender>();
            var name = Faker().Name.FullName();
            var verifiedIdentifiers = new[] {new Identifier(IdentifierType.MOBILE, Faker().Phone.PhoneNumber())};
            var unverifiedIdentifiers = new[] {new Identifier(IdentifierType.MR, patientReferenceNumber)};
            var patientRequest = new PatientEnquiry(consentManagerUserId,
                verifiedIdentifiers,
                unverifiedIdentifiers,
                name,
                gender,
                yearOfBirth);
            var discoveryRequest = new DiscoveryRequest(patientRequest, Faker().Random.String(), RandomString(),
                DateTime.Now);
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(consentManagerUserId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(new List<LinkedAccounts>(), null));

            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(new List<HipLibrary.Patient.Model.Patient>
                {
                    new HipLibrary.Patient.Model.Patient
                    {
                        Identifier = patientReferenceNumber,
                        YearOfBirth = yearOfBirth,
                        Gender = gender,
                        Name = name
                    },
                    new HipLibrary.Patient.Model.Patient
                    {
                        Identifier = patientReferenceNumber,
                        YearOfBirth = yearOfBirth,
                        Gender = gender,
                        Name = name
                    }
                }.AsQueryable()));
>>>>>>> 72cd72a... JAS-971 | Mahendra/Meghna/Sangita | Refactors for Sharing the user demo on confirm (#391)

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldGetNoPatientFoundErrorWhenNoPatientMatchedInOpenMrs()
        {
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.NoPatientFound, "No patient found"));
<<<<<<< HEAD
            var discoveryRequest = discoveryRequestBuilder.Build();
            SetupLinkRepositoryWithLinkedPatient();
            SetupMatchingRepositoryForDiscoveryRequest(discoveryRequest, numberOfPatients: 0);
=======
            var verifiedIdentifiers = new List<Identifier>
            {
                new Identifier(IdentifierType.MR, Faker().Phone.PhoneNumber())
            };
            var patientRequest = new PatientEnquiry(consentManagerUserId,
                verifiedIdentifiers,
                new List<Identifier>(),
                null,
                HipLibrary.Patient.Model.Gender.M,
                2019);
            var discoveryRequest = new DiscoveryRequest(patientRequest, Faker().Random.String(), RandomString(),
                DateTime.Now);
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(consentManagerUserId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(new List<LinkedAccounts>(), null));
>>>>>>> 72cd72a... JAS-971 | Mahendra/Meghna/Sangita | Refactors for Sharing the user demo on confirm (#391)

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Theory]
        [ClassData(typeof(EmptyIdentifierTestData))]
        private async void ReturnNoPatientFoundErrorWhenVerifiedIdentifierIs(IEnumerable<Identifier> identifiers)
        {
            var expectedError = new ErrorRepresentation(new Error(ErrorCode.NoPatientFound, "No patient found"));
<<<<<<< HEAD
            var discoveryRequest = discoveryRequestBuilder.Build();
            SetupLinkRepositoryWithLinkedPatient();
            SetupMatchingRepositoryForDiscoveryRequest(discoveryRequest, numberOfPatients: 0);
=======
            var patientRequest = new PatientEnquiry(consentManagerUserId,
                identifiers,
                new List<Identifier>(),
                null,
                HipLibrary.Patient.Model.Gender.M,
                2019);
            var discoveryRequest = new DiscoveryRequest(patientRequest, Faker().Random.String(), RandomString(),
                DateTime.Now);
            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(consentManagerUserId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(new List<LinkedAccounts>(), null));
>>>>>>> 72cd72a... JAS-971 | Mahendra/Meghna/Sangita | Refactors for Sharing the user demo on confirm (#391)

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldReturnAnErrorWhenDiscoveryRequestAlreadyExists()
        {
            var expectedError =
                new ErrorRepresentation(new Error(ErrorCode.DuplicateDiscoveryRequest,
                    "Discovery Request already exists"));
            var transactionId = RandomString();
            var discoveryRequest = new DiscoveryRequest(null, RandomString(), transactionId, DateTime.Now);
            discoveryRequestRepository.Setup(repository => repository.RequestExistsFor(transactionId))
                .ReturnsAsync(true);

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldReturnPatientWithCareContexts()
        {
            var careContextRepresentations = new[]
            {
                new CareContextRepresentation(Faker().Random.String(), Faker().Random.String()),
                new CareContextRepresentation(Faker().Random.String(), Faker().Random.String())
            };
            var expectedPatient = BuildExpectedPatientByExpectedMatchTypes(careContextRepresentations.ToList() , Match.Mobile,
                    Match.Name,
                    Match.Gender);

            var discoveryRequest = discoveryRequestBuilder.WithUnverifiedIdentifiers(null).Build();
            SetupLinkRepositoryWithLinkedPatient();
            SetupMatchingRepositoryForDiscoveryRequest(discoveryRequest);

            careContextRepository.Setup(e => e.GetCareContexts(openMrsPatientReferenceNumber))
                .Returns(Task.FromResult(new List<CareContextRepresentation>(careContextRepresentations).AsEnumerable()));

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Patient.Should().BeEquivalentTo(expectedPatient);
            discoveryRequestRepository.Verify(
                x => x.Add(It.Is<HipService.Discovery.Model.DiscoveryRequest>(
                    r => r.TransactionId == transactionId && r.ConsentManagerUserId == consentManagerUserId)),
                Times.Once);
            error.Should().BeNull();
        }

        [Fact]
        private async void ShouldReturnErrorIfFailedToFetchCareContexts()
        {
            var expectedError =
                new ErrorRepresentation(new Error(ErrorCode.CareContextConfiguration, "HIP configuration error. If you encounter this issue repeatedly, please report it"));
            var discoveryRequest = discoveryRequestBuilder.WithUnverifiedIdentifiers(null).Build();
            SetupLinkRepositoryWithLinkedPatient();
            SetupMatchingRepositoryForDiscoveryRequest(discoveryRequest);

            careContextRepository
                .Setup(e => e.GetCareContexts(openMrsPatientReferenceNumber))
                .Throws<OpenMrsFormatException>();

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        private async void ShouldReturnErrorIfFailedToConnectToOpenMrsServer()
        {
            var expectedError =
                new ErrorRepresentation(new Error(ErrorCode.OpenMrsConnection, "HIP connection error"));
            var discoveryRequest = discoveryRequestBuilder.WithUnverifiedIdentifiers(null).Build();
            SetupLinkRepositoryWithLinkedPatient();
            SetupMatchingRepositoryForDiscoveryRequest(discoveryRequest);

            matchingRepository
                .Setup(e => e.Where(discoveryRequest))
                .Throws<OpenMrsConnectionException>();

            var (discoveryResponse, error) = await patientDiscovery.PatientFor(discoveryRequest);

            discoveryResponse.Should().BeNull();
            error.Should().BeEquivalentTo(expectedError);
        }

        private PatientEnquiryRepresentation BuildExpectedPatientByExpectedMatchTypes(
            params Match[] expectedMatchTypes)
        {
            return BuildExpectedPatientByExpectedMatchTypes(null, expectedMatchTypes);
        }

        private PatientEnquiryRepresentation BuildExpectedPatientByExpectedMatchTypes(
            List<CareContextRepresentation> expectedCareContextRepresentation, params Match[] expectedMatchTypes)
        {
            var expectedCareContexts =
                expectedCareContextRepresentation switch
                {
                    null => new List<CareContextRepresentation>(),
                    _ => expectedCareContextRepresentation
                };

            return new PatientEnquiryRepresentation(openMrsPatientIdentifier,
                name,
                expectedCareContexts,
                expectedMatchTypes?.Select(m => m.ToString()));
        }

        private void SetupLinkRepositoryWithLinkedPatient(params string[] patientIds)
        {
            SetupLinkRepositoryWithLinkedPatient(null, patientIds);
        }

        private void SetupLinkRepositoryWithLinkedPatient(
            CareContextRepresentation linkedCareContextRepresentation, params string[] patientIds)
        {
            var linkedCareContexts =
                linkedCareContextRepresentation switch
                {
                    null => new List<CareContextRepresentation> {
                            new CareContextRepresentation(Faker().Random.Uuid().ToString(), Faker().Random.String())
                        },
                    _ => new List<CareContextRepresentation> { linkedCareContextRepresentation }
                };
            var linkedAccounts = patientIds.Select(p =>
                new LinkedAccounts(p,
                    Faker().Random.Hash(),
                    consentManagerUserId,
                    It.IsAny<string>(),
                    linkedCareContexts.Select(c => c.ReferenceNumber).ToList())
            );

            linkPatientRepository.Setup(e => e.GetLinkedCareContexts(consentManagerUserId))
                .ReturnsAsync(new Tuple<IEnumerable<LinkedAccounts>, Exception>(linkedAccounts, null));
        }

        private void SetupMatchingRepositoryForDiscoveryRequest(DiscoveryRequest discoveryRequest)
        {
            SetupMatchingRepositoryForDiscoveryRequest(discoveryRequest, 1);
        }

        private void SetupMatchingRepositoryForDiscoveryRequest(DiscoveryRequest discoveryRequest, int numberOfPatients){
            matchingRepository
                .Setup(repo => repo.Where(discoveryRequest))
                .Returns(Task.FromResult(Enumerable.Range(1, numberOfPatients)
                    .Select(_ => new Patient
                        {
                            Gender = gender,
                            Uuid = openMrsPatientReferenceNumber,
                            Name = name,
                            PhoneNumber = phoneNumber,
                            YearOfBirth = yearOfBirth,
                            Identifier = openMrsPatientIdentifier
                        }).ToList().AsQueryable()));
        }

        private void SetupPatientRepository(CareContextRepresentation alreadyLinked, CareContextRepresentation unlinkedCareContext)
        {
            var testPatient =
                new Patient
                {
                    PhoneNumber = phoneNumber,
                    Uuid = openMrsPatientReferenceNumber,
                    Identifier = openMrsPatientIdentifier,
                    Gender = Faker().PickRandom<Gender>(),
                    Name = name,
                    CareContexts = new[]
                    {
                        alreadyLinked,
                        unlinkedCareContext
                    }
                };
            patientRepository.Setup(x => x.PatientWithAsync(testPatient.Identifier))
                .ReturnsAsync(Option.Some(testPatient));
        }
    }

    internal class EmptyIdentifierTestData : TheoryData<IEnumerable<Identifier>>
    {
        public EmptyIdentifierTestData()
        {
            Add(null);
            Add(new Identifier[] {});
        }
    }
}