﻿namespace Secretary.Telegram;

public delegate Task MessageReceive(BotMessage message);

public interface ITelegramClient
{
    event MessageReceive OnMessage;
    
    DateTime LastCheckTime { get; }

    Task RunDriver();
    
    Task SendMessage(long chatId, string message);
    Task SendDocument(long chatId, string path, string fileName);
    Task SendMessageWithKeyBoard(long chatId, string message, string[] choices);
    Task SendMessageWithKeyBoard(long chatId, string message, string[][] choices);
    Task SendSticker(long chatId, string stickerId);
}