using Appium.Net.Integration.Tests.helpers;
using NUnit.Framework;
using OpenQA.Selenium.Appium.Android;
using System;
using System.Threading;

namespace Appium.Net.Integration.Tests.Android
{
    [TestFixture]
    public class AndroidLogcatListenerTest
    {
        private AndroidDriver _driver;
        private readonly SemaphoreSlim messageSemaphore = new SemaphoreSlim(1);
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(20);

        [OneTimeSetUp]
        public void BeforeAll()
        {
            var capabilities = Caps.GetAndroidUIAutomatorCaps(Apps.Get("androidApiDemos"));
            capabilities.AddAdditionalAppiumOption("newCommandTimeout", 80);
            var serverUri = Env.ServerIsRemote() ? AppiumServers.RemoteServerUri : AppiumServers.LocalServiceUri;
            _driver = new AndroidDriver(serverUri, capabilities, Env.InitTimeoutSec);
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
                _driver.StartLogcatBroadcast("127.0.0.1", 5037);
                messageSemaphore.Wait();
                _driver.BackgroundApp(TimeSpan.FromSeconds(1));
                _driver.LaunchApp();
                // Assert.True(_timeout.TotalMilliseconds, messageSemaphore.WaitOne(_timeout.Milliseconds));
                // Assert.That(_timeout.TotalMilliseconds, messageSemaphore.WaitOne(_timeout.Milliseconds));
                Assert.IsTrue(messageSemaphore.Wait(_timeout),$"Didn't receive any log message after {_timeout:h\\:mm\\:ss} timeout");
                //Assert.IsTrue(_timeout.TotalMilliseconds.Equals(messageSemaphore.Wait(_timeout.Milliseconds)),string.Format("Didn't recive any log message after %s timeout"));
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
