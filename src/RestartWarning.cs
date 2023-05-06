using BepInEx;
using HarmonyLib;
using System;
using System.Timers;
using UnityEngine;

namespace RestartWarning
{
    [BepInPlugin("com.bluetigeresw.RestartWarning", "Restart Warning", "1.0.0")]
    public class RestartWarning : BaseUnityPlugin
    {
        private const string MESSAGE = "[RestartWarning] Server will restart in 5 minutes! Saving world...";

        private Timer timer;

        public void Awake()
        {
            // Hook the Chat event to detect when a chat message is sent
            Logger.LogInfo("Loading Restart Warning...");
            ZRoutedRpc.instance.Register<string, Vector3>("ChatMessage", RPC_ChatMessage);
            ZRoutedRpc.instance.Register<string, Vector3>("SystemMessage", RPC_ChatMessage);

            // Set up a timer to trigger at a set time every day
            TimeSpan triggerTime = new TimeSpan(3, 55, 0); // 3:55:00 AM
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            TimeSpan delay = (triggerTime > currentTime) ? triggerTime - currentTime : TimeSpan.FromDays(1) - (currentTime - triggerTime);
            timer = new Timer(delay.TotalMilliseconds);
            timer.Elapsed += OnTimerElapsed;
            timer.Start();
        }

        private void SaveWorld()
        {
            // Define method for saving the world
            ZNet.instance.SaveWorld();
        }
        private void RPC_ChatMessage(long sender, string message, Vector3 position)
        {
            // Check if the chat message contains a specific keyword
            if (message.Contains("save world"))
            {
                if (ZNet.instance != null)
                {
                    SaveWorld();
                }
                else
                {
                    Logger.LogWarning("[RestartWarning] ZNet instance is null, unable to save world.");
                }
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Broadcast a message to all players in the game
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", MESSAGE);

            // Save the world
            SaveWorld();

            // Reset the timer for the next trigger
            timer.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
        }
    }
}
