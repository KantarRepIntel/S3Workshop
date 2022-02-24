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
        private const string _bucketIntegration = "workshop-demo-2022";
        private readonly AwsS3Storage _storage;

        public AwsS3StorageTests()
        {
            var services = new ServiceCollection();

            services.AddDefaultAWSOptions(new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build().GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddScoped<AwsS3Storage>();

            _storage = services.BuildServiceProvider()
                .GetService<AwsS3Storage>()!;
        }

        [Fact]
        public async Task Should_Save_When_JsonText()
        {
            // Arrange
            var fileName = Guid.NewGuid().ToString();
            var expected = @"{""field"": ""value 💪 🦵 🦶 🖕 ✍️ 🙏 💍 👅 👂 👃 👣 👁 👀 🧠 🦴 🦷 🗣 👤 👥""}";

            // Act
            await _storage.SaveAsync(_bucketIntegration, fileName, S3CannedACL.PublicRead, expected);
            var actual = await _storage.GetAsync(_bucketIntegration, fileName);
            await _storage.DeleteAsync(_bucketIntegration, fileName);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
