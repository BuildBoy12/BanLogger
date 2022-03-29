// -----------------------------------------------------------------------
// <copyright file="BanInfo.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace BanLogger.Models
{
    using Exiled.API.Features;

    /// <summary>
    /// Represents information relevant to a ban.
    /// </summary>
    public readonly struct BanInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BanInfo"/> struct.
        /// </summary>
        /// <param name="issuer">The player that issued the ban.</param>
        /// <param name="target">The player that was banned.</param>
        /// <param name="reason">The reason for the ban.</param>
        /// <param name="duration">The duration of the ban.</param>
        public BanInfo(Player issuer, Player target, string reason, long duration)
        {
            IssuerName = issuer?.Nickname ?? "Server Console";
            IssuerId = issuer?.UserId ?? "Server Console";
            BannedName = target.Nickname;
            BannedId = target.UserId;
            Reason = reason;
            Duration = duration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BanInfo"/> struct.
        /// </summary>
        /// <param name="issuer">The player that issued the ban.</param>
        /// <param name="targetName">The name of the player that was banned.</param>
        /// <param name="targetId">The id of the player that was banned.</param>
        /// <param name="reason">The reason for the ban.</param>
        /// <param name="duration">The duration of the ban.</param>
        public BanInfo(Player issuer, string targetName, string targetId, string reason, long duration)
        {
            IssuerName = issuer?.Nickname ?? "Server Console";
            IssuerId = issuer?.UserId ?? "Server Console";
            BannedName = targetName;
            BannedId = targetId;
            Reason = reason;
            Duration = duration;
        }

        /// <summary>
        /// Gets the issuer's name.
        /// </summary>
        public string IssuerName { get; }

        /// <summary>
        /// Gets the issuer's id.
        /// </summary>
        public string IssuerId { get; }

        /// <summary>
        /// Gets the banned user's name.
        /// </summary>
        public string BannedName { get; }

        /// <summary>
        /// Gets the banned user's id.
        /// </summary>
        public string BannedId { get; }

        /// <summary>
        /// Gets the reason for the ban.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Gets the duration of the ban.
        /// </summary>
        public long Duration { get; }
    }
}