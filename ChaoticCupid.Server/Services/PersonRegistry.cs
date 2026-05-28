using ChaoticCupid.Server.Models;

namespace ChaoticCupid.Server.Services;

public class PersonRegistry
{
    private readonly Dictionary<string, Person> _persons = new(StringComparer.OrdinalIgnoreCase);
    private readonly Lock _lock = new();

    public bool TryRegister(Person person)
    {
        lock (_lock)
        {
            if (_persons.ContainsKey(person.Username))
                return false;
            _persons[person.Username] = person;
            return true;
        }
    }

    public void UpdateConnectionId(string username, string connectionId)
    {
        lock (_lock)
        {
            if (_persons.TryGetValue(username, out var p))
                p.ConnectionId = connectionId;
        }
    }

    public void Remove(string connectionId)
    {
        lock (_lock)
        {
            var key = _persons.FirstOrDefault(kv => kv.Value.ConnectionId == connectionId).Key;
            if (key != null)
                _persons.Remove(key);
        }
    }

    public Person? GetByConnection(string connectionId)
    {
        lock (_lock)
        {
            return _persons.Values.FirstOrDefault(p => p.ConnectionId == connectionId);
        }
    }

    public Person? GetByUsername(string username)
    {
        lock (_lock)
        {
            _persons.TryGetValue(username, out var p);
            return p;
        }
    }

    public IReadOnlyList<Person> GetAll()
    {
        lock (_lock)
        {
            return _persons.Values.ToList();
        }
    }
}
