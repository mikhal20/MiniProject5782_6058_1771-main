using System;

namespace Targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome6058();
            Welcome1771();
            Console.ReadKey();
        }
        static partial void Welcome1771();
        private static void Welcome6058()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine(name + ", welcome to my first console application");
        }
    }
}
