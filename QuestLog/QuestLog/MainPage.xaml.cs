namespace QuestLog;

using System.Collections.ObjectModel;





public partial class MainPage : ContentPage
{
    public ObservableCollection<Quest> Quests { get; } = new ObservableCollection<Quest>();

    private string path = Path.Combine(FileSystem.AppDataDirectory, "quests.json");

    string[] pool =
    {
        "Clear the cellar rats",
        "Escort the merchant to Dunmoor",
        "Find the alchemist's cat",
        "Light the three beacons",
        "Return the sword to the lake",
        "Investigate the noise under the inn"
    };

    int next = 0;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
        Quests.Add(new Quest { Title = "Speak to the innkeeper", Giver = "Townsperson", Reward = 10 });
        Quests.Add(new Quest { Title = "Buy a lantern", Giver = "Merchant", Reward = 25 });

        Load();


    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleField.Text) || string.IsNullOrWhiteSpace(GiverField.Text))
        {
            await DisplayAlertAsync("Error", "Please fill out both the Title and Giver fields.", "OK");
            return;
        }

        if (!int.TryParse(RewardField.Text, out int rewardAmount) || rewardAmount < 0)
        {
            await DisplayAlertAsync("Error", "Please enter a valid, positive number for the reward.", "OK");
            return;
        }

        Quests.Add(new Quest
        {
            Title = TitleField.Text,
            Giver = GiverField.Text,
            Reward = rewardAmount
        });

        TitleField.Text = string.Empty;
        GiverField.Text = string.Empty;
        RewardField.Text = string.Empty;

        CountLabel.Text = $"{Quests.Count} quests";

        Save();



    }

    private async void OnTurnInQuestClicked(object sender, EventArgs e)
    {
        var selectedQuest = QuestView.SelectedItem as Quest;

        if (selectedQuest == null)
        {
            await DisplayAlertAsync("Notice", "Please select a quest from the list to turn in.", "OK");
            return;
        }

        
        Quests.Remove(selectedQuest);

        QuestView.SelectedItem = null;

        Save();
    }

    private void Save()
    {
        string json = System.Text.Json.JsonSerializer.Serialize(Quests);

        File.WriteAllText(path, json);
    }

    private void Load()
    {

        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);

        var savedQuests = System.Text.Json.JsonSerializer.Deserialize<List<Quest>>(json);

        if (savedQuests != null)
        {

            foreach (var q in savedQuests)
            {
                Quests.Add(q);
            }
        }
    }

}
