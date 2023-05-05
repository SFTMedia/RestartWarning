using BepInEx;
using HarmonyLib;
using System;
using UnityEngine;

[BepInPlugin("com.example.valheimplugin", "Valheim Plugin", "1.0.0")]
public class ValheimPlugin : BaseUnityPlugin
{
    private const string MESSAGE = "Don't forget to save the world!";

    private Timer timer;

    public void Awake()
    {
        // Hook the On.ChatMessage event to detect when a chat message is sent
        BepInEx.Logging.Logger.LogInfo("Initializing Valheim Plugin");
        On.ChatMessage += OnChatMessage;
        
        // Set up a timer to trigger at a set time every day
        TimeSpan triggerTime = new TimeSpan(12, 0, 0); // 12:00:00 PM
        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        TimeSpan delay = (triggerTime > currentTime) ? triggerTime - currentTime : TimeSpan.FromDays(1) - (currentTime - triggerTime);
        timer = new Timer(delay.TotalMilliseconds);
        timer.Elapsed += OnTimerElapsed;
        timer.Start();
    }

    private void OnChatMessage(Chat obj)
    {
        // Check if the chat message contains a specific keyword
        if (obj.message.Contains("save world"))
        {
            ZNet.instance.SaveWorld();
        }
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        // Broadcast a message to all players in the game
        ZRpc.Broadcast(null, "ChatMessage", new object[] { MESSAGE });
        
        // Save the world
        ZNet.instance.SaveWorld();
        
        // Reset the timer for the next trigger
        timer.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
    }
}
