using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiniDashboard.App.Models;
using MiniDashboard.App.Services;

namespace MiniDashboard.App.ViewModels {
    public partial class MainViewModel : ObservableObject, INotifyPropertyChanged {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private ItemModel? _selectedItem;

        private string _searchText = string.Empty;

        [ObservableProperty]
        private string newItemName = string.Empty;

        [ObservableProperty]
        private string newItemDescription = string.Empty;

        [ObservableProperty]
        private decimal newItemPrice;

        public ObservableCollection<ItemModel> Items { get; set; } = new();

        public MainViewModel() {
            _apiService = new ApiService();
            LoadItemsCommand = new AsyncRelayCommand(LoadItemsAsync);
            AddItemCommand = new AsyncRelayCommand(AddItemAsync);
            UpdateItemCommand = new AsyncRelayCommand(UpdateItemAsync);
            DeleteItemCommand = new AsyncRelayCommand(DeleteItemAsync);
            SearchCommand = new AsyncRelayCommand(SearchItemsAsync);
        }

        // --- Properties ---
        public string SearchText {
            get => _searchText;
            set {
                if (SetProperty(ref _searchText, value))
                    _ = SearchItemsAsync();
            }
        }

        // --- Commands ---
        public ICommand LoadItemsCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand UpdateItemCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand SearchCommand { get; }

        // --- Methods ---
        private async Task LoadItemsAsync() {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try {
                Items.Clear();
                var items = await _apiService.GetItemsAsync();
                foreach (var item in items)
                    Items.Add(item);
            } catch (Exception ex) {
                ErrorMessage = $"Failed to load items: {ex.Message}";
            } finally {
                IsLoading = false;
            }
        }

        private async Task AddItemAsync() {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try {
                var newItem = new ItemModel {
                    Name = NewItemName,
                    Description = NewItemDescription,
                    Price = NewItemPrice
                };

                var created = await _apiService.AddItemAsync(newItem);

                if (created != null) {
                    Items.Add(created);
                    SelectedItem = created;

                    // Clear input fields for next entry
                    NewItemName = string.Empty;
                    NewItemDescription = string.Empty;
                    NewItemPrice = 0;
                } else {
                    ErrorMessage = "Add failed: The created item is null.";
                }
            } catch (Exception ex) {
                ErrorMessage = $"Add failed: {ex.Message}";
            } finally {
                IsLoading = false;
            }
        }

        private async Task UpdateItemAsync() {
            IsLoading = true;
            ErrorMessage = string.Empty;
            if (SelectedItem == null)
                return;

            try {
                await _apiService.UpdateItemAsync(SelectedItem); // UpdateItemAsync returns void, so no assignment is needed
                var index = Items.IndexOf(Items.First(i => i.Id == SelectedItem.Id));
                Items[index] = SelectedItem; // Refresh in UI
            } catch (Exception ex) {
                ErrorMessage = $"Update failed: {ex.Message}";
            } finally {
                IsLoading = false;
            }
        }

        private async Task DeleteItemAsync() {
            IsLoading = true;
            ErrorMessage = string.Empty;
            if (SelectedItem == null)
                return;

            try {
                await _apiService.DeleteItemAsync(SelectedItem.Id);
                Items.Remove(SelectedItem);
                SelectedItem = null;
            } catch (Exception ex) {
                ErrorMessage = $"Delete failed: {ex.Message}";
            } finally {
                IsLoading = false;
            }
        }

        private async Task SearchItemsAsync() {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try {
                var results = await _apiService.GetItemsAsync(SearchText);
                Items.Clear();
                foreach (var item in results)
                    Items.Add(item);
            } catch (Exception ex) {
                ErrorMessage = $"Search failed: {ex.Message}";
            } finally {
                IsLoading = false;
            }
        }
    }
}
