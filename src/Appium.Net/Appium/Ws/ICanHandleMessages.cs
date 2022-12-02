using System;
using System.Collections.Generic;
using System.Text;

namespace OpenQA.Selenium.Appium.Ws
{
    internal interface ICanHandleMessages<T>
    {

        /**
        * @return The list of web socket message handlers.
        */
        List<Action<Exception>> GetMessageHandlers();

        void AddMessageHandler(Action<Exception> handler)
        {
            GetMessageHandlers().Add(handler);
        }
    }
}
