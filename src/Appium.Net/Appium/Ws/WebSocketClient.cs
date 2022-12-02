using System;
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

        public Uri getEndpoint()
        {
            return this._endpoint;
        }

        public async Task Connect(Uri endpoint)
        {
            try
            {
                using (var webSockerContainer = new ClientWebSocket())
                {
                    await webSockerContainer.ConnectAsync(endpoint, CancellationToken.None);
                    SetEndpoint(endpoint);
                }  
            }
            catch (Exception ex)
            {
                throw new WebDriverException(ex.ToString());
            }
        }
    }
}
