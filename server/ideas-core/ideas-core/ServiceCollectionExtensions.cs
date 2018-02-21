using CoE.Ideas.Core.Internal;
using CoE.Ideas.Core.Internal.Initiatives;
using CoE.Ideas.Core.People;
using CoE.Ideas.Core.ProjectManagement;
using CoE.Ideas.Core.Security;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using CoE.Ideas.ProjectManagement.Core.Internal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoE.Ideas.Core
{
    /// <summary>
    /// Extension methods for setting up MVC services in an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Idea services to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// -Specifically, adds an implementation of IIdeaRepository for use with Dependency Injection.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
        /// <param name="dbConnectionString">The connection string to the Idea database.</param>
        /// <returns>The passed in services, for chaining</returns>
        public static IServiceCollection AddIdeaConfiguration(this IServiceCollection services,
            string dbConnectionString, 
            string wordPressUrl,
            string jwtSecretKey = null)
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
                throw new ArgumentNullException("dbConnectionString");

            if (string.IsNullOrWhiteSpace(wordPressUrl))
                throw new ArgumentNullException("wordPressUrl");

            services.AddDbContext<IdeaContext>(options =>
                options.UseMySql(dbConnectionString));

            services.AddScoped<IIdeaRepository, IdeaRepositoryInternal>();
            services.AddScoped<IUpdatableIdeaRepository, IdeaRepositoryInternal>();

            // IHttpContextAccessor is used in WordpressClient
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var wordPressUri = new Uri(wordPressUrl);
            services.Configure<WordPressClientOptions>(options => 
            {
                options.Url = wordPressUri;
            });
            services.AddSingleton<IWordPressClient, WordPressClient>();

            services.Configure<JwtTokenizerOptions>(x =>
            {
                x.WordPressUrl = wordPressUri;
                x.JwtSecretKey = jwtSecretKey;
            });
            services.AddSingleton<IJwtTokenizer, JwtTokenizer>();


            return services;
        }

        public static IServiceCollection AddInitiativeMessaging(this IServiceCollection services,
            string serviceBusConnectionString,
            string serviceBusTopicName,
            string serviceBusSubscription = null)
        {

            services.AddSingleton<ITopicClient, TopicClient>(x =>
            {
                return new TopicClient(serviceBusConnectionString, serviceBusTopicName);
            });
            services.AddSingleton<IInitiativeMessageSender, InitiativeMessageSender>();

            if (!string.IsNullOrWhiteSpace(serviceBusSubscription))
            {
                services.AddSingleton<ISubscriptionClient, SubscriptionClient>(x =>
                {
                    return new SubscriptionClient(serviceBusConnectionString, serviceBusTopicName, serviceBusSubscription);
                });
                services.AddSingleton<IInitiativeMessageReceiver, InitiativeMessageReceiver>();
            }

            return services;
        }

        public static IServiceCollection AddProjectManagementConfiguration(
            this IServiceCollection services,
            string dbConnectionString)
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
                throw new ArgumentNullException("dbConnectionString");


            services.AddDbContext<ProjectManagementContext>(options =>
                options.UseMySql(dbConnectionString));



            services.AddScoped<IProjectManagementRepository, ProjectManagementRepositoryInternal>();

            return services;
        }

        public static IServiceCollection AddRemoteIdeaConfiguration(this IServiceCollection services,
            string ideasApiUrl, string wordpressUrl)
        {
            if (string.IsNullOrWhiteSpace(ideasApiUrl))
                throw new ArgumentNullException("ideasApiUrl");
            if (string.IsNullOrWhiteSpace(wordpressUrl))
                throw new ArgumentNullException("wordpressUrl");


            Uri wordpressUri = new Uri(wordpressUrl);
            services.AddScoped<IIdeaRepository, RemoteIdeaRepository>(x => new RemoteIdeaRepository(ideasApiUrl));


            services.Configure<WordPressClientOptions>(options =>
            {
                options.Url = wordpressUri;
            });
            services.AddScoped<IWordPressClient, WordPressClient>();
            return services;
        }


        public static IServiceCollection AddPeopleService(this IServiceCollection services,
            string peopleServiceUrl)
        {
            if (string.IsNullOrWhiteSpace(peopleServiceUrl))
                throw new ArgumentNullException("peopleServiceUrl");

            Uri peopleServiceUri;
            try
            {
                peopleServiceUri = new Uri(peopleServiceUrl);
            }
            catch (Exception err)
            {
                throw new InvalidOperationException($"peopleServiceUrl is not a valid url: { peopleServiceUrl }", err);
            }

            services.Configure<PeopleServiceOptions>(x =>
            {
                x.ServiceUrl = peopleServiceUri;
            });
            services.AddSingleton<IPeopleService, PeopleService>();

            return services;
        }

        /// <summary>
        /// Adds Idea authentication for WebAPI. Sets up JWT handlers for use with the token generator 
        /// in the WordPress JWT plugin
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
        /// <param name="jwtSecretKey">The JWT_SECRET_KEY, as specified in the corresponding WordPress installation.</param>
        /// <param name="coeAuthKey">The COE_AUTH_KEY, as specified in the corresponding WordPress installation.</param>
        /// <param name="coeAuthIV">The COE_AUTH_IV, as specified in the corresponding WordPress installation.</param>
        /// <param name="wordPressUrl">The full URL of the wordpress installation, used to verify issuers of the JWT token</param>
        /// <returns></returns>
        public static IServiceCollection AddIdeaAuthSecurity(this IServiceCollection services, 
            string jwtSecretKey, string coeAuthKey, string coeAuthIV, string wordPressUrl)
        {
            // get keys
            using (var mySHA256 = System.Security.Cryptography.SHA256Managed.Create())
            {
                __jwtDecrryptKey = mySHA256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(coeAuthKey));
                byte[] iv = mySHA256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(coeAuthIV));
                __jwtDecryptIV = new byte[16];
                for (int i = 0; i < 16; i++)
                    __jwtDecryptIV[i] = iv[i];
            }

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // TODO: Read in Wordpress Cookie
            //.AddCookie(options =>
            //{
            //    //options.
            //})
            .AddJwtBearer(options =>
            {
                var secretKey = jwtSecretKey;
                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = OnJwtMessageReceived,
                    OnTokenValidated = OnJwtTokenValidated
                };

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = wordPressUrl,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };
                options.Validate();
            });

            return services;
        }



        private static byte[] __jwtDecrryptKey = null;
        private static byte[] __jwtDecryptIV = null;

        private static Task OnJwtMessageReceived(MessageReceivedContext ctx)
        {
            var headers = ctx.Request.Headers;
            if (headers.ContainsKey("AuthorizationEncrypted") && "true".Equals(headers["AuthorizationEncrypted"], StringComparison.OrdinalIgnoreCase))
            {

                if (ctx.Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues authKey))
                {
                    string authKeyString = authKey.ToString();
                    bool? isBearer = authKeyString?.StartsWith("Bearer");
                    if (isBearer.HasValue && isBearer.Value && authKeyString.Length > 7)
                    {
                        // try to decrypt the authorizationkey as it should be encrypted

                        string decrypted = null;
                        try
                        {
                            decrypted = DecryptString(authKey.ToString().Substring(7), __jwtDecrryptKey, __jwtDecryptIV);
                        }
                        catch (Exception err)
                        {
                            throw new System.Security.SecurityException("Invalid authentication token", err);
                        }

                        ctx.Request.Headers["Authorization"] = "Bearer " + decrypted;
                    }
                }
            }
            return Task.CompletedTask;
        }

        private static string DecryptString(string cipherText, byte[] key, byte[] iv)
        {
            // from https://gist.github.com/odan/138dbd41a0c5ef43cbf529b03d814d7c

            // Instantiate a new Aes object to perform string symmetric encryption
            var encryptor = System.Security.Cryptography.Aes.Create();

            encryptor.Mode = System.Security.Cryptography.CipherMode.CBC;
            //encryptor.KeySize = 256;
            //encryptor.BlockSize = 128;
            //encryptor.Padding = PaddingMode.Zeros;

            // Set key and IV
            encryptor.Key = key;
            encryptor.IV = iv;

            // Instantiate a new encryptor from our Aes object
            var aesDecryptor = encryptor.CreateDecryptor();

            // Will contain decrypted plaintext
            string plainText = String.Empty;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            // and a new CryptoStream object to process the data and write it to the 
            // memory stream
            using (var memoryStream = new System.IO.MemoryStream())
            using (var cryptoStream = new System.Security.Cryptography.CryptoStream(memoryStream, aesDecryptor, System.Security.Cryptography.CryptoStreamMode.Write))
            { 
                // Convert the ciphertext string into a byte array
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Decrypt the input ciphertext string
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

                // Complete the decryption process
                cryptoStream.FlushFinalBlock();

                // Convert the decrypted data from a MemoryStream to a byte array
                byte[] plainBytes = memoryStream.ToArray();

                // Convert the encrypted byte array to a base64 encoded string
                plainText = System.Text.Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }

            // Return the encrypted data as a string
            return plainText;
        }

    
        private static Task OnJwtTokenValidated(TokenValidatedContext context)
        {
            // here we will add any additional claims to our user
            // i.e. Nameidentifier
            if (context.Principal?.Identity is ClaimsIdentity claimsIdentity)
            {
                var dataClaim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == "data");
                if (dataClaim != null && !string.IsNullOrWhiteSpace(dataClaim.Value))
                {
                    try
                    {
                        dynamic userInfo = JObject.Parse(dataClaim.Value);
                        if (userInfo?.user != null)
                        {
                            string userId = userInfo.user.id;
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId, ClaimValueTypes.Integer, dataClaim.Issuer));
                        }
                    }
                    catch (Exception err)
                    {
                        throw new SecurityTokenDecryptionFailedException("Unable to get user object from JWT token", err);
                    }
                }
            }

            return Task.CompletedTask;
        }


        private class SimpleOptions<T> : IOptions<T> where T : class, new()
        {
            public T Value { get; set; }
        }
    }
}
