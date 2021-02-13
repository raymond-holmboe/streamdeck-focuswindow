using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BarRaider.SdTools;
using Synkrono.FocusWindow.Util;

namespace Synkrono.FocusWindow.Actions
{
    [PluginActionId("com.synkrono.focuswindow")]
    public class FocusWindowAction : PluginBase
    {
        #region Private Members

        private readonly PluginSettings settings;

        #endregion
        public FocusWindowAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
                this.settings = PluginSettings.CreateDefaultSettings();
            else
                this.settings = payload.Settings.ToObject<PluginSettings>();
            Connection.OnSendToPlugin += Connection_OnSendToPlugin;
            SaveSettings();
        }

        public override void Dispose()
        {
            Connection.OnSendToPlugin -= Connection_OnSendToPlugin;
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Destructor called");
        }

        public async override void KeyPressed(KeyPayload payload)
        {
            await FocusApplication();
            //Logger.Instance.LogMessage(TracingLevel.INFO, "Key was pressed");
        }

        public override void KeyReleased(KeyPayload payload) { }

        public async override void OnTick()
        {
            await Connection.SetTitleAsync(settings.Process);
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Received settings: {payload.Settings}");
            string oldprocess = settings.Process;
            Tools.AutoPopulateSettings(settings, payload.Settings);
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        #region Private Methods


        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        private async Task FocusApplication()
        {
            var windowfinder = new WindowFinder();

            (IntPtr main, IntPtr child) = windowfinder.FindWindowWithText(settings.ChildWindow, settings.Process);
            if (!string.IsNullOrWhiteSpace(settings.ChildWindow) && child == IntPtr.Zero)
            {
                Logger.Instance.LogMessage(TracingLevel.WARN, $"Could not find window with title {settings.ChildWindow}");
                await Connection.ShowAlert();
                return;
            }
            var windowfocuser = new WindowFocuser();
            windowfocuser.SetFocus(main, child, settings.RestoreWindow);
            return;
        }

        private void GetChildWindows()
        {
            if (string.IsNullOrWhiteSpace(settings.Process))
                return;
            var windowfinder = new WindowFinder();
            var childWindows = windowfinder.GetChildWindows(settings.Process);
            childWindows = childWindows.OrderBy(w => w.Title).ToList();
            childWindows.Insert(0, new ChildWindow { Title = "", WindowsHandle = IntPtr.Zero });
            settings.ChildWindows = childWindows;
            //Logger.Instance.LogMessage(TracingLevel.DEBUG, "Found childwindows: " + JsonConvert.SerializeObject(settings.ChildWindows, Formatting.Indented));
            if (settings.ChildWindows.Count > 1 && string.IsNullOrEmpty(settings.ChildWindow))
                settings.ChildWindow = settings.ChildWindows.First().Title;
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Found {settings.ChildWindows.Count} windows");
        }

        private void GetProcesses()
        {
            var processLoader = new ProcessFinder();
            var processnames = processLoader.GetProcessNamesWithMainWindow();
            settings.Processes = processnames.Select(p => new ProcessListItem { Name = p }).OrderBy(p => p.Name).ToList();
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Found {settings.Processes.Count} processes");
        }

        private async void Connection_OnSendToPlugin(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.SendToPlugin> e)
        {
            var payload = e.Event.Payload;
            string prop = payload["property_inspector"]?.ToString().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(prop))
                return;
            Logger.Instance.LogMessage(TracingLevel.INFO, $"{prop} called");
            switch (prop)
            {
                case "getchildwindows":
                    GetChildWindows();
                    await SaveSettings();
                    break;
                case "getprocesses":
                    GetProcesses();
                    await SaveSettings();
                    break;
            }
        }

        #endregion
    }
}