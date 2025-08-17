using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using SafeVault.Model;

namespace SafeVault.Service;

public class DynamoDbService
{
    private static AmazonDynamoDBClient _amazonDynamoDbClient;

    public DynamoDbService()
    {
        InitializeDynamoDb();
    }
    
    private void InitializeDynamoDb()
    {
        string projectDir = Directory.GetCurrentDirectory();
        string credentialsFilePath = Path.Combine(projectDir, "aws-credentials", "credentials.json");
        
        string jsonContent = File.ReadAllText(credentialsFilePath);
        var credentialsJson = JsonConvert.DeserializeObject<CredentialsJson>(jsonContent);

        var awsCredentials = credentialsJson.Default;
        
        var credentials = new BasicAWSCredentials(awsCredentials.IAM_ACCESS_KEY, awsCredentials.IAM_SECRET_KEY);
            
        _amazonDynamoDbClient = new AmazonDynamoDBClient(credentials, RegionEndpoint.GetBySystemName(awsCredentials.Region));   
    }
    
    public string GetEncryptionKey()
    {
        // change this somehow, need to find a way
        string tableName = "EncryptionKeys";
        
        
        // key is index 0 in this database; used for other database
        var key = new Dictionary<string, AttributeValue>
        {
            { "KeyId", new AttributeValue { S = "0" } }
        };
        
        var response = _amazonDynamoDbClient.GetItemAsync(new GetItemRequest
        {
            TableName = tableName,
            Key = key
        }).GetAwaiter().GetResult();

        try
        {
            if (response.Item != null)
            {
                if (response.Item.TryGetValue("Key Value", out var value))
                {
                    return value.S;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error has occured receiving ID");
        }

        return null;
    }

    public void Put(string applicationName, string username, string password)
    {
        // find way to replace hardcoded table name; randomly generated key id
        string tableName = "SafeVault";
        string keyId = Guid.NewGuid().ToString();
        
        var item = new Dictionary<string, AttributeValue>
        {
            { "id", new AttributeValue { S = keyId } },
            { "Application Name", new AttributeValue { S = applicationName } }, 
            { "Username", new AttributeValue { S = username } },
            { "Password", new AttributeValue { S = password } } 
        };

        var request = _amazonDynamoDbClient.PutItemAsync(new PutItemRequest
        {
            TableName = tableName,
            Item = item
        }).GetAwaiter().GetResult();
        
        try
        {
            Console.WriteLine("Item inserted successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting item: {ex.Message}");
        }
    }
}