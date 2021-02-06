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
            if (!string.IsNullOrEmpty(settings.ApplicationName))
            {
                await Connection.SetTitleAsync(settings.ApplicationName);
            }
            else
            {
                await Connection.SetTitleAsync(settings.TitleFilter);
            }
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            //Logger.Instance.LogMessage(TracingLevel.INFO, $"Received settings: {payload.Settings}");
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
            //Logger.Instance.LogMessage(TracingLevel.INFO, $"Focus application with settings titlefilter: {settings.TitleFilter}, process name: {settings.ApplicationName}, ");

            (IntPtr main, IntPtr child) = windowfinder.FindWindowWithText(settings.TitleFilter, settings.ApplicationName);
            if (main == IntPtr.Zero)
            {
                if (string.IsNullOrWhiteSpace(settings.TitleFilter))
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"Could not find window with title like {settings.TitleFilter}");
                else
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"Could not find window for process {settings.ApplicationName}");

                await Connection.ShowAlert();
                return;
            }

            //Logger.Instance.LogMessage(TracingLevel.INFO, $"Found main window {main}");
            //if (child != IntPtr.Zero)
            //    Logger.Instance.LogMessage(TracingLevel.INFO, $"Found child window {child}");

            var windowfocuser = new WindowFocuser();
            windowfocuser.SetFocus(main, child, settings.RestoreWindow);
            return;
        }

        private async void Connection_OnSendToPlugin(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.SendToPlugin> e)
        {
            // may do logging here
        }

        #endregion
    }
}