using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Util.Store;
using MailKit.Net.Smtp;
using MailKit.Security;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GmailOAuth
{
    public static class Smtp
    {
        public static async Task<SmtpClient> CreateServiceAccountSmtpClientAsync(ServiceAccount serviceAccount, string senderEmail)
        {
            var credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(serviceAccount.Client_email)
                {
                    User = senderEmail, // The user you want to impersonate
                    Scopes = new[] { GmailService.Scope.MailGoogleCom, GmailService.Scope.GmailSend }
                }.FromPrivateKey(serviceAccount.Private_key));

            if (!await credential.RequestAccessTokenAsync(CancellationToken.None).ConfigureAwait(false))
                throw new Exception("Error getting access token.");

            return await CreateSmtpClientAsync(senderEmail, credential.Token.AccessToken).ConfigureAwait(false);
        }

        public static async Task<SmtpClient> CreateSmtpClientAsync(ProjectAuth projectAuth, string senderEmail)
        {
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = projectAuth.Installed.Client_id,
                ClientSecret = projectAuth.Installed.Client_secret
            }, new[] { "https://mail.google.com/" }, "user", CancellationToken.None,
                new FileDataStore("token.json", true)).ConfigureAwait(false);

            return await CreateSmtpClientAsync(senderEmail, credential.Token.AccessToken).ConfigureAwait(false);
        }

        private static async Task<SmtpClient> CreateSmtpClientAsync(string senderEmail, string accessToken)
        {
            var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls).ConfigureAwait(false);
            client.AuthenticationMechanisms.Remove("PLAIN");
            client.AuthenticationMechanisms.Remove("LOGIN");
            await client.AuthenticateAsync(new SaslMechanismOAuth2(senderEmail, accessToken)).ConfigureAwait(false);
            return client;
        }
    }
}