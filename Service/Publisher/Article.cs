using System.Globalization;
using System.Security.Cryptography;
using Config;
using Domain;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Utilites;
using File = System.IO.File;

namespace Service.Publisher;

public class Article
{
    private readonly IBuilderConfig _config;
    private readonly PostRepository _postRepository;
    private readonly CancellationToken _ctx;
    private readonly string _projectRootPath = @"../../../..";
    private readonly ITelegramBotClient _tbot;

    public Article(IBuilderConfig config, CancellationToken ctx)
    {
        _config = config;
        _postRepository = new PostRepository(_config.ConnectionString);
        _ctx = ctx;
        _tbot = new TelegramBotClient(_config.AccessToken);
    }

    public async Task<string> PrepareContentAsync(Post post)
    {
        var templatePath = Path.Combine(_projectRootPath, "Resources/PostTemplate.html");
        var tpl = await File.ReadAllTextAsync(templatePath, _ctx);
        var (size, capacity) = HumanBytes.Calculate(post.Size);
        return String.Format(
            new CultureInfo("en-US"),
            tpl,
            post.Title, // 0
            post.Year, // 1
            post.Developer, // 2
            post.Version, // 3
            post.Language, // 4
            post.Tags, // 5
            post.Slug, // 6
            post.Slug, // 7
            size, // 8
            capacity // 9
        );
    }

    public async Task<bool> Release()
    {
        Post? post = await _postRepository.GetPostRandomAsync();
        if (post is null)
        {
            return false;
        }

        var content = await PrepareContentAsync(post);
        var chatId = new ChatId(_config.ChatId);
        var albumMediaGroup = new IAlbumInputMedia[]
        {
            new InputMediaPhoto($"https://what2play.xyz/content/{post.Slug}/images/screenshot_0")
            {
                Caption = content,
                ParseMode = ParseMode.Html,
            },
            new InputMediaPhoto($"https://what2play.xyz/content/{post.Slug}/images/screenshot_1"),
            new InputMediaPhoto($"https://what2play.xyz/content/{post.Slug}/images/screenshot_2"),
            new InputMediaPhoto($"https://what2play.xyz/content/{post.Slug}/images/screenshot_3"),
        };

        try
        {
            var result = await _tbot.SendMediaGroupAsync(_config.ChatId, albumMediaGroup, cancellationToken: _ctx);
        }
        catch (Exception? ex)
        {
            var guid = Guid.NewGuid().ToString();
            var fileName = String.Format($"what2play_{guid}");
            var exPath = Path.Combine(Path.GetTempPath(), fileName);
            
            using (var writer = new StreamWriter(exPath))
            {
                writer.WriteLine("-----------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString(CultureInfo.CurrentCulture));
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }

            return false;
        }
        
        // set flag true
        var rowUpdated = await _postRepository.SetFlagTrueAsync(post);

        return true;
    }
}