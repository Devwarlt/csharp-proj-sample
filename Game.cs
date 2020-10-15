using Game.Sample.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace Game.Sample
{
    public sealed class Game
    {
        private static bool _finishGame = false;

        private static void Main()
        {
            var consoleThread = new Thread(() =>
            {
                Console.Read();
                _finishGame = true;
            })
            { IsBackground = true };
            consoleThread.Start();

            var gameThread = new Thread(() =>
            {
                var rng = new Random();
                var totalPlayers = 20;
                var totalEnemies = 17;
                var players = new Player[totalPlayers];
                for (var i = 0; i < players.Length; i++)
                    players[i] = new Player("Vinicius")
                    {
                        AttackLevel = (ushort)rng.Next(30, 75),
                        DefenseLevel = (ushort)rng.Next(10, 30),
                        HP = 100,
                        Experience = 0,
                        Position = new Vector2(
                            rng.Next(-10, 10),
                            rng.Next(-10, 10)
                        )
                    };

                var enemies = new Enemy[totalEnemies];
                for (var i = 0; i < enemies.Length; i++)
                    enemies[i] = new Enemy("Isaac", (ushort)rng.Next(20, 50), (ushort)rng.Next(10, 30))
                    {
                        HP = (uint)rng.Next(100, 150),
                        Experience = (uint)rng.Next(10, 80),
                        Position = new Vector2(
                            rng.Next(-10, 10),
                            rng.Next(-10, 10)
                        )
                    };

                var entities = new List<Entity>();
                entities.AddRange(players);
                entities.AddRange(enemies);

                while (!_finishGame)
                {
                    foreach (var entity in entities.ToList())
                    {
                        entity.Behavior();
                        // entity.ShowCurrentPosition();

                        // MvP
                        if (entity is Enemy)
                        {
                            var enemy = entity as Enemy;
                            var newPlayers = players.ToArray();
                            for (var i = 0; i < newPlayers.Length; i++)
                            {
                                if (newPlayers[i] == null || newPlayers[i].IsDead) continue;
                                if (Vector2.Distance(enemy.Position, newPlayers[i].Position) <= 1f)
                                {
                                    var damage = Math.Abs(enemy.Attack() - newPlayers[i].Defense());
                                    if (damage >= newPlayers[i].HP)
                                    {
                                        newPlayers[i].HP = 0;
                                        newPlayers[i].OnDeath?.Invoke(null, enemy);
                                        entities.Remove(newPlayers[i]);
                                        newPlayers[i] = null;
                                    }
                                    else
                                    {
                                        newPlayers[i].HP -= (uint)damage;
                                        Console.WriteLine(
                                            $"[MvP] Enemy {enemy.Id} dealt {damage} HP on Player {newPlayers[i].Id}!"
                                        );
                                    }
                                    break;
                                }
                            }
                            players = newPlayers;
                        }
                        // PvM
                        else
                        {
                            var player = entity as Player;
                            var newEnemies = enemies.ToArray();
                            for (var i = 0; i < newEnemies.Length; i++)
                            {
                                if (newEnemies[i] == null || newEnemies[i].IsDead) continue;
                                if (Vector2.Distance(newEnemies[i].Position, player.Position) <= 2f)
                                {
                                    var damage = Math.Abs(player.Attack() - newEnemies[i].Defense());
                                    if (damage >= newEnemies[i].HP)
                                    {
                                        newEnemies[i].HP = 0;
                                        newEnemies[i].OnDeath?.Invoke(null, player);
                                        entities.Remove(newEnemies[i]);
                                        newEnemies[i] = null;
                                    }
                                    else
                                    {
                                        newEnemies[i].HP -= (uint)damage;
                                        Console.WriteLine(
                                            $"[PvM] Player {player.Id} dealt {damage} HP on Enemy {newEnemies[i].Id}!"
                                        );
                                    }
                                    break;
                                }
                            }
                            enemies = newEnemies;
                        }
                    }

                    Thread.Sleep(1000);
                }

                var playerSurvivors = entities.Where(_ => _.GetType() == typeof(Player)).Select(_ => (Player)_).ToArray();
                var enemySurvivors = entities.Where(_ => _.GetType() == typeof(Enemy)).Select(_ => (Enemy)_).ToArray();

                Console.WriteLine("\n---\n");
                Console.WriteLine("Survivors:");
                Console.WriteLine($"\t- players: {playerSurvivors.Length}/{totalPlayers}");
                foreach (var player in playerSurvivors)
                    Console.WriteLine(
                        $"\t\t- player -> " +
                        $"id: {player.Id}, " +
                        $"experience: {player.Experience}, " +
                        $"hp: {player.HP}{(player.MaxHP == 0 ? "" : $"/{player.MaxHP}")}, " +
                        $"kills: {player.EnemiesKilledCount}"
                    );
                Console.WriteLine($"\t- enemies: {enemySurvivors.Length}/{totalEnemies}");
                foreach (var enemy in enemySurvivors)
                    Console.WriteLine(
                        $"\t\t- enemy -> " +
                        $"id: {enemy.Id}, " +
                        $"hp: {enemy.HP}{(enemy.MaxHP == 0 ? "" : $"/{enemy.MaxHP}")}, " +
                        $"kills: {enemy.PlayersKilledCount}"
                    );
            });
            gameThread.Start();
        }
    }
}
