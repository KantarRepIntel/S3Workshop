namespace S3Workshop.Tests
{
    using Amazon.S3;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Xunit;

    public class AwsS3StorageTests
    {
        private readonly string _bucketIntegration = "repintel-integration-tests";
        private readonly AwsS3Storage _storage;

        public AwsS3StorageTests()
        {
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
            var services = new ServiceCollection();
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddScoped<AwsS3Storage>();
            var provider = services.BuildServiceProvider();

            _storage = provider.GetService<AwsS3Storage>()!;
        }

        [Fact]
        public async Task Should_Save_When_JsonText()
        {
            var fileName = Guid.NewGuid().ToString();
            var expectedContent = @"{""fruit"": ""Apple"",""size"": ""Large"",""color"": ""Red🤙 💪 🦵 🦶 🖕 ✍️ 🙏 💍 💄 💋 👄 👅 👂 👃 👣 👁 👀 🧠 🦴 🦷 🗣 👤 👥""}";

            await _storage.SaveAsync(_bucketIntegration, fileName, S3CannedACL.PublicRead, expectedContent);

            var actualContent = await _storage.GetAsync(_bucketIntegration, fileName);

            await _storage.DeleteAsync(_bucketIntegration, fileName);

            Assert.Equal(expectedContent, actualContent);
        }
    }
}
