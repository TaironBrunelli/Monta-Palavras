/// </summary>
/// Banco de Palavras
/*
A lógica do programa baseia-se em percorrer primeiramente o banco de dados/palavras (BancoDePalavras.txt) que é pego por “TextAsset WordsDatabase” e 
convertido em uma lista de strings “AllWords”, e também é criado uma cópia dessa lista “AllWordsAlpha”, 
só que com todas as LETRAS de cada palavra ordenadas alfabeticamente (mas a ordem das PALAVRAS na lista se mantém iguais para ambas listas).

De forma similar, a entrada do jogador também terá essas duas versões. Uma normal, e outra ordenadas alfabeticamente.

Optei em fazer as comparações com ambas palavras ordenadas alfabeticamente, pois acredito (não fiz as contas ainda para ter certeza) que 
seria melhor em performance do que as comparar de forma natural. 
 
Desse jeito, a palavra “Faixa”, ficaria “aafix”, como pego a entrada do jogador e faço a mesma coisa, caso não tenha “a” na entrada do jogador, 
e o “a” será sempre a primeira letra a ser comparada, já faço o programa pular a para outra palavra.

E como acabei de falar, dentro do loop do banco de dados, existem mais dois loops, um para percorrer todas as letras da palavra atual do banco de dados, 
e mais um para percorrer todas as letras da palavra do jogador.

Todas as palavras existentes no banco de dados e contidas na palavras do jogador é guardada em “AllWordsCorrect”.
A cada palavra que é encontrada, é feita a contagem de pontos pela função “CheckPoints”, e o maior valor obtido é salvo em “maxPoint”. 
Caso uma palavra obtenha uma pontuação maior do que a atual em maxPoint, essa palavra é Inserida no index zero (Insert(0, palavra) ao invés de ser ‘Add(palavra)’

Fiz isso para que, além de poder evoluir o projeto para mostrar todos os resultados ou mostrar outras pontuações, 
tenho também o controle fácil de qual é a palavra de maior ponto, que sempre será a do index zero.
*/
/// Tairon Brunelli
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BancoDePalavras : MonoBehaviour
{
    public TextAsset WordsDatabase;
    private string[] Words; 
    private List<string> AllWords;
    private List<string> AllWordsAlpha;
    public TMP_InputField playerField;
    private List<string> AllWordsCorrect;
    private int maxPoint;

    public TMP_Text chosenWord;
    public TMP_Text playerPoints;
    public TMP_Text lettersLeft;

    private char[] delimiterChars = { ' ', ',', '"', '\n' };

    void Start()
    {
        // Create a empty List
        AllWords = new List<string>();
        AllWordsAlpha = new List<string>();
        AllWordsCorrect = new List<string>();

        // Split the Data Base into words
        if (WordsDatabase != null)
            Words = WordsDatabase.text.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries);
            
        foreach (string word in Words)
		{
			AllWords.Add(UpperString(word));
		}

        foreach (string word in Words)
        {
            AllWordsAlpha.Add(SortString(word));
        }
    }

    //-- Convert the string to Uppercase
    public static string UpperString (string inputString)
    {
        inputString = RemoveAccents(inputString.ToUpper());
        return inputString;
    }

    //-- Remove Accents
    static string RemoveAccents(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }
        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string SortString(string inputString)
    {
        // convert input  
        // string to char array  
        inputString = RemoveAccents(inputString.ToUpper());
        char[] tempArray = inputString.ToCharArray();

        // sort tempArray  
        Array.Sort(tempArray);

        // return new sorted string  
        return new string(tempArray);
    }

    public void PlayBtt ()
    {
        string playerWord = playerField.text;

        CheckWord(SortString(playerWord));
    }

    private void CheckWord(string playerWord)
    {
        chosenWord.text = " ";
        playerPoints.text = " ";
        lettersLeft.text = " ";
        char[] tempArray;
        char[] playerWordArray = playerWord.ToCharArray();
        char[] tempAnswer = new char[100];
        string tempLettersLeftOver = " ";
        int tempPoints = 0;
        bool isLetterIn;
        maxPoint = 0; //-- Restart game
        AllWordsCorrect.Clear();  //-- Restart game

        //-- Loop all the words in my Datebase
        for (int i = 0; i < AllWordsAlpha.Count; i++)
        {
            tempArray = AllWordsAlpha[i].ToCharArray();
            Array.Clear(tempAnswer, 0, 99);
            playerWordArray = playerWord.ToCharArray(); //-- Reset the player's word;
            //-- Loop all the letters in the database's word
            for (int n = 0; n < tempArray.Length; n++)
            {
                isLetterIn = false;                
                //-- Loop all the letters in the player's word
                for (int m = 0; m < playerWordArray.Length; m++)
                {
                    if (tempArray[n] == playerWordArray[m])
                    {
                        tempAnswer[n] = playerWordArray[m]; //-- Answer Array receive the letter in the same position that Word in the AllWordsAlpha;
                        playerWordArray[m] = '1'; //-- remove the letter if it is the same; If not removed, the same letter can be used more more times;
                        isLetterIn = true;
                        break; //-- jump to next letter in the database's word
                    }
                }
                //-- The player's word did not contains one letter the in tempArray[n]'s word. 
                if (!isLetterIn)
                    break; //-- Jump to next word in AllWordsAlpha -> i++;
            }

            string tempAnswerString = new string(tempAnswer);
            string tempArrayString = new string(tempArray);

            //-- the player's word contains the database's word
            if (tempAnswerString.Contains(tempArrayString))
            {
                // check the points, if is bigger than the first, insert, if not, add.
                tempPoints = CheckPoints(AllWords[i]);
                if (tempPoints > maxPoint)
                {
                    maxPoint = tempPoints; //-- Change the Max Player's points and put the word in first at the list
                    AllWordsCorrect.Insert(0, AllWords[i]);
                    tempLettersLeftOver = LettersLeftOver(AllWords[i], playerWord);
                }
                else
                    AllWordsCorrect.Add(AllWords[i]); //-- Add to the list a AllWords[i], that contains the right word;
            }
        }
        //-- Show the answer and points;
        if (AllWordsCorrect.Count > 0)
        {
            chosenWord.text = AllWordsCorrect[0];
            lettersLeft.text = tempLettersLeftOver;
        }
        else
        {
            chosenWord.text = " ";
            lettersLeft.text = " ";
        }
        playerPoints.text = maxPoint.ToString();
        

        //-- Remove duplicates
        AllWordsCorrect = AllWordsCorrect.Distinct().ToList();

        //-- Print all list
        /*AllWordsCorrect.ForEach(delegate (String name)
        {
            UnityEngine.Debug.Log(name);
        });*/
    }

    private int CheckPoints(string wordPoint)
    {
        int tempPoints = 0;
        char[] tempArray;
        tempArray = wordPoint.ToCharArray();
        for (int n = 0; n < tempArray.Length; n++)
        {
            if ("EAIONRTLSU".Contains(tempArray[n]))
                tempPoints += 1;
            if ("WDG".Contains(tempArray[n]))
                tempPoints += 2;
            if ("BCMP".Contains(tempArray[n]))
                tempPoints += 3;
            if ("FHV".Contains(tempArray[n]))
                tempPoints += 4;
            if ("JX".Contains(tempArray[n]))
                tempPoints += 8;
            if ("QZ".Contains(tempArray[n]))
                tempPoints += 10;
        }
        return tempPoints;
    }

    private string LettersLeftOver(string wordChosen, string playerWord)
    {
        char[] tempWordChosen = wordChosen.ToCharArray();
        char[] tempPlayerWord = playerWord.ToCharArray();        
        string tempLettersLeft = playerWord;
        //-- Loop all the letters in the Player's word
        for (int n = 0; n < tempPlayerWord.Length; n++)
        {
            //-- Loop all the letters in the wordChosen's word
            for (int m = 0; m < tempWordChosen.Length; m++)
            {
                if (tempPlayerWord[n] == tempWordChosen[m])
                {
                    tempPlayerWord[n] = '1'; //-- remove the letter
                    tempWordChosen[m] = '2'; //-- remove the letter
                    break; //-- Found a letter;
                }                
            }  
        }
        string aux = new string(tempPlayerWord);
        tempLettersLeft = new String(aux.Where(Char.IsLetter).ToArray());
        return tempLettersLeft;
    }
}