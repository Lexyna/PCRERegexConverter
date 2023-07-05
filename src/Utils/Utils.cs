public class Utils<T>
{

    public static List<List<T>> CrossProduct(List<List<T>> listOfLists)
    {
        List<List<T>> combinations = listOfLists.Take(1)
            .FirstOrDefault()
            .Select(i => (new T[] { i }).ToList())
            .ToList();

        foreach (List<T> series in listOfLists.Skip(1))
        {
            combinations = combinations.Join(
                series as List<T>, combination => true,
                i => true,
                (combination, i) =>
                {
                    List<T> nextCombination = new List<T>(combination);
                    nextCombination.Add(i);
                    return nextCombination;
                }
            ).ToList();
        }
        return combinations;
    }
}