using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using VoiceTexterBot.Services;
using System.Threading;


namespace TextControllers
{
    public class TextMessageController
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly IStorage _memoryStorage;

        public TextMessageController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            switch (message.Text)
            {
                case "/start":

                    // Объект, представляющий кноки
                    var buttons = new List<InlineKeyboardButton[]>();
                    buttons.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData($" Длина строки" , $"Длина строки"),
                        InlineKeyboardButton.WithCallbackData($" Сумма цифр" , $"Сумма цифр"),
                        InlineKeyboardButton.WithCallbackData($" Мне ничего не надо",$"Очень жаль")
                    });

                    // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"<b> Наш бот подсчитывает колличество символов или сумму чисел.</b> {Environment.NewLine}" +
                        $"{Environment.NewLine}Выберите что вы хотите.", cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));
                    break;

                default:
                    switch (_memoryStorage.GetSession(message.From.Id).Choice)
                    {
                        case "Длина строки":
                            await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Длина сообщения: {message.Text.Length} знаков", cancellationToken: ct);
                            break;
                        case "Сумма цифр":
                            string[] numbers = message.Text.Split(' ');
                            int sumNumbers = 0;
                            foreach (string str in numbers)
                            {
                                if (int.TryParse(str, out int value)) sumNumbers += value;
                                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Сумма цифр: {sumNumbers} ", cancellationToken: ct);
                            }
                            break;
                        default:
                            await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Введите команду: /start", cancellationToken: ct);
                            break;
                    }
                    break;
            }
        }
    }
}