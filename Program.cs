using System;
using System.Collections.Generic;

// Preparation
List<string> wordsList = new List<string>(File.ReadAllLines("/Users/antoninanovak/RiderProjects/spell-corrector-dp/words_list.txt"));
// /Users/vladshcherbyna/RiderProjects/spell-corrector-dp/words_list.txt
Console.WriteLine("Enter a sentence:");
string? input = Console.ReadLine();

List<string> words = SplitWords(input);
foreach (string word in words)
{
    Console.WriteLine(word);
}

List<string> misspelledWords = FindMisspelledWords(words, wordsList);

PrintMisspelledWords(misspelledWords);

foreach (string misspelledWord in misspelledWords)
{
    List<string> suggestions = FindSuggestions(misspelledWord, wordsList);
    Console.WriteLine($"For the word '{misspelledWord}', here are some suggestions: {string.Join(", ", suggestions)}");
}

static List<string> SplitWords(string? input)
{
    var withoutPunctuation = new string(input.Where(c => !char.IsPunctuation(c) || c == '\'').ToArray());
    
    var words = withoutPunctuation.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    
    var wordsList = words.ToList();
    return wordsList;
}


static List<string> FindMisspelledWords(List<string> words, List<string> wordsList)
{
    List<string> misspelledWords = new List<string>();
    
    foreach (string word in words)
    {
        if (!wordsList.Contains(word) && !wordsList.Contains(word.ToLower()))
        {
            misspelledWords.Add(word);
        }
    }
    return misspelledWords;
}

static void PrintMisspelledWords(List<string> misspelledWords)
{
    if (misspelledWords.Count > 0)
    {
        Console.WriteLine($"Looks like you have typos in the next words: {string.Join(", ", misspelledWords)}");
    }
    else
    {
        Console.WriteLine("No typos found.");
    }
}

// до кутових + 1
// до іншого + 0,якщо кутові однакові
// + 1 якщо кутові різні
static int DamerauLevenshteinDistance(string source, string target)
{
    int sourceLength = source.Length;
    int targetLength = target.Length;

    if (sourceLength == 0) return targetLength;
    if (targetLength == 0) return sourceLength;

    int[,] distanceMatrix = new int[sourceLength + 1, targetLength + 1]; // матриця для зберігання відстані між символами
    
    for (int i = 0; i <= sourceLength; i++) distanceMatrix[i, 0] = i;
    for (int j = 0; j <= targetLength; j++) distanceMatrix[0, j] = j;

    for (int i = 1; i <= sourceLength; i++)
    {
        for (int j = 1; j <= targetLength; j++)
        {
            int substitutionCost;
            if (source[i - 1] == target[j - 1])
            {
                substitutionCost = 0;
            }
            else
            {
                substitutionCost = 1;
            }
            
            int insertionCost = distanceMatrix[i - 1, j] + 1;
            int deletionCost = distanceMatrix[i, j - 1] + 1;
            int replaceCost = distanceMatrix[i - 1, j - 1] + substitutionCost;

            distanceMatrix[i, j] = Math.Min(Math.Min(insertionCost, deletionCost), replaceCost);

            // враховуємо перестановки (Damerau-Levenshtein Distance)
            if (i > 1 && j > 1 && source[i - 1] == target[j - 2] && source[i - 2] == target[j - 1])
            {
                int permutationCost = distanceMatrix[i - 2, j - 2] + substitutionCost;
                distanceMatrix[i, j] = Math.Min(distanceMatrix[i, j], permutationCost);
            }
        }
    }

    return distanceMatrix[sourceLength, targetLength];
}

static List<string> FindSuggestions(string incorrectWord, List<string> dictionaryWords)
{
    Dictionary<string, int> wordDistances = new Dictionary<string, int>();

    foreach (string word in dictionaryWords)
    {
        int distance = DamerauLevenshteinDistance(incorrectWord, word);
        wordDistances[word] = distance;
    }

    var sortedWordDistances = wordDistances.OrderBy(x => x.Value).Take(5).Select(x => x.Key).ToList();
    return sortedWordDistances;
}
