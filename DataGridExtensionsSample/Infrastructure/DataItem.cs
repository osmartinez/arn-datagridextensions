namespace DataGridExtensionsSample.Infrastructure
{
    using System;
    using System.Windows;

    public class DataItem
    {
        private static readonly Random _rand = new Random();
        private static readonly string[] _samples = new[] {"lorem", "ipsum", "dolor", "sit", "amet"};

        public DataItem(int index)
        {
            Flag = _rand.Next(2) == 0;
            Index = index;
            Column1 = Guid.NewGuid().ToString();
            Column2 = _rand.Next(20) == 0 ? null : Guid.NewGuid().ToString();
            Column3 = Guid.NewGuid().ToString();
            Column4 = Guid.NewGuid().ToString();
            Column5 = _samples[_rand.Next(_samples.Length)];
            Column6 = (Visibility)_rand.Next(3);
            Column7 = CreateRandomWordNumberCombination() + " " +CreateRandomWordNumberCombination() + " " +CreateRandomWordNumberCombination();
            Probability = _rand.NextDouble();
        }

        public static string CreateRandomWordNumberCombination()
        {
            Random rnd = new Random();
            //Dictionary of strings
            string[] words = { "Bold", "Think", "Friend", "Pony", "Fall", "Easy" };
            //Random number from - to
            int randomNumber = rnd.Next(2000, 3000);
            //Create combination of word + number
            string randomString = $"{words[rnd.Next(0, words.Length)]}{randomNumber}";

            return randomString;

        }

        public bool Flag { get; private set; }
        public int Index { get; private set; }
        public string Column1 { get; set; }
        public string? Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public Visibility Column6 { get; set; }
        public string Column7 { get; set; }
        public double Probability { get; private set; }
    }
}
