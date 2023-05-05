using System;
using System.Collections.Generic;

// Preparation
List<string> wordsList = new List<string>(File.ReadAllLines("/Users/vladshcherbyna/RiderProjects/spell-corrector-dp/words_list.txt"));
// /Users/vladshcherbyna/RiderProjects/spell-corrector-dp/words_list.txt//////antoninanovak
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
    List<string> suggestions = FindBestWords(misspelledWord, wordsList);
    Console.WriteLine($"For the word '{misspelledWord}', here are some suggestions: {string.Join(", ", suggestions)}");
}

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

static string LongestCommonSubsequence(string firstWord, string secondWord)
{
    int[,] lcsMatrix = new int[firstWord.Length + 1, secondWord.Length + 1];

    for (int i = 0; i <= firstWord.Length; i++)
    {
        for (int j = 0; j <= secondWord.Length; j++)
        {
            if (i == 0 || j == 0)
            {
                lcsMatrix[i, j] = 0;
            }
            else if (firstWord[i - 1] == secondWord[j - 1])
            {
                lcsMatrix[i, j] = lcsMatrix[i - 1, j - 1] + 1;
            }
            else
            {
                lcsMatrix[i, j] = Math.Max(lcsMatrix[i - 1, j], lcsMatrix[i, j - 1]);
            }
        }
    }

    int lcsLength = lcsMatrix[firstWord.Length, secondWord.Length];
    char[] lcsChars = new char[lcsLength];
    int index = lcsLength - 1;

    int m = firstWord.Length, n = secondWord.Length;
    while (m > 0 && n > 0)
    {
        if (firstWord[m - 1] == secondWord[n - 1])
        {
            lcsChars[index] = firstWord[m - 1];
            m--;
            n--;
            index--;
        }
        else if (lcsMatrix[m - 1, n] > lcsMatrix[m, n - 1])
        {
            m--;
        }
        else
        {
            n--;
        }
    }

    return new string(lcsChars);
}

static List<string> FindBestWords(string word, List<string> wordsList)
{
    List<string> bestWords = new List<string>();
    Dictionary<string, int> wordLCS = new Dictionary<string, int>();

    foreach (string w in wordsList)
    {
        int lcsLength = LongestCommonSubsequence(word, w).Length;
        wordLCS[w] = lcsLength;
    }

    var orderedLCS = wordLCS.OrderByDescending(l => l.Value).Take(5);

    foreach (var item in orderedLCS)
    {
        bestWords.Add(item.Key);
    }

    return bestWords;
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
