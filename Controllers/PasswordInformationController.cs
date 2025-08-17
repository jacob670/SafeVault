using Microsoft.AspNetCore.Mvc;
using SafeVault.Model;
using SafeVault.Service;

namespace SafeVault.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordInformationController : ControllerBase
{
    // figure out how to get value from database and decrypt it
    // update react js 
    
    private readonly AesEncryptionService _aesEncryptionService;
    private readonly DynamoDbService _dynamoDbService;

    public PasswordInformationController(AesEncryptionService aesEncryptionService, DynamoDbService dynamoDbService)
    {
        _aesEncryptionService = aesEncryptionService;
        _dynamoDbService = dynamoDbService;
    }
    
    [HttpPost("greeting")]
    public IActionResult SubmitAppInformation([FromBody] PasswordInformation info)
    {
        byte[] encryptedPassword = _aesEncryptionService.EncryptString(info.password, _aesEncryptionService.iv);
        string encrpytedPassword = Convert.ToBase64String(encryptedPassword);

        string decryptedPassword = _aesEncryptionService.DecryptString(encryptedPassword, _aesEncryptionService.iv);

        return Ok (new
        {
            info.applicationName, info.username, info.password, encrpytedPassword, decryptedPassword 
        });
    }
    
    [HttpPost("pushInfo")]
    public ActionResult<string> PostDb([FromBody] PasswordInformation info)
    {
        try
        {
            byte[] encryptedBytePassword = _aesEncryptionService.EncryptString(info.password, _aesEncryptionService.iv);
            string encryptedPassword = Convert.ToBase64String(encryptedBytePassword);

            _dynamoDbService.Put(info.applicationName, info.username, encryptedPassword);

            return Ok("Success");
        }

        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}