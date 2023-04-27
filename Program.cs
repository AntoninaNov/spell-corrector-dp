//using System;
//using System.Collections.Generic;

// Preparation
List<string> wordsList = new List<string>(File.ReadAllLines("words_list.txt"));

Console.WriteLine("Enter a sentence:");
string? input = Console.ReadLine();

List<string> words = SplitWords(input);
//List<string> misspelledWords = FindMisspelledWords(words, wordsList);

static List<string> SplitWords(string? input)
{
    var withoutPunctuation = new string(input.Where(c => !char.IsPunctuation(c)).ToArray());
    
    var words = withoutPunctuation.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
    
    var wordsList = words.ToList();
    return wordsList;
}

//static List<string> FindMisspelledWords(List<string> words, List<string> wordsList)
{
    // Check each word for its presence in the wordsList and return a list of misspelled words
}