// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BanLogger
{
    using System.ComponentModel;
    using Exiled.API.Interfaces;

    /// <inheritdoc />
    public sealed class Config : IConfig
    {
        /// <inheritdoc />
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the url of the webhook.
        /// </summary>
        public string WebhookUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the title of the webhook.
        /// </summary>
        public string WebhookTitle { get; set; } = "Punishment Logger";

        /// <summary>
        /// Gets or sets the steam api key to get the nickname of obanned users.
        /// </summary>
        [Description("Steam Api key to get the nickname of obanned users. Get your api key in https://steamcommunity.com/dev/apikey")]
        public string SteamApiKey { get; set; } = "00000000000000000000000000000000";
    }
}