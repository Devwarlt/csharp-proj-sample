using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Game.Sample2
{
    public sealed class PlayerInputHandler
    {
        public List<Player> Players;

        public bool IsRunning { get; private set; }

        private readonly Thread _inputThread;
        private readonly ConsoleKey _cancellationKey;

        public PlayerInputHandler(ConsoleKey cancellationKey)
        {
            Players = new List<Player>();

            _inputThread = new Thread(HandlePlayerInputs)
            { IsBackground = true };
            _cancellationKey = cancellationKey;
        }

        public void Start()
        {
            IsRunning = true;

            Console.WriteLine("Starting PIH handler...");

            _inputThread.Start();
        }

        private void HandlePlayerInputs()
        {
            while (true)
            {
                var keyInfo = Console.ReadKey();
                var key = keyInfo.Key;
                if (key == _cancellationKey)
                    break;

                foreach (var player in Players.ToList())
                {
                    player.CastSpell(key);
                    player.RequestSpellCancellation(key);
                }
            }

            _inputThread.Interrupt();

            IsRunning = false;
        }
    }
}
