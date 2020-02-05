using System;

namespace Farmdustry.Entities
{
    public class PlayerList
    {
        private Player[] players = new Player[0];
        private byte playersCount = 0;

        private const float PLAYER_SPEED = 5f;

        /// <summary>
        /// Get the current amount of player entries.
        /// </summary>
        /// <remarks>This is not the amount of players connected to the server.</remarks>
        public byte Count => playersCount;

        /// <summary>
        /// Set a player's velocity.
        /// </summary>
        /// <param name="playerId">The id of the player.</param>
        /// <param name="yVelocity">The y velocity of the player.</param>
        /// <param name="xVelocity">The x velocity of the player.</param>
        public void SetPlayerVelocity(byte playerId, float yVelocity, float xVelocity)
        {
            //Add new player entries if they dont exist yet
            if (playerId >= playersCount)
            {
                playersCount = (byte)(playerId + 1);
                Array.Resize(ref players, playersCount);
            }

            players[playerId].YVelocity = yVelocity;
            players[playerId].XVelocity = xVelocity;
        }

        /// <summary>
        /// Set a player's position and velocity.
        /// </summary>
        /// <param name="playerId">The id of the player.</param>
        /// <param name="y">The x coördinate of the player.</param>
        /// <param name="x">The y coördinate of the player.</param>
        /// <param name="yVelocity">The y velocity of the player.</param>
        /// <param name="xVelocity">The x velocity of the player.</param>
        public void SetPlayerPositionAndVelocity(byte playerId, float y, float x, float yVelocity, float xVelocity)
        {
            //Add new player entries if they dont exist yet
            if (playerId >= playersCount)
            {
                playersCount = (byte)(playerId + 1);
                Array.Resize(ref players, playersCount);
            }

            players[playerId].Y = y;
            players[playerId].X = x;
            players[playerId].YVelocity = yVelocity;
            players[playerId].XVelocity = xVelocity;
        }

        /// <summary>
        /// Get a snapshot of the player's position and velocity.
        /// </summary>
        /// <param name="playerId">The id of the player.</param>
        /// <returns>A copy of the player's position and velocity.</returns>
        public Player GetPlayerSnapshot(byte playerId)
        {
            //Add new player entries if they dont exist yet
            if (playerId >= playersCount)
            {
                playersCount = (byte)(playerId + 1);
                Array.Resize(ref players, playersCount);
            }

            return players[playerId];
        }

        /// <summary>
        /// Move all players according to their velocity and position
        /// </summary>
        /// <param name="deltaTime">The time between the previous update and now.</param>
        public void UpdatePlayers(float deltaTime)
        {
            for (int i = 0; i < playersCount; i++)
            {
                players[i].Y += players[i].YVelocity * deltaTime * PLAYER_SPEED;
                players[i].X += players[i].XVelocity * deltaTime * PLAYER_SPEED;
            }
        }
    }
}
