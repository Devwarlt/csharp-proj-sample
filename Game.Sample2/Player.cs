using System;
using System.Threading;
using System.Threading.Tasks;

namespace Game.Sample2
{
    public sealed class Player
    {
        private readonly ConsoleKey _actionKey;
        private readonly ConsoleKey _cancellationKey;

        private Action _spell;
        private TimeSpan _spellCastTime;
        private Task _spellTask;
        private CancellationTokenSource _cts;

        public Player(ConsoleKey actionKey, ConsoleKey cancellationKey)
        {
            _actionKey = actionKey;
            _cancellationKey = cancellationKey;
            _spellTask = null;
            _cts = new CancellationTokenSource();
        }

        public void ConfigSpell(Action spell, TimeSpan spellCastTime)
        {
            Console.WriteLine("Configuring player spell...");

            _spell = spell;
            _spellCastTime = spellCastTime;
        }

        public void CastSpell(ConsoleKey key)
        {
            if (_spell == null || key != _actionKey)
                return;

            if (_spellTask != null)
            {
                Console.WriteLine("Player spell task is already running...");
                return;
            }

            Console.WriteLine("Preparing to cast player spell!");

            _spellTask = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(_spellCastTime);

                    _cts.Token.ThrowIfCancellationRequested();

                    Console.WriteLine("Executing player spell action!");

                    _spell?.Invoke();
                    _spellTask = null;
                    _cts = new CancellationTokenSource();
                }
                catch (OperationCanceledException) { Console.WriteLine("Player spell task has been cancelled!"); }
                catch (Exception e) { Console.WriteLine(e); }
            }, _cts.Token);
        }

        public void RequestSpellCancellation(ConsoleKey key)
        {
            if (key == _cancellationKey && _spellTask != null)
            {
                Console.WriteLine("Cancellating player spell task!");

                _cts.Cancel();
            }
        }
    }
}
