using Appium.Net.Integration.Tests.helpers;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appium.Net.Integration.Tests.Android
{
    [TestFixture]
    public class AndroidLogcatListenerTest
    {
        private AndroidDriver _driver;
        private readonly Semaphore messageSemaphore = new Semaphore(0, 1);
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(15);

        [OneTimeSetUp]
        public void BeforeAll()
        {
            var capabilities = Caps.GetAndroidUIAutomatorCaps(Apps.Get("androidApiDemos"));
            var serverUri = Env.ServerIsRemote() ? AppiumServers.RemoteServerUri : AppiumServers.LocalServiceUri;
            _driver = new AndroidDriver(serverUri, capabilities, Env.ImplicitTimeoutSec);
            _driver.Manage().Timeouts().ImplicitWait = Env.ImplicitTimeoutSec;
        }

        [OneTimeTearDown]
        public void AfterAll()
        {
            _driver?.Quit();
            if (!Env.ServerIsRemote())
            {
                AppiumServers.StopLocalService();
            }
        }

        [Test]
        public void VerifyLogcatListenerCanBeAssigned()
        {
            _driver.AddLogcatMessagesListener(msg => messageSemaphore.Release());
            _driver.AddLogcatConnectionListener(() => Console.WriteLine("Connected to the web socket"));
            _driver.AddLogcatDisconnectionListener(() => Console.WriteLine("Disconnected from the web socket"));
            _driver.AddLogcatErrorsListener(StackTrace => StackTrace.ToString());
            try
            {
                _driver.StartLogcatBroadcast();
                messageSemaphore.WaitOne();
                _driver.BackgroundApp(TimeSpan.FromSeconds(1));
               // Assert.True(_timeout.TotalMilliseconds, messageSemaphore.WaitOne(_timeout.Milliseconds));
               // Assert.That(_timeout.TotalMilliseconds, messageSemaphore.WaitOne(_timeout.Milliseconds));
                Assert.IsTrue(_timeout.TotalMilliseconds.Equals(messageSemaphore.WaitOne(_timeout.Milliseconds)),string.Format("Didn't recive any log message after %s timeout"));
            }
            catch (ThreadInterruptedException e)
            {
                throw new InvalidOperationException(e.ToString());
            }
            finally
            {
                messageSemaphore.Release();
                _driver.StopLogcatBroadcast();
            }

        }

    }
}
