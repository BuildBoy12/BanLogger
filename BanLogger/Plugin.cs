// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BanLogger
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// The main plugin class.
    /// </summary>
    public class Plugin : Plugin<Config>
    {
        private EventHandlers eventHandlers;

        /// <inheritdoc />
        public override string Author => "Build";

        /// <inheritdoc />
        public override Version RequiredExiledVersion { get; } = new Version(5, 0, 0);

        /// <summary>
        /// Gets an instance of the <see cref="BanLogger.WebhookController"/> class.
        /// </summary>
        public WebhookController WebhookController { get; private set; }

        /// <inheritdoc />
        public override void OnEnabled()
        {
            if (string.IsNullOrEmpty(Config.WebhookUrl))
            {
                Log.Error("The webhook url cannot be empty! BanLogger will not be loaded.");
                return;
            }

            WebhookController = new WebhookController(this);

            eventHandlers = new EventHandlers(this);
            Exiled.Events.Handlers.Player.Banned += eventHandlers.OnBanned;
            Exiled.Events.Handlers.Player.Banning += eventHandlers.OnBanning;
            Exiled.Events.Handlers.Player.Kicking += eventHandlers.OnKicking;

            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Banned -= eventHandlers.OnBanned;
            Exiled.Events.Handlers.Player.Banning -= eventHandlers.OnBanning;
            Exiled.Events.Handlers.Player.Kicking -= eventHandlers.OnKicking;
            eventHandlers = null;

            WebhookController?.Dispose();
            WebhookController = null;

            base.OnDisabled();
        }
    }
}