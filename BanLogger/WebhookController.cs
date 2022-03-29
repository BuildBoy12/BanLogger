// -----------------------------------------------------------------------
// <copyright file="WebhookController.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BanLogger
{
    using System;
    using BanLogger.Models;
    using DSharp4Webhook.Core;
    using DSharp4Webhook.Core.Constructor;
    using Exiled.API.Features;

    /// <summary>
    /// Handles the sending of messages via a webhook.
    /// </summary>
    public class WebhookController : IDisposable
    {
        private static readonly EmbedBuilder EmbedBuilder = ConstructorProvider.GetEmbedBuilder();
        private static readonly EmbedFieldBuilder FieldBuilder = ConstructorProvider.GetEmbedFieldBuilder();
        private static readonly MessageBuilder MessageBuilder = ConstructorProvider.GetMessageBuilder();
        private readonly Plugin plugin;
        private readonly IWebhook webhook;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebhookController"/> class.
        /// </summary>
        /// <param name="plugin">An instance of the <see cref="Plugin"/> class.</param>
        public WebhookController(Plugin plugin)
        {
            this.plugin = plugin;
            webhook = WebhookProvider.CreateStaticWebhook(plugin.Config.WebhookUrl);
        }

        /// <summary>
        /// Sends a message via the <see cref="webhook"/>.
        /// </summary>
        /// <param name="banInfo">The information to send.</param>
        public void SendMessage(BanInfo banInfo)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WebhookController));

            webhook.SendMessage(PrepareMessage(banInfo).Build()).Queue((result, isSuccessful) =>
            {
                if (!isSuccessful)
                    Log.Warn("Failed to send ban information.");
            });
        }

        /// <inheritdoc />
        public void Dispose()
        {
            isDisposed = true;
            webhook?.Dispose();
        }

        private static string TimeFormatter(long duration)
        {
            if (duration == 0)
                return "Kick";

            TimeSpan timespan = new TimeSpan(0, 0, (int)duration);
            string finalFormat = string.Empty;

            if (timespan.TotalDays >= 365)
                finalFormat += $" {timespan.TotalDays / 365}y";
            else if (timespan.TotalDays >= 30)
                finalFormat += $" {timespan.TotalDays / 30}mon";
            else if (timespan.TotalDays >= 1)
                finalFormat += $" {timespan.TotalDays}d";
            else if (timespan.Hours > 0)
                finalFormat += $" {timespan.Hours}h";
            if (timespan.Minutes > 0)
                finalFormat += $" {timespan.Minutes}min";
            if (timespan.Seconds > 0)
                finalFormat += $" {timespan.Seconds}s";

            return finalFormat.Trim();
        }

        private static string CodeLine(string message) => $"```{message}```";

        private MessageBuilder PrepareMessage(BanInfo banInfo)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(WebhookController));

            EmbedBuilder.Reset();
            FieldBuilder.Reset();
            MessageBuilder.Reset();

            FieldBuilder.Inline = false;

            FieldBuilder.Name = "User Punished";
            FieldBuilder.Value = CodeLine(banInfo.BannedName + " " + $"({banInfo.BannedId})");
            EmbedBuilder.AddField(FieldBuilder.Build());

            FieldBuilder.Name = "Issuing Staff";
            FieldBuilder.Value = CodeLine(banInfo.IssuerName + " " + $"({banInfo.IssuerId})");
            EmbedBuilder.AddField(FieldBuilder.Build());

            FieldBuilder.Name = "Reason";
            FieldBuilder.Value = CodeLine(banInfo.Reason);
            EmbedBuilder.AddField(FieldBuilder.Build());

            FieldBuilder.Name = "Ban Duration";
            FieldBuilder.Value = CodeLine(TimeFormatter(banInfo.Duration));
            EmbedBuilder.AddField(FieldBuilder.Build());

            EmbedBuilder.Title = plugin.Config.WebhookTitle;
            EmbedBuilder.Timestamp = DateTimeOffset.UtcNow;
            EmbedBuilder.Color = (uint)DSharp4Webhook.Util.ColorUtil.FromHex("#D10E11");

            MessageBuilder.AddEmbed(EmbedBuilder.Build());

            return MessageBuilder;
        }
    }
}