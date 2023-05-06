using BepInEx;
using HarmonyLib;
using System;
using System.Timers;
using UnityEngine;

namespace RestartWarning
{
    [BepInPlugin("com.bluetigeresw.RestartWarning", "Restart Warning", "1.0.3")]
    public class RestartWarning : BaseUnityPlugin
    {
        private const string MESSAGE_1 = "[RestartWarning] Server will restart in 15 minutes!";
        private const string MESSAGE_2 = "[RestartWarning] Server will restart in 5 minutes! Saving world...";

        private Timer timer1;
        private Timer timer2;

        public void Awake()
        {
            // Hook the Chat event to detect when a chat message is sent
            Logger.LogInfo("Loading Restart Warning...");

            // Set up a timer to trigger at a set time every day for MESSAGE_1
            TimeSpan triggerTime1 = new TimeSpan(3, 55, 0); // 3:55:00 AM
            TimeSpan currentTime1 = DateTime.Now.TimeOfDay;
            TimeSpan delay1 = (triggerTime1 > currentTime1) ? triggerTime1 - currentTime1 : TimeSpan.FromDays(1) - (currentTime1 - triggerTime1);
            timer1 = new Timer(delay1.TotalMilliseconds);
            timer1.Elapsed += OnTimerElapsed1;
            timer1.Start();

            // Set up a timer to trigger at a set time every day for MESSAGE_2
            TimeSpan triggerTime2 = new TimeSpan(3, 45, 0); // 3:45:00 AM
            TimeSpan currentTime2 = DateTime.Now.TimeOfDay;
            TimeSpan delay2 = (triggerTime2 > currentTime2) ? triggerTime2 - currentTime2 : TimeSpan.FromDays(1) - (currentTime2 - triggerTime2);
            timer2 = new Timer(delay2.TotalMilliseconds);
            timer2.Elapsed += OnTimerElapsed2;
            timer2.Start();
        }

        // On timer1 elapsed
        private void OnTimerElapsed1(object sender, ElapsedEventArgs e)
        {
            // Broadcast a message to all players in the game
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", MESSAGE_1);

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
            timer1.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
        }

        // On timer2 elapsed
        private void OnTimerElapsed2(object sender, ElapsedEventArgs e)
        {
            // Broadcast a message to all players in the game
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", MESSAGE_1);

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
            timer2.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
        }
    }
}
