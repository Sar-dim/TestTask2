using Data;

var wordBegining = string.Empty;

Console.WriteLine("Input begining of words");
wordBegining = Console.ReadLine();
Console.WriteLine();

try
{
    if (string.IsNullOrWhiteSpace(wordBegining))
    {
        throw new Exception("Null or white space has been inserted");
    }

    using (var context = new ApplicationContext())
    {
        context.Words.Where(x => x.Name.StartsWith(wordBegining))
            .OrderByDescending(x => x.CountOfOccurrences)
            .ThenBy(x => x.Name)
            .Take(5)
            .ToList()
            .ForEach(x => Console.WriteLine($"Name: {x.Name}, Count: {x.CountOfOccurrences}"));
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
