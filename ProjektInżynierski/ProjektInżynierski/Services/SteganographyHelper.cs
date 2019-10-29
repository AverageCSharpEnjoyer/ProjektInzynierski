using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

class SteganographyHelper
{
    public static string DecryptText(Bitmap original, Bitmap encrypted)
    {
        return ReverseString(getTextFromDifferentInts(getDifferentInts(original, encrypted)));
    }

    private static List<int[]> getDifferentInts(Bitmap original, Bitmap encrypted)
    {
        var result = new List<int[]>();
        for (int i = 0; i < original.Height; i++)
        {
            // pass through each row
            for (int j = 0; j < original.Width; j++)
            {
                Color pixel = original.GetPixel(j, i);
                Color pixelEn = encrypted.GetPixel(j, i);
                if (pixel.R != pixelEn.R || pixel.G != pixelEn.G || pixel.B != pixelEn.B)
                {
                    var singleResult = new int[3];

                    singleResult[0] = pixel.R - pixelEn.R;
                    singleResult[1] = pixel.G - pixelEn.G;
                    singleResult[2] = pixel.B - pixelEn.B;
                    result.Add(singleResult);
                }
            }
        }
        return result;
    }

    private static string getTextFromDifferentInts(List<int[]> differentInts)
    {
        int charValue = 0;
        int bitCounter = 0;

        // holds the text that will be extracted from the image
        string extractedText = string.Empty;
        foreach (var inte in differentInts)
        {
            for (int j = 0; j < 3; j++)
            {
                //charValue = charValue * 2 + pixel.R % 2;
                switch (inte[j])
                {
                    //Case 2 happens if pixel already had 0 in stored value
                    case -1:
                    case 2:
                        charValue = charValue * 2 + 1;
                        break;
                    //Case -2 happens if pixel already had 255 in stored value
                    case 1:
                    case -2:
                        charValue = charValue * 2 + 0;
                        break;
                }

                bitCounter++;

                // if 8 bits has been added,
                // then add the current character to the result text
                if (bitCounter % 8 == 0)
                {
                    // reverse? of course, since each time the process occurs
                    // on the right (for simplicity)

                    charValue = ReverseBits(charValue);

                    // can only be 0 if it is the stop character (the 8 zeros)
                    if (charValue == 0)
                    {
                        return extractedText;
                    }

                    // convert the character value from int to char
                    char c = (char)charValue;

                    // add the current character to the result text
                    extractedText += c.ToString();
                }
            }
        }
        return extractedText;
    }

    public static Bitmap EncryptText(string text, Bitmap bmp)
    {
        Random rnd = new Random();

        Bitmap copyBmp = (Bitmap)bmp.Clone();
        //3 is number for RGB, number of colors in bitmap
        //8 is for byte length in bites
        int maxForRandomHiding = (int)((bmp.Height * bmp.Width) / (text.Length * 8));

        byte[] btText;
        btText = System.Text.Encoding.UTF8.GetBytes(text);
        Array.Reverse(btText);
        BitArray bits = new BitArray(btText);

        // holds the index of the bit that is being hidden
        int bitIndex = 0;

        int cellToChange = 0;
        int i = 0;
        int j = 0;

        int R;
        int G;
        int B;

        while (bitIndex < bits.Length)
        {
            i += cellToChange;
            //ensure jumping to next row
            while (i >= copyBmp.Width)
            {
                j++;
                i -= copyBmp.Width;
            }
            Color pixel = copyBmp.GetPixel(i, j);

            R = CheckIndexAndFill(bits, pixel.R, ref bitIndex);
            G = CheckIndexAndFill(bits, pixel.G, ref bitIndex);
            B = CheckIndexAndFill(bits, pixel.B, ref bitIndex);

            copyBmp.SetPixel(i, j, Color.FromArgb(R, G, B));

            cellToChange = rnd.Next(1, maxForRandomHiding);
        }


        return copyBmp;
    }

    private static int CheckIndexAndFill(BitArray bits, int pixelValue, ref int bitIndex)
    {
        int value;
        if (bitIndex < bits.Length)
        {
            value = ChangeBit(pixelValue, bits[bitIndex]);
            bitIndex++;
        }
        else
        {
            value = pixelValue;
        }

        return value;
    }

    private static int ChangeBit(int valueNow, bool valueOfBit)
    {
        int bit;
        if (valueOfBit == false)
        {
            //if its 0 we cannot substract
            if (valueNow == 0)
            {
                bit = valueNow + 2;
            }
            else
            {
                bit = valueNow - 1;
            }
        }
        else
        {
            //if its 255 we cannot add
            if (valueNow == 255)
            {
                bit = valueNow - 2;
            }
            else
            {
                bit = valueNow + 1;
            }
        }

        return bit;
    }
    public static int ReverseBits(int n)
    {
        int result = 0;

        for (int i = 0; i < 8; i++)
        {
            result = result * 2 + n % 2;

            n /= 2;
        }

        return result;
    }
    public static string ReverseString(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}