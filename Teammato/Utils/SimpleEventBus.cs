namespace Teammato.Utils;

public class SimpleEventBus
{
    private static readonly Dictionary<string, List<Action<object>>> _handlers =
        new Dictionary<string, List<Action<object>>>();

    private static readonly object _lock = new object();

    public static void CreateEvent(string eventName)
    {
        lock (_lock)
        {
            if (!_handlers.ContainsKey(eventName))
            {
                _handlers[eventName] = new List<Action<object>>();
            }
        }
    }

    public static void Subscribe(string eventName, Action<object> handler)
    {
        lock (_lock)
        {
            if (!_handlers.ContainsKey(eventName))
            {
                _handlers[eventName] = new List<Action<object>>();
            }

            _handlers[eventName].Add(handler);
        }
    }

    public static void Unsubscribe(string eventName, Action<object> handler)
    {
        lock (_lock)
        {
            if (_handlers.ContainsKey(eventName))
            {
                _handlers[eventName].Remove(handler);

                // Optionally remove empty lists to free memory
                if (_handlers[eventName].Count == 0)
                {
                    _handlers.Remove(eventName);
                }
            }
        }
    }
    public static void Emit(string eventName, object parameter = null)
    {
        List<Action<object>> subscribersCopy;

        lock (_lock)
        {
            if (!_handlers.ContainsKey(eventName))
                return;

            // Make a copy to avoid modification during iteration
            subscribersCopy = new List<Action<object>>(_handlers[eventName]);
        }

        foreach (var handler in subscribersCopy)
        {
            
                handler?.Invoke(parameter);
            
        }
    }
}