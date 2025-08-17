using Newtonsoft.Json;

namespace SafeVault.Model;

public class AwsCredential
{
    [JsonProperty("aws_access_key_id")]
    public string IAM_ACCESS_KEY { get; set; }

    [JsonProperty("aws_secret_access_key")]
    public string IAM_SECRET_KEY { get; set; }

    public string Region { get; set; }
}