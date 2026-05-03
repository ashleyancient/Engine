using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Engine.Models;

namespace Engine.Services;

public class MockDataService
{
    private static MockDataService? _instance;
    public static MockDataService Instance => _instance ??= new MockDataService();

    public ObservableCollection<User> Users { get; } = new();
    public ObservableCollection<Chat> Chats { get; } = new();
    public ObservableCollection<Channel> Channels { get; } = new();
    public ObservableCollection<Story> Stories { get; } = new();
    public ObservableCollection<Notification> Notifications { get; } = new();
    public List<(string Email, string Password)> Accounts { get; } = new();

    public User CurrentUser { get; private set; }

    private static readonly Random _rng = new(20260501);

    private static readonly (string First, string Last)[] _names =
    {
        ("Александр", "Иванов"), ("Мария", "Смирнова"), ("Дмитрий", "Кузнецов"),
        ("Анна", "Попова"), ("Сергей", "Волков"), ("Елена", "Соколова"),
        ("Никита", "Лебедев"), ("Ольга", "Козлова"), ("Кирилл", "Морозов"),
        ("Юлия", "Новикова"), ("Артём", "Петров"), ("Наташа", "Орлова"),
        ("Максим", "Михайлов"), ("Виктория", "Фёдорова"), ("Павел", "Сергеев"),
        ("Ксения", "Александрова"), ("Иван", "Васильев"), ("Дарья", "Никитина"),
        ("Michael", "Brown"), ("Sarah", "Wilson")
    };

    private static readonly (string Start, string End)[] _gradients =
    {
        ("#7B61FF", "#00D4FF"), ("#FF6B6B", "#FFD93D"), ("#6BCB77", "#00D4FF"),
        ("#FF9671", "#FFC75F"), ("#845EC2", "#D65DB1"), ("#00C9A7", "#4D8076"),
        ("#FF4757", "#FF7F50"), ("#1E90FF", "#7B61FF"), ("#3DDC97", "#00D4FF"),
        ("#FF6FB5", "#7B61FF"), ("#FFB347", "#FF4757"), ("#43E97B", "#38F9D7"),
        ("#5B86E5", "#36D1DC"), ("#F857A6", "#FF5858"), ("#11998E", "#38EF7D"),
        ("#FC466B", "#3F5EFB"), ("#00B4DB", "#0083B0"), ("#F7971E", "#FFD200"),
        ("#7F00FF", "#E100FF"), ("#43CEA2", "#185A9D")
    };

    private MockDataService()
    {
        Accounts.Add(("admin@nexus.com", "password123"));
        Accounts.Add(("user@nexus.com", "qwerty"));

        CurrentUser = new User
        {
            Id = 0, FirstName = "Администратор", LastName = "Системы",
            Username = "admin", Email = "admin@nexus.com", Password = "password123",
            Bio = "Connect beyond limits", PhoneNumber = "+7 (900) 000-00-01",
            AvatarColorStart = "#7B61FF", AvatarColorEnd = "#00D4FF",
            IsOnline = true, LastSeen = DateTime.Now
        };

        BuildUsers();
        BuildChats();
        BuildChannels();
        BuildStories();
        BuildNotifications();
    }

    public bool TryAuthenticate(string emailOrUsername, string password, out User? user)
    {
        var match = Accounts.FirstOrDefault(a =>
            string.Equals(a.Email, emailOrUsername, StringComparison.OrdinalIgnoreCase) && a.Password == password);
        if (!string.IsNullOrEmpty(match.Email)) { user = CurrentUser; return true; }
        var u = Users.FirstOrDefault(x =>
            (string.Equals(x.Email, emailOrUsername, StringComparison.OrdinalIgnoreCase) ||
             string.Equals(x.Username, emailOrUsername, StringComparison.OrdinalIgnoreCase))
            && x.Password == password);
        if (u is not null) { user = u; return true; }
        user = null; return false;
    }

    public bool IsUsernameTaken(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return true;
        return Users.Any(x => string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase));
    }

    public void RegisterUser(User user) { Users.Add(user); Accounts.Add((user.Email, user.Password)); }

    private void BuildUsers()
    {
        for (int i = 0; i < _names.Length; i++)
        {
            var (first, last) = _names[i];
            var grad = _gradients[i % _gradients.Length];
            Users.Add(new User
            {
                Id = i + 1, FirstName = first, LastName = last,
                Username = Translit(first).ToLower() + "_" + Translit(last).ToLower(),
                Email = Translit(first).ToLower() + "." + Translit(last).ToLower() + "@nexus.com",
                Password = "qwerty", Bio = GetBio(i),
                PhoneNumber = $"+7 (9{_rng.Next(10, 99)}) {_rng.Next(100, 999)}-{_rng.Next(10, 99)}-{_rng.Next(10, 99)}",
                AvatarColorStart = grad.Start, AvatarColorEnd = grad.End,
                IsOnline = i % 3 == 0,
                LastSeen = DateTime.Now.AddMinutes(-_rng.Next(2, 60 * 24))
            });
        }
    }

    private static string GetBio(int i) => i switch
    {
        0 => "Дизайнер интерфейсов и архитектор систем",
        1 => "Художник, путешественник, мечтатель",
        2 => "Разработчик распределённых систем",
        3 => "Маркетолог и автор статей о трендах",
        4 => "Инженер по машинному обучению",
        5 => "Финансист, инвестор, лектор",
        6 => "Музыкант и звукорежиссёр",
        7 => "Преподаватель английского языка",
        8 => "Фронтенд-разработчик и UI-инженер",
        9 => "Журналист, редактор, копирайтер",
        _ => "Участник сообщества Engine"
    };

    private static string Translit(string input)
    {
        var map = new Dictionary<char, string>
        {
            ['а']="a",['б']="b",['в']="v",['г']="g",['д']="d",
            ['е']="e",['ё']="e",['ж']="zh",['з']="z",['и']="i",
            ['й']="j",['к']="k",['л']="l",['м']="m",['н']="n",
            ['о']="o",['п']="p",['р']="r",['с']="s",['т']="t",
            ['у']="u",['ф']="f",['х']="h",['ц']="c",['ч']="ch",
            ['ш']="sh",['щ']="sch",['ъ']="",['ы']="y",['ь']="",
            ['э']="e",['ю']="yu",['я']="ya"
        };
        var result = new System.Text.StringBuilder();
        foreach (var raw in input)
        {
            var ch = char.ToLower(raw);
            if (map.TryGetValue(ch, out var v)) result.Append(v);
            else if (char.IsLetterOrDigit(ch)) result.Append(ch);
        }
        return result.ToString();
    }

    private void BuildChats()
    {
        var saved = new Chat { Id=1, Type=ChatType.Saved, Title="Избранное",
            AvatarColorStart="#7B61FF", AvatarColorEnd="#00D4FF", IsPinned=true,
            Description="Личные заметки и сохранённые сообщения" };
        AppendMessage(saved, CurrentUser, "Список идей для нового проекта", DateTime.Now.AddHours(-2));
        AppendMessage(saved, CurrentUser, "Ссылка на отчёт за квартал", DateTime.Now.AddHours(-1));
        Chats.Add(saved);

        int chatId = 2;
        foreach (var i in new[] { 0, 1, 2, 3, 4, 5, 6 })
        {
            var partner = Users[i];
            var chat = new Chat { Id=chatId++, Type=ChatType.Direct, Title=partner.FullName,
                Counterpart=partner, AvatarColorStart=partner.AvatarColorStart, AvatarColorEnd=partner.AvatarColorEnd,
                IsPinned=i==0, IsMuted=i==4, UnreadCount=i%3==1?_rng.Next(1,12):0 };
            chat.Members.Add(partner); chat.Members.Add(CurrentUser);
            FillDialog(chat, partner);
            Chats.Add(chat);
        }

        var groupNames = new[]
        {
            ("Команда продукта", "Совместная работа над релизом", new[]{0,1,2,3,8,12}),
            ("Дизайн-чат", "Идеи, ревю, мудборды", new[]{1,5,9,13}),
            ("Друзья", "Просто болтаем", new[]{6,7,10,11,14,17}),
            ("Книжный клуб", "Что мы читаем в этом месяце", new[]{2,4,8,15})
        };
        foreach (var (name, desc, members) in groupNames)
        {
            var grad = _gradients[chatId % _gradients.Length];
            var group = new Chat { Id=chatId++, Type=ChatType.Group, Title=name, Description=desc,
                AvatarColorStart=grad.Start, AvatarColorEnd=grad.End, UnreadCount=_rng.Next(0,6) };
            foreach (var m in members) group.Members.Add(Users[m]);
            group.Members.Add(CurrentUser);
            FillGroup(group);
            Chats.Add(group);
        }

        foreach (var (name, desc) in new[]
        {
            ("Tech News Daily", "Новости технологий каждый день"),
            ("Финансовый дайджест", "Итоги торгов и аналитика рынков"),
            ("Научный вестник", "Открытия и публикации недели")
        })
        {
            var grad = _gradients[chatId % _gradients.Length];
            var channel = new Chat { Id=chatId++, Type=ChatType.Channel, Title=name, Description=desc,
                AvatarColorStart=grad.Start, AvatarColorEnd=grad.End,
                Subscribers=_rng.Next(50_000,3_500_000), UnreadCount=_rng.Next(0,4) };
            FillChannel(channel);
            Chats.Add(channel);
        }

        var bot = new Chat { Id=chatId, Type=ChatType.Bot, Title="Engine Помощник",
            Description="Виртуальный помощник Engine", AvatarColorStart="#11998E", AvatarColorEnd="#38EF7D" };
        FillBot(bot);
        Chats.Add(bot);
    }

    private void FillDialog(Chat chat, User other)
    {
        var phrases = new[]
        {
            "Привет! Как дела?", "Только что закончил презентацию.",
            "Получилось интересно, посмотришь?", "Конечно, отправляй.",
            "Сейчас будет несколько минут на ревю.",
            "Спасибо. Отдохнём в выходные?", "Давай. Я свободен в субботу.",
            "Согласен по поводу макета.", "Скинь, пожалуйста, ссылку.",
            "Документ прикрепил, посмотри последний раздел.",
            "Хорошо, занесу правки в план."
        };
        var start = DateTime.Now.AddDays(-2).AddHours(-_rng.Next(0, 6));
        for (int i = 0; i < 18; i++)
        {
            var sender = i % 2 == 0 ? other : CurrentUser;
            AppendMessage(chat, sender, phrases[_rng.Next(phrases.Length)], start.AddMinutes(i * _rng.Next(7, 25)));
        }
        if (_rng.Next(2) == 0)
            chat.Messages.Add(new Message { Id=chat.Messages.Count+1, ChatId=chat.Id, Sender=other,
                IsOutgoing=false, FileName="presentation.pdf", FileSize="2.4 МБ",
                Type=MessageType.File, Timestamp=DateTime.Now.AddHours(-3), Status=MessageStatus.Read });
        chat.Messages.Add(new Message { Id=chat.Messages.Count+1, ChatId=chat.Id, Sender=CurrentUser,
            IsOutgoing=true, VoiceDuration=TimeSpan.FromSeconds(_rng.Next(5,60)),
            Type=MessageType.Voice, Timestamp=DateTime.Now.AddMinutes(-30), Status=MessageStatus.Delivered });
    }

    private void FillGroup(Chat chat)
    {
        var phrases = new[]
        {
            "Коллеги, успели посмотреть макет?", "Да, по большей части хорошо.",
            "Мне нравится верхняя часть, но кнопка слишком тёмная.",
            "Давайте обсудим на дейли в 11:00.", "Я подготовлю презентацию к завтрашнему утру.",
            "Спасибо за оперативную работу.", "Сегодня выложил последний билд, проверьте.",
            "Подключаюсь, открываю.", "Всё, что обсуждали, на доске задач.", "До встречи завтра."
        };
        var members = chat.Members.ToList();
        var start = DateTime.Now.AddDays(-1).AddHours(-_rng.Next(2, 8));
        chat.Messages.Insert(0, new Message { Id=0, ChatId=chat.Id, IsOutgoing=false,
            Type=MessageType.System, Text=$"{CurrentUser.FullName} создал(а) группу «{chat.Title}»",
            Timestamp=start.AddMinutes(-5), Status=MessageStatus.Read });
        for (int i = 0; i < 22; i++)
        {
            var sender = members[_rng.Next(members.Count)];
            AppendMessage(chat, sender, phrases[_rng.Next(phrases.Length)], start.AddMinutes(i * _rng.Next(5, 20)), inGroup: true);
        }
    }

    private void FillChannel(Chat chat)
    {
        var phrases = new[]
        {
            "Главное событие дня: запуск нового сервиса.",
            "Аналитика рынка за прошлую неделю.",
            "Подборка лучших материалов недели.",
            "Эксперты делятся мнениями о новом регулировании.",
            "Открыта регистрация на ежегодную конференцию.",
            "Опубликовано исследование по теме года.", "Свежий выпуск подкаста уже доступен."
        };
        var start = DateTime.Now.AddDays(-3);
        for (int i = 0; i < 12; i++)
            chat.Messages.Add(new Message { Id=i+1, ChatId=chat.Id, IsOutgoing=false,
                Text=phrases[_rng.Next(phrases.Length)], Timestamp=start.AddHours(i*4),
                Status=MessageStatus.Read, Type=MessageType.Text });
    }

    private void FillBot(Chat chat)
    {
        chat.Messages.Add(new Message { Id=1, ChatId=chat.Id, IsOutgoing=false,
            Text="Здравствуйте! Я помощник Engine. Чем я могу быть полезен сегодня?",
            Timestamp=DateTime.Now.AddDays(-1), Status=MessageStatus.Read });
        chat.Messages.Add(new Message { Id=2, ChatId=chat.Id, IsOutgoing=true, Sender=CurrentUser,
            Text="Покажи список доступных команд.",
            Timestamp=DateTime.Now.AddDays(-1).AddMinutes(1), Status=MessageStatus.Read });
        chat.Messages.Add(new Message { Id=3, ChatId=chat.Id, IsOutgoing=false,
            Text="/start — приветствие\n/help — справка\n/settings — настройки уведомлений\n/feedback — обратная связь",
            Timestamp=DateTime.Now.AddDays(-1).AddMinutes(2), Status=MessageStatus.Read });
    }

    private void AppendMessage(Chat chat, User sender, string text, DateTime when, bool inGroup = false)
    {
        chat.Messages.Add(new Message
        {
            Id=chat.Messages.Count+1, ChatId=chat.Id, Sender=sender,
            IsOutgoing=sender.Id==CurrentUser.Id, Text=text, Timestamp=when,
            Type=MessageType.Text, Status=MessageStatus.Read, IsGroupContext=inGroup
        });
    }

    private void BuildChannels()
    {
        var data = new (string Title, string User, string Desc, int Subs, string Cat)[]
        {
            ("Tech News",          "@technews",    "Главные технологические новости и обзоры.",   1_200_000, "Технологии"),
            ("Crypto Today",       "@crypto_today","Криптовалюты, блокчейн, аналитика.",            890_000, "Финансы"),
            ("Engine Science",     "@science_hub", "Самые интересные открытия науки.",       2_100_000, "Наука"),
            ("Daily Notes",        "@daily_notes", "Подборка лучших публикаций дня.",       5_400_000, "Разное"),
            ("Finance Pro",        "@financepro",  "Профессиональная аналитика рынков.",      340_000, "Финансы"),
            ("GameZone",           "@gamezone",    "Новости игровой индустрии и обзоры.",    1_700_000, "Игры"),
            ("Music Vibes",        "@musicvibes",  "Свежая музыка, плейлисты и интервью.",      980_000, "Музыка"),
            ("Sports Live",        "@sportslive",  "Спортивные события в прямом эфире.",   3_200_000, "Спорт")
        };
        for (int i = 0; i < data.Length; i++)
        {
            var d = data[i]; var grad = _gradients[i % _gradients.Length];
            Channels.Add(new Channel { Id=i+1, Title=d.Title, Username=d.User, Description=d.Desc,
                Subscribers=d.Subs, Category=d.Cat, AvatarColorStart=grad.Start, AvatarColorEnd=grad.End,
                IsSubscribed=i<3 });
        }
    }

    private void BuildStories()
    {
        for (int i = 0; i < 10 && i < Users.Count; i++)
        {
            var grad = _gradients[(i + 3) % _gradients.Length];
            Stories.Add(new Story
            {
                Id=i+1, Author=Users[i], PublishedAt=DateTime.Now.AddHours(-_rng.Next(1,22)),
                Caption = i switch { 0=>"Вид из окна офиса утром", 1=>"Прогулка по парку",
                    2=>"Новый проект в работе", 3=>"Чашка кофе и план на день", _=>"Момент дня" },
                GradientStart=grad.Start, GradientEnd=grad.End,
                PlaceholderText="Engine", IsViewed=i>5
            });
        }
    }

    private void BuildNotifications()
    {
        Notifications.Add(new Notification { Id=1, Kind=NotificationKind.NewMessage, FromUser=Users[0],
            Title=Users[0].FullName, Body="Прислал(а) новое сообщение", Timestamp=DateTime.Now.AddMinutes(-3), ChatId=2 });
        Notifications.Add(new Notification { Id=2, Kind=NotificationKind.Mention, FromUser=Users[2],
            Title="Команда продукта", Body=$"{Users[2].FullName} упомянул(а) вас", Timestamp=DateTime.Now.AddMinutes(-40) });
        Notifications.Add(new Notification { Id=3, Kind=NotificationKind.ContactRequest, FromUser=Users[5],
            Title=Users[5].FullName, Body="Хочет добавить вас в контакты", Timestamp=DateTime.Now.AddHours(-3) });
        Notifications.Add(new Notification { Id=4, Kind=NotificationKind.GroupInvite, FromUser=Users[7],
            Title="Книжный клуб", Body=$"{Users[7].FullName} приглашает вас в группу", Timestamp=DateTime.Now.AddDays(-1) });
        Notifications.Add(new Notification { Id=5, Kind=NotificationKind.System,
            Title="Engine", Body="Ваш профиль был успешно обновлён", Timestamp=DateTime.Now.AddDays(-2) });
    }
}
