namespace S3Workshop
{
    using Amazon.S3;
    using Amazon.S3.Model;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class AwsS3Storage
    {
        private readonly IAmazonS3 _client;

        public AwsS3Storage(IAmazonS3 client)
        {
            _client = client;
        }

        public async Task<string> SaveAsync(string bucketName, string key, S3CannedACL cannedACL, string contentBody,
                                            string? contentType = null)
        {
            CheckParameters(bucketName, key);

            var respnose = await _client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                CannedACL = cannedACL,
                ContentType = contentType,
                ContentBody = contentBody
            });

            return respnose.ResponseMetadata.RequestId;
        }

        public async Task<string> GetAsync(string bucketName, string key)
        {
            CheckParameters(bucketName, key);

            var response = await _client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            });

            using var reader = new StreamReader(response.ResponseStream);
            var data = await reader.ReadToEndAsync()!;

            return data;
        }

        public async Task<string> DeleteAsync(string bucketName, string key)
        {
            CheckParameters(bucketName, key);

            var response = await _client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
            });

            return response.ResponseMetadata.RequestId;
        }

        private static void CheckParameters(string bucketName, string key)
        {
            if (String.IsNullOrWhiteSpace(bucketName))
                throw new ArgumentNullException(nameof(bucketName));

            if (String.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
        }
    }
}
