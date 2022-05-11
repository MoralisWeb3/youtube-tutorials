using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Exceptions;
using Moralis.Platform.Services.Models;
using Moralis.Platform.Objects;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Services.ClientServices
{
    /// <summary>
    /// The command runner for all SDK operations that need to interact with the targeted deployment of Moralis Server.
    /// </summary>
    public class MoralisCommandRunner<TUser> : IMoralisCommandRunner where TUser : MoralisUser
    {
        IWebClient WebClient { get; }

        IInstallationService InstallationService { get; }

        IMetadataService MetadataService { get; }

        IServerConnectionData ServerConnectionData { get; }

        Lazy<IUserService<TUser>> UserService { get; }

        IWebClient GetWebClient() => WebClient;

        /// <summary>
        /// Creates a new Moralis SDK command runner.
        /// </summary>
        /// <param name="webClient">The <see cref="IWebClient"/> implementation instance to use.</param>
        /// <param name="installationController">The <see cref="IInstallationService"/> implementation instance to use.</param>
        public MoralisCommandRunner(IWebClient webClient, IInstallationService installationController, IMetadataService metadataController, IServerConnectionData serverConnectionData, Lazy<IUserService<TUser>> userController)
        {
            WebClient = webClient;
            InstallationService = installationController;
            MetadataService = metadataController;
            ServerConnectionData = serverConnectionData;
            UserService = userController;

            
        }

        /// <summary>
        /// Runs a specified <see cref="MoralisCommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="MoralisCommand"/> to run.</param>
        /// <param name="uploadProgress">An <see cref="IProgress{MoralisUploadProgressEventArgs}"/> instance to push upload progress data to.</param>
        /// <param name="downloadProgress">An <see cref="IProgress{MoralisDownloadProgressEventArgs}"/> instance to push download progress data to.</param>
        /// <param name="cancellationToken">An asynchronous operation cancellation token that dictates if and when the operation should be cancelled.</param>
        /// <returns>Tuple<HttpStatusCode, string></returns>
        //public Task<Tuple<HttpStatusCode, string>> RunCommandAsync(MoralisCommand command, IProgress<IDataTransferLevel> uploadProgress = null, IProgress<IDataTransferLevel> downloadProgress = null, CancellationToken cancellationToken = default) => PrepareCommand(command).ContinueWith(commandTask => GetWebClient().ExecuteAsync(commandTask.Result, uploadProgress, downloadProgress, cancellationToken).OnSuccess(task =>
        public async Task<Tuple<HttpStatusCode, string>> RunCommandAsync(MoralisCommand command, IProgress<IDataTransferLevel> uploadProgress = null, IProgress<IDataTransferLevel> downloadProgress = null, CancellationToken cancellationToken = default) // => PrepareCommand(command).ContinueWith(commandTask => GetWebClient().ExecuteAsync(commandTask.Result, uploadProgress, downloadProgress, cancellationToken).OnSuccess(task =>
        {
            MoralisCommand cmd = await PrepareCommand(command);
            
            Tuple<HttpStatusCode, string> cmdResult = await GetWebClient().ExecuteAsync(cmd, uploadProgress, downloadProgress, cancellationToken); //, uploadProgress, downloadProgress, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            string content = cmdResult.Item2;
            int responseCode = (int)cmdResult.Item1;


            if (responseCode >= 500)
            {
                throw new MoralisFailureException(MoralisFailureException.ErrorCode.InternalServerError, content);
            }

            // The Moralis server returns POCO saved dates as an object. Convert to ISO datetime string.
            string adjustedData = content.AdjustJsonForParseDate();

            // Remove Results wrapper.
            if (adjustedData.StartsWith("{\"results\":"))
            {
                adjustedData = adjustedData.Substring(11, adjustedData.Length - 12);
            }
            else if (adjustedData.StartsWith("{\"result\":"))
            {
                adjustedData = adjustedData.Substring(10, adjustedData.Length - 11);
            }

            Tuple<HttpStatusCode, string> newResponse = new Tuple<HttpStatusCode, string>(cmdResult.Item1, adjustedData);
            
            return newResponse;
        }//)).Unwrap();

        Task<MoralisCommand> PrepareCommand(MoralisCommand command)
        {
            MoralisCommand newCommand = new MoralisCommand(command)
            {
                Resource = ServerConnectionData.ServerURI
            };

            Task<MoralisCommand> installationIdFetchTask = InstallationService.GetAsync().ContinueWith(task =>
            {
                lock (newCommand.Headers)
                {
                    newCommand.Headers.Add(new KeyValuePair<string, string>("X-Parse-Installation-Id", task.Result.ToString()));
                }

                return newCommand;
            });

            lock (newCommand.Headers)
            {
                KeyValuePair<string, string> appId = new KeyValuePair<string, string>("X-Parse-Application-Id", ServerConnectionData.ApplicationID);

                if (!newCommand.Headers.Contains(appId))
                {
                    newCommand.Headers.Add(appId);
                }

                KeyValuePair<string, string> clientVersion = new KeyValuePair<string, string>("X-Parse-Client-Version", MoralisService<TUser>.Version.ToString());

                if (!newCommand.Headers.Contains(clientVersion))
                {
                    newCommand.Headers.Add(clientVersion);
                }

                if (ServerConnectionData.Headers != null)
                {
                    foreach (KeyValuePair<string, string> header in ServerConnectionData.Headers)
                    {
                        newCommand.Headers.Add(header);
                    }
                }

                if (!String.IsNullOrEmpty(MetadataService.HostManifestData.Version))
                {
                    newCommand.Headers.Add(new KeyValuePair<string, string>("X-Parse-App-Build-Version", MetadataService.HostManifestData.Version));
                }

                if (!String.IsNullOrEmpty(MetadataService.HostManifestData.ShortVersion))
                {
                    newCommand.Headers.Add(new KeyValuePair<string, string>("X-Parse-App-Display-Version", MetadataService.HostManifestData.ShortVersion));
                }

                if (!String.IsNullOrEmpty(MetadataService.EnvironmentData.OSVersion))
                {
                    newCommand.Headers.Add(new KeyValuePair<string, string>("X-Parse-OS-Version", MetadataService.EnvironmentData.OSVersion));
                }

                if (!String.IsNullOrEmpty(ServerConnectionData.MasterKey))
                {
                    newCommand.Headers.Add(new KeyValuePair<string, string>("X-Parse-Master-Key", ServerConnectionData.MasterKey));
                }
                else
                {
                    newCommand.Headers.Add(new KeyValuePair<string, string>("X-Parse-Windows-Key", ServerConnectionData.Key));
                }

                if (UserService.Value.RevocableSessionEnabled)
                {
                    newCommand.Headers.Add(new KeyValuePair<string, string>("X-Parse-Revocable-Session", "1"));
                }
            }

            return installationIdFetchTask;
        }
    }
}
