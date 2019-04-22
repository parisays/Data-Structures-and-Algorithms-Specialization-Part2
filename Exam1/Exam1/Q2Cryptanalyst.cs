using TestCommon;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;

namespace Exam1
{
    public class Q2Cryptanalyst : Processor
    {
        public Q2Cryptanalyst(string testDataName) : base(testDataName)
        {
            this.ExcludeTestCaseRangeInclusive(31, 37);
            LoadDictionary();
        }

        private void LoadDictionary()
        {
            string address = @"Exam1_TestData\TD2\dictionary.txt";
            var lines = File.ReadAllLines(address);
            foreach (var line in lines)
                Vocab.Add(line);
        }

        public override string Process(string inStr) => Solve(inStr);

        public HashSet<string> Vocab = new HashSet<string>();

        private int TestCaseNumber { get; set; }

        public string Solve(string cipher)
        {
            //Cryptanalysis c = new Cryptanalysis(
            //    @"Exam1_TestData\TD2\dictionary.txt",
            //    '0', '9');
            //return c.Decipher(
            //    cipher, 3, ' ', 'z',
            //    Cryptanalysis.IsDecipheredI1).GetHashCode().ToString();

            
            string result = null;
            string pass = GetFirstPass();
            while(true)
            {
                Encryption encryption = new Encryption(pass, ' ', 'z', false);
                result = encryption.Decrypt(cipher);
                List<string> words = result.Split(' ').ToList();
                if (VerifyResult(words))
                    break;
                
                pass = MakePass(pass);
            }

            TestCaseNumber++;
            return result.GetHashCode().ToString();
        }

        private string GetFirstPass()
        {
            int digits = TestCaseNumber / 10 + 1;
            string result = "0";
            while (result.Length < digits)
                result = "0" + result;

            return result;
        }

        private string MakePass(string prevNumber)
        {
            int digits = TestCaseNumber / 10 + 1;
            int newNumber = int.Parse(prevNumber) + 1;
            string result = newNumber.ToString();
            while (result.Length < digits)
                result = "0" + result;

            return result;
        }

        private bool VerifyResult(List<string> words)
        {
            int containCount = 0;
            int wordCount = words.Count();
            for(int i=0; i<wordCount; i++)
            {
                if (Vocab.Contains(words[i]))
                    containCount++;
            }

            int precentage = (int)(containCount * 100 / wordCount);

            return  precentage >= 75;
        }
    }
}