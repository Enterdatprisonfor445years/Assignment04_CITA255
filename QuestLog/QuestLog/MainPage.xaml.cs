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
        // Check if the Title or Giver fields were left empty
        if (string.IsNullOrWhiteSpace(TitleEntry.Text) || string.IsNullOrWhiteSpace(GiverEntry.Text))
        {
            await DisplayAlertAsync("Error", "Please fill out both the Title and Giver fields.", "OK");
            return;
        }

        // Safely convert the reward text into a whole number
        if (!int.TryParse(RewardEntry.Text, out int rewardAmount) || rewardAmount < 0)
        {
            await DisplayAlertAsync("Error", "Please enter a valid, positive number for the reward.", "OK");
            return;
        }

        // Add the new structured quest object to your collection
        Quests.Add(new Quest
        {
            Title = TitleEntry.Text,
            Giver = GiverEntry.Text,
            Reward = rewardAmount
        });

        // Clear the input text boxes so they are ready for the next entry
        TitleEntry.Text = string.Empty;
        GiverEntry.Text = string.Empty;
        RewardEntry.Text = string.Empty;

        // Update the total quest count tracker label on the screen
        CountLabel.Text = $"{Quests.Count} quests";



    }

    private async void OnTurnInQuestClicked(object sender, EventArgs e)
    {
        // Grab the currently selected quest object from your UI using the correct name: QuestView
        var selectedQuest = QuestView.SelectedItem as Quest;

        // Validate that an item is actually highlighted
        if (selectedQuest == null)
        {
            await DisplayAlertAsync("Notice", "Please select a quest from the list to turn in.", "OK");
            return;
        }

        // Delete the chosen quest from your tracking collection
        Quests.Remove(selectedQuest);

        // Reset the UI selection so nothing remains highlighted accidentally
        QuestView.SelectedItem = null;

        // Trigger your JSON backend manager to sync changes directly onto the local drive
        Save();
    }

    // Converts your tracking collection into plain text and commits it to disk
    private void Save()
    {
        // Serialize your Quests list into a structured JSON string
        string json = System.Text.Json.JsonSerializer.Serialize(Quests);

        // Write that text block directly into your system path file
        File.WriteAllText(path, json);
    }

    // Reads the text back from storage to reconstruct your collection on launch
    private void Load()
    {
        // Safety check: Make sure a saved file exists so the application doesn't crash on absolute first boot
        if (!File.Exists(path)) return;

        // Pull the entire JSON string out of the text file
        string json = File.ReadAllText(path);

        // Turn the raw text back into a list of Quest objects
        var savedQuests = System.Text.Json.JsonSerializer.Deserialize<List<Quest>>(json);

        if (savedQuests != null)
        {
            // Add them to your live observable collection one by one to avoid breaking data bindings
            foreach (var q in savedQuests)
            {
                Quests.Add(q);
            }
        }
    }

}
