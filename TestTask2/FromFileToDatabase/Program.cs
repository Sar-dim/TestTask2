using Data;
using Microsoft.Extensions.Configuration;

try
{
    IConfigurationRoot configuration = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build();
    var filePath = configuration.GetSection("FilePath").Value;

    if (filePath == null)
    {
        throw new Exception("Put file path into the appsettings.json");
    }

    var textFromFile = File.ReadAllText(filePath);
    var separators = new char[] { ' ', '.', ',', ':', '!', '?', '\r', '\n' };
    var listOfWords = textFromFile.Split(separators, StringSplitOptions.RemoveEmptyEntries)
        .ToList();

    var wordsForAddingToDatabase = new List<Word>();

    listOfWords.ForEach(x =>
    {
        if (x.Length >= 3 && x.Length <= 20)
        {
            var word = wordsForAddingToDatabase.FirstOrDefault(y => y.Name == x);
            if (word != null)
            {
                word.CountOfOccurrences++;
            }
            else
            {
                wordsForAddingToDatabase.Add(new Word
                {
                    Name = x,
                    CountOfOccurrences = 1
                });
            }
        }
    });

    wordsForAddingToDatabase.RemoveAll(x => x.CountOfOccurrences < 4);

    using (var context = new ApplicationContext())
    {
        var entitiesToChangeCounter = context.Words.Where(x => wordsForAddingToDatabase.Select(y => y.Name).Contains(x.Name))
            .ToList();
        entitiesToChangeCounter.ForEach(x => x.CountOfOccurrences += wordsForAddingToDatabase.First(y => y.Name == x.Name).CountOfOccurrences);

        var entitiesToAdd = wordsForAddingToDatabase.ExceptBy(entitiesToChangeCounter.Select(x => x.Name), x => x.Name).ToList();
        context.Words.AddRange(entitiesToAdd);

        context.SaveChanges();
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

