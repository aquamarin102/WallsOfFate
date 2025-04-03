using System;
using System.Collections.Generic;
using System.Linq;

public static class AssembledPickups
{
    private static HashSet<Pickup> pickups = new HashSet<Pickup>();

    public static void AddPickup(Pickup pickup)
    {
        if (pickup == null)
        {
            throw new ArgumentNullException(nameof(pickup), "Pickup cannot be null.");
        }
        if (!pickups.Any(p => p.Name.Equals(pickup.Name, StringComparison.OrdinalIgnoreCase)))
        {
            pickups.Add(pickup);
        }

    }   

    public static void RemovePickup(int index)
    {
        if (index < 0 || index >= pickups.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }
        Pickup elem = pickups.ElementAt(index); 
        pickups.Remove(elem);
    }

    public static Pickup FindByName(string name)
    {
        return pickups.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public static IEnumerable<Pickup> GetPickupsByType(string type)
    {
        return pickups.Where(p => p.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
    }

    public static List<Pickup> GetAllPickups()
    {
        return pickups.ToList();
    }

    public static List<Pickup> GetRenderedPickups()
    {
        return pickups.Where(p => p.Rendered == true).ToList();
    }

    public static void Clear()
    {
        pickups.Clear();
    }

    public static int Count => pickups.Count;

    public static Pickup GetPickupByIndex(int index)
    {
        if (index < 0 || index >= pickups.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }
        return pickups.ElementAt(index); // Используем ElementAt для доступа по индексу
    }
}