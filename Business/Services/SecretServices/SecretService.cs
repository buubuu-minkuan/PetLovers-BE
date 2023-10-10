using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.SecretServices
{
    public static class SecretService
    {
        public static string URI = "https://petloverssecret.vault.azure.net/";
        public static string GetConnectionString()
        {
            string? conn = null;
            try
            {
                SecretClientOptions options = new()
                {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
                SecretClient client = new(new Uri(URI), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret("PetLoversDB");

                conn = secret.Value;
            }
            catch (Exception ex)
            {
                _ = ex.ToString();
            }

            return conn;
        }
        
        public static string GetJWTKey()
        {
            string? key = null;
            try
            {
                SecretClientOptions options = new()
                {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
                SecretClient client = new(new Uri(URI), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret("JWTKey");

                key = secret.Value;
            }
            catch (Exception ex)
            {
                _ = ex.ToString();
            }

            return key;
        }

        public static string GetJWTIssuser()
        {
            string? issuser = null;
            try
            {
                SecretClientOptions options = new()
                {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
                SecretClient client = new(new Uri(URI), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret("JWTIssuser");

                issuser = secret.Value;
            }
            catch (Exception ex)
            {
                _ = ex.ToString();
            }

            return issuser;
        }

        public static string GetSMTPEmail()
        {
            string? issuser = null;
            try
            {
                SecretClientOptions options = new()
                {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
                SecretClient client = new(new Uri(URI), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret("SMTPEmail");

                issuser = secret.Value;
            }
            catch (Exception ex)
            {
                _ = ex.ToString();
            }

            return issuser;
        }

        public static string GetSMTPPass()
        {
            string? issuser = null;
            try
            {
                SecretClientOptions options = new()
                {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
                SecretClient client = new(new Uri(URI), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret("SMTPPass");

                issuser = secret.Value;
            }
            catch (Exception ex)
            {
                _ = ex.ToString();
            }

            return issuser;
        }
    }
}
