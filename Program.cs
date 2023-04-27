using System;
using System.Collections.Generic;

// Preparation
List<string> wordsList = new List<string>(File.ReadAllLines("/Users/vladshcherbyna/RiderProjects/spell-corrector-dp/words_list.txt"));

Console.WriteLine("Enter a sentence:");
string? input = Console.ReadLine();

List<string> words = SplitWords(input);
foreach (string word in words)
{
    Console.WriteLine(word);
}

List<string> misspelledWords = FindMisspelledWords(words, wordsList);

PrintMisspelledWords(misspelledWords);

static List<string> SplitWords(string? input)
{
    var withoutPunctuation = new string(input.Where(c => !char.IsPunctuation(c) || c == '\'').ToArray());
    
    var words = withoutPunctuation.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
    
    var wordsList = words.ToList();
    return wordsList;
}

static List<string> FindMisspelledWords(List<string> words, List<string> wordsList)
{
    List<string> misspelledWords = new List<string>();

    foreach (string word in words)
    {
        if (!wordsList.Contains(word.ToLower()))
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
