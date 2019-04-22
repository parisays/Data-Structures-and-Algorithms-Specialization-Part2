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
            this.ExcludeTestCaseRangeInclusive(1, 2);
            this.ExcludeTestCaseRangeInclusive(11, 37);
            //this.ExcludeTestCaseRangeInclusive(27, 37);
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
            int range = 10;
            for(int i = 0; i<range; i++)
            {
                Encryption encryption = new Encryption(i.ToString(), ' ', 'z', false);
                result = encryption.Decrypt(cipher);
                List<string> words = result.Split(' ').ToList();
                if (VerifyResult(words))
                    break;
            }

            TestCaseNumber++;
            return result.GetHashCode().ToString();
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

            return  precentage >= 80;
        }
    }
}