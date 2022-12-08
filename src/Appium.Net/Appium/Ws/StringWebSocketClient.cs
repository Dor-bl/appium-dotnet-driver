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
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace OpenQA.Selenium.Appium.Ws
{
    public class StringWebSocketClient : WebSocketClient
    {
        private readonly IList<Action<string>> messagesHandlers = new SynchronizedCollection<Action<string>>();
        private readonly IList<Action<Exception>> errorHandlers = new SynchronizedCollection<Action<Exception>>();
        private readonly IList<ThreadStart> connectHandlers = new SynchronizedCollection<ThreadStart>();
        private readonly IList<ThreadStart> disconnectHandlers = new SynchronizedCollection<ThreadStart>();

        private volatile ClientWebSocket socket;

        public new async Task Connect(Uri endpoint)
        {
            if (socket != null)
            {
                if (endpoint.Equals(this.GetEndpoint()))
                {
                    return;
                }

                RemoveAllHandlers();
                try
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                catch ( IOException e)
                {
                    //igonre
                }
                socket = null;
            }
            await base.Connect(endpoint);
        }

        public void OnOpen()
        {
            this.socket = null;
            foreach (ThreadStart ts in GetConnectionHandlers())
            {
                new Thread(ts).Start();
            }
           // GetConnectionHandlers().ForEach(ts => new Thread(new ThreadStart(ts)).Start());
        }


        public void OnClose() 
        {
            this.socket = null;
            foreach (ThreadStart ts in GetDisconnectionHandlers())
            {
                new Thread(ts).Start();
            }
            //GetDisconnectionHandlers().ForEach(ts => new Thread(new ThreadStart(ts)).Start());
        }

        public void OnError(Exception cause)
        {
            this.socket = null;
            foreach (Action<Exception> errorException in GetErrorHandlers())
            {
                errorException.Invoke(cause);
            }
           // GetErrorHandlers().ForEach(x => x.Invoke(cause));
        }

        public void OnMessage(string message)
        {
            foreach (Action<string> messageHandler in GetMessageHandlers())
            {
                messageHandler.Invoke(message);
            }    
        }

        public override IList<Action<string>> GetMessageHandlers()
        {
            return messagesHandlers;
        }

        public override IList<Action<Exception>> GetErrorHandlers()
        {
            return errorHandlers;
        }

        public override IList<ThreadStart> GetConnectionHandlers()
        {
            return connectHandlers;
        }

        public override IList<ThreadStart> GetDisconnectionHandlers()
        {
            return disconnectHandlers;
        }

        /// <summary>
        /// Remove all the registered handlers.
        /// </summary>
        public void RemoveAllHandlers()
        {
            RemoveMessageHandlers();
            RemoveErrorHandlers();
            RemoveConnectionHandlers();
            RemoveDisconnectionHandlers();
        }
    }

   
}
