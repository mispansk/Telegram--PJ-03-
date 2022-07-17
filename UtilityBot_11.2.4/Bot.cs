using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Hosting;
using UtilityBot_11._2._4.Controllers;
using UtilityBot_11._2._4;

namespace UtilityBot
{
    internal class Bot : BackgroundService
    {

        private ITelegramBotClient _telegramClient;

        // Контроллеры различных видов сообщений
        private InlineKeyboardController _inlineKeyboardController;
        private TextMessageController _textMessageController;
        private DefaultMessageController _defaultMessageController;
        private LenchText _lenchText;
        private SumNumbers _sumNumbers;
        /// <summary>
        /// параметр, который запоминает выбор кнопки (1 или 2 в текущем решении) 
        /// если значение null-кнопка не выбрана и как следствие выдаем чтовведите /start
        /// если при входящем текстовом событии значение 1 или 2 - значит была выбрана соотвествующая кнопка (обрабатываем своими контроллерами) и сразу переходим в начачльный режим бота (выставляем Condition в null)
        /// в будущем можно расширять таким  образом состав кнопок в этом классе и просто добавлять новые контроллеры 
        /// </summary>
        public string Condition; 

        public Bot(ITelegramBotClient telegramClient, 
            InlineKeyboardController inlineKeyboardController,
            TextMessageController textMessageController,
            DefaultMessageController defaultMessageController, 
            LenchText lenchText,
            SumNumbers sumNumbers)
        {
            _telegramClient = telegramClient;
            _inlineKeyboardController = inlineKeyboardController;
            _textMessageController = textMessageController;
            _defaultMessageController = defaultMessageController;
            _lenchText = lenchText;
            _sumNumbers = sumNumbers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _telegramClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions() { AllowedUpdates = { } }, // Здесь выбираем, какие обновления хотим получать. В данном случае разрешены все
                cancellationToken: stoppingToken);

            Console.WriteLine("Бот запущен");
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {          
                //  Обрабатываем нажатия на кнопки  из Telegram Bot API: https://core.telegram.org/bots/api#callbackquery
                if (update.Type == UpdateType.CallbackQuery)
                {
                    await _inlineKeyboardController.Handle(update.CallbackQuery, cancellationToken);
                    Condition = update.CallbackQuery.Data;              
                    return;
                }              

            // Обрабатываем входящие сообщения из Telegram Bot API: https://core.telegram.org/bots/api#message
            if (update.Type == UpdateType.Message)
            {
                switch (update.Message!.Type)
                {
                    case MessageType.Text:
                        {
                            if (Condition == "1")
                            {
                                await _lenchText.HandleUpdateAsync(update, cancellationToken);                             
                            }
                            else
                            if (Condition == "2")
                            {
                                await _sumNumbers.HandleUpdateAsync(update, cancellationToken);                             
                            }
                            else
                            {
                                await _textMessageController.Handle(update.Message, cancellationToken);
                            }

                            Condition = null;
                            return;
                        }
                    default:
                        await _defaultMessageController.Handle(update.Message, cancellationToken);
                        return;
                }               
            }
        }
        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Задаем сообщение об ошибке в зависимости от того, какая именно ошибка произошла
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            // Выводим в консоль информацию об ошибке
            Console.WriteLine(errorMessage);

            // Задержка перед повторным подключением
            Console.WriteLine("Ожидаем 10 секунд перед повторным подключением.");
            Thread.Sleep(10000);

            return Task.CompletedTask;
        }
    }
}