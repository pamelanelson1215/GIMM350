using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    //static class that won't be in the actual scene

    public static string ProcessText(string input)
    {
        //my code goes here
        //determine whether user has input a word or a number
        //return the string "word" if user enters a word (includes 'ab.23cd')
        //return the string "number" if user enters a number (inludes 1.3)

        //try catch method to try for numerical value
        try
        {
            double number = double.Parse(input);
            return "number";
        }

        catch
        {
            return "word";
        }
    }
}
