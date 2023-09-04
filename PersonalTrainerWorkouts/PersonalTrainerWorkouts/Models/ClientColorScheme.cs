using PersonalTrainerWorkouts.Models.ContactsAndClients;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Models;

[Table($"{nameof(ClientColorScheme)}s")]
public class ClientColorScheme
{
    public Color FontColor       { get; set; }
    public Color BackgroundColor { get; set; }

    [ForeignKey(typeof(Client))]
    public int? ClientId { get; set; }

    [OneToOne(CascadeOperations = CascadeOperation.All)]
    public Client Client { get; set; }
}
