using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Tests.CaseData
{
    public class MainViewModelTestDataCase
    {
        public static IEnumerable TestCasesForThreeDownoading
        {
            get
            {
                yield return new TestCaseData(12.6, 24.6, 45.7).Returns((12.6 + 24.6 + 45.7) / 3);
                yield return new TestCaseData(89.2, 33.6, 87.7).Returns((89.2 + 33.6 + 87.7) / 3);
                yield return new TestCaseData(52.6, 45.6, 90.7).Returns((52.6 + 45.6 + 90.7) / 3);
            }
        }

        public static IEnumerable TestCasesForTwoDownoading
        {
            get
            {
                yield return new TestCaseData(12.6, 24.6).Returns((12.6 + 24.6) / 2);
                yield return new TestCaseData(89.2, 33.6).Returns((89.2 + 33.6) / 2);
                yield return new TestCaseData(52.6, 45.6).Returns((52.6 + 45.6) / 2);
            }
        }

    }
}
