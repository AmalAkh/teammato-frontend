namespace Teammato.Abstractions;

public class ErrorReport
{
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public string Platform { get; set; }
}