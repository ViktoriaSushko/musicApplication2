using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using musicApplication.Settings;
namespace musicApplication.Services
{
    public class S3Service
    {
        private readonly AmazonS3Client client;
        private readonly AwsSettings settings;
        public S3Service(IConfiguration configuration)
        {
            this.settings = configuration.GetSection("AWS").Get<AwsSettings>()!;
            this.client = new AmazonS3Client(settings.AccessKey, settings.SecretKey, RegionEndpoint.GetBySystemName(settings.Region));
        }
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var key = Guid.NewGuid() + "_" + file.FileName;
            using var stream = file.OpenReadStream();
            var request = new PutObjectRequest()
            {
                BucketName = settings.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = file.ContentType
            };
            await client.PutObjectAsync(request);
            return key;
        }
        public string GetFileUrl(string key)
        {
            return $"https://{settings.BucketName}.s3.amazonaws.com/{key}";
        }
        public async Task DeleteFileAsync(string key)
        {
            await client.DeleteObjectAsync(settings.BucketName, key);
        }
    }
}
