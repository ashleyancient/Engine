using System.Windows.Input;
using Engine.Commands;
using Engine.Services;

namespace Engine.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly ThemeService _theme = ThemeService.Instance;

    private bool _isDarkTheme = true;
    private double _fontSize = 13;
    private bool _compactMode;
    private bool _notifSound = true;
    private bool _notifVibrate;
    private bool _notifPreview = true;
    private bool _doNotDisturb;
    private string _privacyPhone = "Контакты";
    private string _privacyOnline = "Все";
    private string _privacyAvatar = "Все";
    private bool _twoFactor;
    private double _storageUsedGb = 2.3;
    private double _storageTotalGb = 15.0;
    private string _selectedAccent = "Фиолетовый";

    public SettingsViewModel()
    {
        ToggleThemeCommand = new RelayCommand(_ => { IsDarkTheme = !IsDarkTheme; });
        SetAccentCommand = new RelayCommand(p => { if (p is string s) ApplyAccent(s); });
        ClearCacheCommand = new RelayCommand(_ => { StorageUsedGb = 0.1; });
        SetPrivacyPhoneCommand = new RelayCommand(p => PrivacyPhone = p?.ToString() ?? "Все");
        SetPrivacyOnlineCommand = new RelayCommand(p => PrivacyOnline = p?.ToString() ?? "Все");
        SetPrivacyAvatarCommand = new RelayCommand(p => PrivacyAvatar = p?.ToString() ?? "Все");
    }

    public ICommand ToggleThemeCommand { get; }
    public ICommand SetAccentCommand { get; }
    public ICommand ClearCacheCommand { get; }
    public ICommand SetPrivacyPhoneCommand { get; }
    public ICommand SetPrivacyOnlineCommand { get; }
    public ICommand SetPrivacyAvatarCommand { get; }

    public bool IsDarkTheme { get => _isDarkTheme; set { if (SetProperty(ref _isDarkTheme, value)) { _theme.Apply(value ? ThemeMode.Dark : ThemeMode.Light); OnPropertyChanged(nameof(ThemeName)); } } }
    public string ThemeName => IsDarkTheme ? "Тёмная тема" : "Светлая тема";
    public double FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }
    public bool CompactMode { get => _compactMode; set => SetProperty(ref _compactMode, value); }
    public bool NotifSound { get => _notifSound; set => SetProperty(ref _notifSound, value); }
    public bool NotifVibrate { get => _notifVibrate; set => SetProperty(ref _notifVibrate, value); }
    public bool NotifPreview { get => _notifPreview; set => SetProperty(ref _notifPreview, value); }
    public bool DoNotDisturb { get => _doNotDisturb; set => SetProperty(ref _doNotDisturb, value); }
    public string PrivacyPhone { get => _privacyPhone; set => SetProperty(ref _privacyPhone, value); }
    public string PrivacyOnline { get => _privacyOnline; set => SetProperty(ref _privacyOnline, value); }
    public string PrivacyAvatar { get => _privacyAvatar; set => SetProperty(ref _privacyAvatar, value); }
    public bool TwoFactor { get => _twoFactor; set => SetProperty(ref _twoFactor, value); }
    public double StorageUsedGb { get => _storageUsedGb; set { if (SetProperty(ref _storageUsedGb, value)) { OnPropertyChanged(nameof(StorageProgress)); OnPropertyChanged(nameof(StorageText)); } } }
    public double StorageTotalGb { get => _storageTotalGb; set => SetProperty(ref _storageTotalGb, value); }
    public double StorageProgress => StorageTotalGb <= 0 ? 0 : StorageUsedGb / StorageTotalGb * 100.0;
    public string StorageText => $"{StorageUsedGb:0.#} ГБ из {StorageTotalGb:0.#} ГБ";
    public string SelectedAccent { get => _selectedAccent; set => SetProperty(ref _selectedAccent, value); }

    private void ApplyAccent(string name)
    {
        SelectedAccent = name;
        switch (name)
        {
            case "Фиолетовый": _theme.ApplyAccent("#7B61FF", "#00D4FF"); break;
            case "Синий":   _theme.ApplyAccent("#1E90FF", "#00D4FF"); break;
            case "Зелёный": _theme.ApplyAccent("#11998E", "#38EF7D"); break;
            case "Красный":  _theme.ApplyAccent("#FF4757", "#FF7F50"); break;
            case "Оранжевый": _theme.ApplyAccent("#F7971E", "#FFD200"); break;
            case "Розовый":  _theme.ApplyAccent("#F857A6", "#FF5858"); break;
        }
    }
}
