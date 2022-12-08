﻿//Licensed under the Apache License, Version 2.0 (the "License");
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

using System.Collections.Generic;
using System.Threading;

namespace OpenQA.Selenium.Appium.Ws
{
    public interface ICanHandleConnects
    {


        /// <returns> The list of web socket connection handlers.</returns>
        IList<ThreadStart> GetConnectionHandlers();

        /// <summary>
        /// Register a new message handler.
        /// </summary>
        /// <param name="handler"> handler a callback function, which is going to be executed when web socket connection event arrives</param>
        void AddConnectionHandler(ThreadStart handler);

        /// <summary>
        /// Removes existing web socket connection handlers.
        /// </summary>
        void RemoveConnectionHandlers();
    }
}
