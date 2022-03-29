// -----------------------------------------------------------------------
// <copyright file="EventHandlers.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BanLogger
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using BanLogger.Models;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    /// <summary>
    /// Handles events derived from <see cref="Exiled.Events.Handlers"/>.
    /// </summary>
    public class EventHandlers
    {
        private readonly Plugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlers"/> class.
        /// </summary>
        /// <param name="plugin">An instance of the <see cref="Plugin"/> class.</param>
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnBanned(BannedEventArgs)"/>
        public void OnBanned(BannedEventArgs ev)
        {
            if (ev.Type != BanHandler.BanType.UserId || ev.Details.OriginalName != "Unknown - offline ban")
                return;

            string name = "Unknown";
            if (ev.Details.Id.Contains("@steam") && !string.IsNullOrEmpty(plugin.Config.SteamApiKey))
                name = GetUserName(ev.Details.Id);

            long ticks = (long)TimeSpan.FromTicks(ev.Details.Expires - ev.Details.IssuanceTime).TotalSeconds;
            plugin.WebhookController.SendMessage(new BanInfo(ev.Issuer, name, ev.Details.Id, ev.Details.Reason, ticks));
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnBanning(BanningEventArgs)"/>
        public void OnBanning(BanningEventArgs ev)
        {
            plugin.WebhookController.SendMessage(new BanInfo(ev.Issuer, ev.Target, ev.Reason, ev.Duration));
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnKicking(KickingEventArgs)"/>
        public void OnKicking(KickingEventArgs ev)
        {
            plugin.WebhookController.SendMessage(new BanInfo(ev.Issuer, ev.Target, ev.Reason, 0));
        }

        private string GetUserName(string userid)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={plugin.Config.SteamApiKey}&steamids={userid}");
                httpWebRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                    return Regex.Match(result, @"\x22personaname\x22:\x22(.+?)\x22").Groups[1].Value;
                }
            }
            catch
            {
                Log.Error("An error has occured while contacting steam servers. This could be due to them being down or usage of an invalid steam API key.");
            }

            return "Unknown";
        }
    }
}