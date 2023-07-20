//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//See the NOTICE file distributed with this work for additional
//information regarding copyright ownership.
//You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OpenQA.Selenium.Appium.Ws
{
    public abstract class WebSocketClient
    {
        private Uri _endpoint;


        private void SetEndpoint(Uri endpoint)
        {
            this._endpoint = endpoint;
        }

        public Uri GetEndpoint()
        {
            return this._endpoint;
        }

        /// <summary>
        /// Connects web socket client.
        /// </summary>
        /// <param name="endpoint">endpoint The full address of an endpoint to connect to.
        /// Usually starts with 'ws://'.</param>
        /// <returns></returns>
        /// <exception cref="WebDriverException"></exception>
        public async Task Connect(Uri endpoint)
        {

            try
            {
                var webSocketContainer = new ClientWebSocket();
                {
                    await webSocketContainer.ConnectAsync(endpoint, CancellationToken.None);
                    SetEndpoint(endpoint);
                }
            }
            catch (Exception ex)
            {
                throw new WebDriverException(ex.ToString());
            }
        }

        #region WebSocket handlers implementation

        public abstract IList<ThreadStart> GetConnectionHandlers();

        public void AddConnectionHandler(ThreadStart handler)
        {
            GetConnectionHandlers().Add(handler);
        }

        public void RemoveConnectionHandlers()
        {
            GetConnectionHandlers().Clear();
        }

        public abstract IList<ThreadStart> GetDisconnectionHandlers();

        public void AddDisconnectionHandler(ThreadStart handler)
        {
            GetDisconnectionHandlers().Add(handler);
        }

        public void RemoveDisconnectionHandlers()
        {
            GetConnectionHandlers().Clear();
        }

        public abstract IList<Action<Exception>> GetErrorHandlers();

        public void AddErrorHandler(Action<Exception> handler)
        {
            GetErrorHandlers().Add(handler);
        }

        public void RemoveErrorHandlers()
        {
            GetErrorHandlers().Clear();
        }

        public abstract IList<Action<string>> GetMessageHandlers();

        public void AddMessageHandler(Action<string> handler)
        {
            GetMessageHandlers().Add(handler);
        }

        public void RemoveMessageHandlers()
        {
            GetMessageHandlers().Clear();
        }

        #endregion WebSocket handlers implementation
    }
}
