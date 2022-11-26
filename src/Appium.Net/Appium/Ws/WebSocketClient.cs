using Autofac.Integration.Web;
using System;


namespace OpenQA.Selenium.Appium.Ws
{
    public abstract class WebSocketClient
    {
        private Uri _endpoint;

        private void setEndpoint(Uri endpoint)
        {
            this._endpoint = endpoint;
        }

        public Uri getEndpoint()
        {
            return this._endpoint;
        }

        public void connect(Uri endpoint)
        {
            try
            {
                ContainerProvider containerProvider = new ContainerProvider();
                var container = containerProvider.ApplicationContainer;
            }
        }
    }
}
