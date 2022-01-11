using Playground.CSharp;
using System;

public class Program
{
    public static void Main()
    {
        // removed because that would give an error in the struct in 17.1
        //var x = new S0();
        //Console.WriteLine(x.X.ToString());


        var x3 = new S1();
        Console.WriteLine(x3.X.ToString());

        // removed because that would give an error here in 17.1
        //var x4 = new S2();
        //Console.WriteLine(x4.X.ToString());

        var x5 = new S3();
        Console.WriteLine(x5.X.ToString());

        var x6 = new S3("Hello");
        Console.WriteLine(x5.X.ToString());

    }
}
