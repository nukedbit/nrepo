using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NukedBit.NRepo;
using NukedBit.NRepo.Services;
using Octokit;
using Optional;
using Optional.Unsafe;
using Xunit;

namespace NRepo.Tests
{
    public class LicenseServiceTests
    {
        [Fact(DisplayName = "Display the license list to choose from")]
        public async Task PrintLicenseList()
        {
            var licenseApi = Substitute.For<IGitHubLicenseApi>();
            var consoleService = Substitute.For<IConsoleService>();
            var fileService = Substitute.For<IFileService>();
            var service = new LicenseService(licenseApi, consoleService, fileService);

            licenseApi.ListAsync().Returns(new List<LicenseMetadata>()
            {
                new LicenseMetadata("mit","mit", "mit","idx", "url", false)
            });

            var result = await service.PickLicenseAsync();

            consoleService.Received(1).WriteLine("{0}: {1}", 1, "mit");
        }

        [Fact(DisplayName = "Choose the first license")]
        public async Task ChooseFirstLicense()
        {
            var licenseApi = Substitute.For<IGitHubLicenseApi>();
            var consoleService = Substitute.For<IConsoleService>();
            var fileService = Substitute.For<IFileService>();
            var service = new LicenseService(licenseApi, consoleService, fileService);

            consoleService.ReadInputNumber(min: 1, max: 2).Returns(Option.Some(1));
            var expectedLicenseMetadata = new LicenseMetadata("gpl", "gpl", "gpl", "idx2", "url2", false);

            licenseApi.ListAsync().Returns(new List<LicenseMetadata>()
            {
                new LicenseMetadata("mit", "mit", "mit", "idx", "url", false),
                expectedLicenseMetadata
            });

            licenseApi.DownloadLicenseContentAsync(Arg.Is<LicenseMetadata>(a => a.Key == "gpl")).Returns("license content");

            var result = await service.PickLicenseAsync();

            await licenseApi.Received(1).DownloadLicenseContentAsync(expectedLicenseMetadata);

            Assert.Equal("LICENSE", result.ValueOrDefault());
        }

        [Fact(DisplayName = "Do not choose a license")]
        public async Task NoLicenseChoiceWithExit()
        {
            var licenseApi = Substitute.For<IGitHubLicenseApi>();
            var consoleService = Substitute.For<IConsoleService>();
            var fileService = Substitute.For<IFileService>();
            var service = new LicenseService(licenseApi, consoleService, fileService);

            consoleService.ReadInputNumber(min: 1, max: 2).Returns(Option.None<int>());
            var expectedLicenseMetadata = new LicenseMetadata("gpl", "gpl", "gpl", "idx2", "url2", false);

            licenseApi.ListAsync().Returns(new List<LicenseMetadata>()
            {
                new LicenseMetadata("mit", "mit", "mit", "idx", "url", false),
                expectedLicenseMetadata
            });

            var result = await service.PickLicenseAsync();

            Assert.Null(result.ValueOrDefault());
        }
    }
}
