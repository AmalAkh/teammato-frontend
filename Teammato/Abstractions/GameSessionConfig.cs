namespace Teammato.Abstractions;

public class GameSessionConfig
{
    public string GameId { get; set; }
    public int PlayersCount { get; set; }
    
    public List<string> Languages { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Duration { get; set; }
    public string Desciption { get; set; }

    public GameSessionConfig()
    {
        Languages = new List<string>();
    }
}