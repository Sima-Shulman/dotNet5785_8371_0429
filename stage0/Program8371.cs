namespace stage0
{
    partial class Program
    {
        private static void Main(string[] args)
        {
            Welcome8371();
            Welcome0429();
            Console.ReadKey();
        }

        static partial void Welcome0429();
        private static void Welcome8371()
        {
            Console.WriteLine("Enter your name: ");
            string userName = Console.ReadLine();
            Console.WriteLine($"{userName}, welcome to my first console application");
        }
    }
}