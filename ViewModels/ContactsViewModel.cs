using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Engine.Commands;
using Engine.Models;
using Engine.Services;

namespace Engine.ViewModels;

public class ContactsViewModel : BaseViewModel
{
    private readonly MockDataService _data = MockDataService.Instance;
    private string _searchText = string.Empty;
    private User? _selectedContact;

    public ObservableCollection<User> AllContacts => _data.Users;
    public ObservableCollection<User> OnlineContacts { get; } = new();
    public ObservableCollection<User> SortedContacts { get; } = new();

    public ContactsViewModel()
    {
        OpenProfileCommand = new RelayCommand(p => SelectedContact = p as User);
        CloseProfileCommand = new RelayCommand(_ => SelectedContact = null);
        AddContactCommand = new RelayCommand(_ => { });
        StartChatCommand = new RelayCommand(_ => { });
        BlockCommand = new RelayCommand(_ => SelectedContact = null);
        Refresh();
    }

    public ICommand OpenProfileCommand { get; }
    public ICommand CloseProfileCommand { get; }
    public ICommand AddContactCommand { get; }
    public ICommand StartChatCommand { get; }
    public ICommand BlockCommand { get; }

    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) Refresh(); } }
    public User? SelectedContact { get => _selectedContact; set { if (SetProperty(ref _selectedContact, value)) OnPropertyChanged(nameof(IsProfileVisible)); } }
    public bool IsProfileVisible => SelectedContact is not null;

    private void Refresh()
    {
        OnlineContacts.Clear(); SortedContacts.Clear();
        IEnumerable<User> q = AllContacts;
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var s = SearchText.Trim().ToLower();
            q = q.Where(u => u.FullName.ToLower().Contains(s) || u.Username.ToLower().Contains(s));
        }
        var list = q.ToList();
        foreach (var u in list.Where(x => x.IsOnline).OrderBy(x => x.FullName)) OnlineContacts.Add(u);
        foreach (var u in list.OrderBy(x => x.FullName)) SortedContacts.Add(u);
    }
}
