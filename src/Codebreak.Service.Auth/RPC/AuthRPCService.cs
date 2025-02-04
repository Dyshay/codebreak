﻿using Codebreak.Framework.Configuration;
using Codebreak.RPC.Protocol;
using Codebreak.RPC.Service;

namespace Codebreak.Service.Auth.RPC
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AuthRPCService : AbstractRpcService<AuthRPCService, AuthRPCServiceClient, AuthMessageBuilder>
    {
        /// <summary>
        /// 
        /// </summary>
        [Configurable("RPCServiceIP")]
        public static string RPCServiceIP = "127.0.0.1";

        /// <summary>
        /// 
        /// </summary>
        [Configurable("RPCServicePort")]
        public static int RPCServicePort = 4321;

        /// <summary>
        /// 
        /// </summary>
        public AuthRPCService()
        {
            base.RegisterHandler((int)MessageIdEnum.WORLD_TO_AUTH_CREDENTIAL, HandleAuthentification);
            base.RegisterHandler((int)MessageIdEnum.WORLD_TO_AUTH_STATE_UPDATE,  HandleGameStateUpdate);
            base.RegisterHandler((int)MessageIdEnum.WORLD_TO_AUTH_ID_UPDATE, HandleGameIdUpdate);
            base.RegisterHandler((int)MessageIdEnum.WORLD_TO_AUTH_ACCOUNT_DISCONNECTED, HandleGameAccountDisconnected);
            base.RegisterHandler((int)MessageIdEnum.WORLD_TO_AUTH_ACCOUNT_CONNECTED_LIST, HandleAccountConnectedLists);
        }

        /// <summary>
        /// 
        /// </summary>
        public new void Start()
        {
            base.Start(RPCServiceIP, RPCServicePort);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        protected override void OnRPCClientConnected(AuthRPCServiceClient client)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        protected override void OnRPCClientDisconnected(AuthRPCServiceClient client)
        {
            if (client.AuthState != AuthStateEnum.SUCCESS)
                return;

            if (client.GameId != -1)
                AuthService.Instance.DeleteWorld(client.GameId);

            Logger.Warn(string.Format("AuthServiceRPC [{0}][{1}] Disconnected", client.Ip, client.GameId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        protected override void OnMessageReceived(AuthRPCServiceClient client, AbstractRcpMessage message)
        {
            Logger.Debug("AuthServiceRPC " + (MessageIdEnum)message.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        private void HandleAuthentification(AuthRPCServiceClient client, AbstractRcpMessage message)
        {
            if (client.AuthState != AuthStateEnum.NEGOTIATING)
                return;

            var result = AuthResultEnum.FAILED;
            var authMessage = (AuthentificationMessage)message;

            if (authMessage.Password == "smarken")
            {
                client.AuthState = AuthStateEnum.SUCCESS;
                result = AuthResultEnum.SUCCESS;

                client.RemoteIp = authMessage.RemoteIp;
                Logger.Info(string.Format("AuthServiceRPC [{0}] Authed sucessfully", client.Ip));
            }
            
            client.Send(new AuthentificationResult(result));                       
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        private void HandleGameIdUpdate(AuthRPCServiceClient client, AbstractRcpMessage message)
        {
            if (client.AuthState != AuthStateEnum.SUCCESS)
                return;

            var gameIdUpdateMessage = (IdUpdateMessage)message;

            AuthService.Instance.RegisterWorld(gameIdUpdateMessage.GameId, client);

            Logger.Info(string.Format("AuthServiceRPC [{0}] GameId updated to [{1}]", client.Ip, gameIdUpdateMessage.GameId));

            client.GameId = gameIdUpdateMessage.GameId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        private void HandleGameStateUpdate(AuthRPCServiceClient client, AbstractRcpMessage message)
        {
            if (client.AuthState != AuthStateEnum.SUCCESS)
                return;

            var state = ((StateUpdateMessage)message).State;

            Logger.Info(string.Format("AuthServiceRPC [{0}][{1}] GameState updated to {2}", client.Ip, client.GameId, state));

            client.GameState = state;

            AuthService.Instance.RefreshWorldList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        private void HandleGameAccountDisconnected(AuthRPCServiceClient client, AbstractRcpMessage message)
        {
            if (client.AuthState != AuthStateEnum.SUCCESS)
                return;

            var accountId = ((AccountDisconnected)message).AccountId;
            
            Logger.Info(string.Format("AuthServiceRPC [{0}][{1}] GameAccount disconnected accountId={2}", client.Ip, client.GameId, accountId));

            AuthService.Instance.AddMessage(() => client.Players.Remove(accountId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        private void HandleAccountConnectedLists(AuthRPCServiceClient client, AbstractRcpMessage message)
        {
            if (client.AuthState != AuthStateEnum.SUCCESS)
                return;

            var connectedList = (AccountConnectedList)message;

            Logger.Info(string.Format("AuthServiceRPC [{0}][{1}] GameAccount connected list, playerCount={2}", client.Ip, client.GameId, connectedList.ConnectedAccounts.Count));

            AuthService.Instance.AddMessage(() => client.Players.AddRange(connectedList.ConnectedAccounts));
        }
    }
}
