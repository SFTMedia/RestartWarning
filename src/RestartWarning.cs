using BepInEx;
using HarmonyLib;
using System;
using System.Timers;
using UnityEngine;

namespace RestartWarning
{
    [BepInPlugin("com.bluetigeresw.RestartWarning", "Restart Warning", "1.0.2")]
    public class RestartWarning : BaseUnityPlugin
    {
        private const string MESSAGE = "[RestartWarning] Server will restart in 5 minutes! Saving world...";

        private Timer timer;

        public void Awake()
        {
            // Hook the Chat event to detect when a chat message is sent
            Logger.LogInfo("Loading Restart Warning...");

            // Set up a timer to trigger at a set time every day
            TimeSpan triggerTime = new TimeSpan(3, 55, 0); // 3:55:00 AM
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            TimeSpan delay = (triggerTime > currentTime) ? triggerTime - currentTime : TimeSpan.FromDays(1) - (currentTime - triggerTime);
            timer = new Timer(delay.TotalMilliseconds);
            timer.Elapsed += OnTimerElapsed;
            timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Broadcast a message to all players in the game
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", MESSAGE);

            // Check if the game is running on a dedicated server
            if (ZNet.instance.IsServer())
            {
                // Get a reference to the game console
                var console = Console.instance;

                if (console != null)
                {
                    // Execute the command to save world
                    console.TryRunCommand("save");
                }
                else
                {
                    Debug.LogWarning("Could not find game console");
                }
            }
            else
            {
                Debug.LogWarning("Plugin can only be run on a dedicated server");
            }
            
            // Reset the timer for the next trigger
            timer.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
        }
    }
}
