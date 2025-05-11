namespace Teammato.Abstractions;

public class GameSessionSearchConfig
{
    public bool Nearest { get; set; }
    public string ?Description {get;set;}
    public  uint PlayersCount {get;set;}
    public List<string> Languages {get;set;}
    public List<string> GameIds {get;set;}
    public double DurationFrom{get;set;}
    public double DurationTo{get;set;}
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
}
