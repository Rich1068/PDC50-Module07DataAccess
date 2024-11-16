
using Module07DataAccess.Services;
using MySql.Data.MySqlClient;

namespace Module07DataAccess
{
    public partial class MainPage : ContentPage
    {

        private readonly DatabaseConnectionService _dbConnectionService;

        public MainPage()
        {
            InitializeComponent();
            
            //initialize database connection
            _dbConnectionService = new DatabaseConnectionService();
        }

       

        private async void OnTestConnectionClicked(object sender, EventArgs e)
        {
            var connectionString = _dbConnectionService.GetConnectionString();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    ConnectionStatusLabel.Text = "Connection Successful";
                    ConnectionStatusLabel.TextColor = Color.FromArgb("#4CAF50");
                }
            }
            catch (Exception ex)
            {
                ConnectionStatusLabel.Text = $"Connection Failed: {ex.Message}";
                ConnectionStatusLabel.TextColor = Colors.Red;
            }
        }
        private async void OpenViewPersonal(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ViewPersonal");
        }
    }

}
