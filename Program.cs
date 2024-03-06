using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CheerUpBot
{
    internal class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("7089171767:AAHdVQzmYFoVWirRI_3TaSKuvUxzInz39Gk");
        static Random random = new Random();

        static List<string> photos = new List<string>()
        {
            "https://raw.githubusercontent.com/Ulquaza/CheerUpBot/main/bin/Debug/net7.0/pics/capy.jpg",
            "https://raw.githubusercontent.com/Ulquaza/CheerUpBot/main/bin/Debug/net7.0/pics/doggo.jpg",
            "https://raw.githubusercontent.com/Ulquaza/CheerUpBot/main/bin/Debug/net7.0/pics/rabbits.jpg",
        };

        static List<string> advices = new List<string>()
        {
            "Не бойтесь бояться.\nИногда единственное, что сдерживает ваш рост — это то, чего вы больше всего боитесь.",
            "Используйте кризис как возможность.\nКаждая потеря ведет к новой возможности, а каждая трудная ситуация таит в себе потенциал.",
        };

        static List<string> jokes = new List<string>()
        {
            "К Штирлицу постучали. Это был человек в фуфайке и на лыжах\r\n— Фуфлыжник. — Догадался Штирлиц.",
            "Штирлиц попал в глубокую яму и чудом из нее вылез.\r\n«Чудес не бывает», — подумал Штирлиц и на всякий случай залез обратно.",
        };

        static List<string> supports = new List<string>()
        {
            "«Бездействие порождает беспокойство и страх. Действие — уверенность и смелость. Если ты хочешь победить страх, не сиди дома и не думай об этом. Встань и действуй».\nМэг Джей",
            "«Сила происходит не от побед. Силу порождает борьба. Когда вы проходите через трудности и решаете не сдаваться — это и есть сила».\nАрнольд Шварценеггер",
        };

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message)
            {
                if (update.Message is null) return;
                if (update.Message.Text is null) return;

                var message = update.Message;
                var messageText = message.Text;
                var chatId = message.Chat.Id;
                Console.WriteLine($"Received a '{messageText}' message from {message.Chat.Username} in chat {chatId}");

                if (messageText.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Приветствую", replyMarkup: GetKeyboard());
                }
                else
                if (messageText.ToLower() == "pic")
                {
                    Message sentPhoto = await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: InputFile.FromUri(photos[random.Next(0, photos.Count)]),
                            cancellationToken: cancellationToken);
                    return;
                }
            }
            if (update.Type == UpdateType.CallbackQuery)
            {
                if (update.CallbackQuery is null) return;
                if (update.CallbackQuery.Data is null) return;
                if (update.CallbackQuery.Message is null) return;
                CallbackQuery? callbackQuery = update.CallbackQuery;
                string data = callbackQuery.Data;
                long chatId = callbackQuery.Message.Chat.Id;
                string text = String.Empty;

                switch (data)
                {
                    case "photo":
                        await botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: InputFile.FromUri(photos[random.Next(0, photos.Count)]),
                            replyMarkup: GetKeyboard(),
                            cancellationToken: cancellationToken);
                        return;
                    case "advice":
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: advices[random.Next(0, advices.Count)],
                            replyMarkup: GetKeyboard(),
                            cancellationToken: cancellationToken);
                        return;
                    case "joke":
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: jokes[random.Next(0, jokes.Count)],
                            replyMarkup: GetKeyboard(),
                            cancellationToken: cancellationToken);
                        return;
                    case "support":
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: supports[random.Next(0, supports.Count)],
                            replyMarkup: GetKeyboard(),
                            cancellationToken: cancellationToken);
                        return;
                    default:
                        break;
                }
            }
        }

        public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Запущен бот {bot.GetMeAsync().Result.FirstName}");

            var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            bot.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token);
            Console.ReadLine();
        }

        static InlineKeyboardMarkup GetKeyboard()
        {
            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>
            {
                new InlineKeyboardButton[] {
                    InlineKeyboardButton.WithCallbackData(text: "Фото", callbackData: "photo"),
                    InlineKeyboardButton.WithCallbackData(text: "Анекдот", callbackData: "joke")
                },
                new InlineKeyboardButton[] {
                    InlineKeyboardButton.WithCallbackData(text: "Совет", callbackData: "advice"),
                    InlineKeyboardButton.WithCallbackData(text: "Поддержка", callbackData: "support")
                }
            };

            return buttons.ToArray();
        }
    }
}