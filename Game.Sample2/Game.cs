using System;
using System.Threading;

namespace Game.Sample2
{
    public sealed class Game
    {
        private static void Main()
        {
            // configure player
            var player = new Player(
                actionKey: ConsoleKey.Q,
                cancellationKey: ConsoleKey.X
            );
            player.ConfigSpell(
                spell: () => Console.WriteLine("Casting fireball spell!"),
                spellCastTime: TimeSpan.FromSeconds(2)
            );

            // configure player input handler
            var handler = new PlayerInputHandler(
                cancellationKey: ConsoleKey.End
            );
            handler.Players.Add(player);
            handler.Start();

            // keep running PIH core processing
            while (handler.IsRunning)
                Thread.Sleep(50);

            Environment.Exit(0);
        }
    }
}
