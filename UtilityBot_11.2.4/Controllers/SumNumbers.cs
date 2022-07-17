using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace UtilityBot_11._2._4.Controllers
{
    /// <summary>
    /// Класс "Подсчет суммы"
    /// </summary>
    public class SumNumbers
    {
        private readonly ITelegramBotClient _telegramClient;

        public SumNumbers(ITelegramBotClient telegramBotClient)
        {
            _telegramClient = telegramBotClient;
        }
        /// <summary>
        /// Метод для подсчета суммы введеных чисел
        /// </summary>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task HandleUpdateAsync( Update update, CancellationToken cancellationToken)
        {
            string str2 = ""; // промежуточная строка, для составления числа
            int number; // преобразованное число (из последоательности символов)
            int sum = 0; // пополняемая сумма чисел
            
            //  Обрабатываем нажатия на кнопки  из Telegram Bot API: https://core.telegram.org/bots/api#callbackquery
            if (update.Type == UpdateType.CallbackQuery)
            {
                await _telegramClient.SendTextMessageAsync(update.CallbackQuery.From.Id, $"Данный тип сообщений не поддерживается. Пожалуйста введите числа через пробел.", cancellationToken: cancellationToken);
                return;
            }

            // Обрабатываем входящие сообщения из Telegram Bot API: https://core.telegram.org/bots/api#message
            if (update.Type == UpdateType.Message)
            {
                foreach (char h in update.Message.Text) // перебираем посимвольно текст
                {
                    if (char.IsDigit(h) == false && char.IsWhiteSpace(h) == false) // если символ не пробел или не цифра, то выводим сообщение об ошибке и выходим
                    {                       
                        await _telegramClient.SendTextMessageAsync(update.Message.From.Id, $"Некооректрый воод , введине числа через пробел.", cancellationToken: cancellationToken);
                        return;
                    }
                    if (char.IsDigit(h))
                    {
                        str2 += h; // составляем число из символов
                    }
                    else
                    {
                        if (str2 != "")  // если встречаем пробел и str2 не пустое, то превращаем символы в число и сразу увеличиваем сумму 
                        {
                            number = Convert.ToInt32(str2);
                            sum += number;
                            str2 = "";
                        }
                    }
                }
                if (str2 != "") // обработка последнего символа
                {
                    number = Convert.ToInt32(str2);
                    sum += number;
                }
                await _telegramClient.SendTextMessageAsync(update.Message.From.Id, $"Сумма чисел: {sum} ", cancellationToken: cancellationToken);
                return;              
            }
        }
    }
}
