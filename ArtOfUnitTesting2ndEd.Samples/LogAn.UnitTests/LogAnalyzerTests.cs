using System;
using NUnit.Framework;

namespace LogAn.UnitTests
{
    /// <summary>
    /// LogAnalyzer 的單元測試類
    /// [TestFixture] 告訴 NUnit 這個類包含測試方法
    /// </summary>
    [TestFixture]
    public class LogAnalyzerTests
    {
        // ========== 基礎測試 ==========
        // 這些是最簡單的測試，展示 AAA 模式（Arrange-Act-Assert）

        /// <summary>
        /// 測試 1：副檔名不對時應返回 false
        /// 測試場景：.foo 不是有效的日誌副檔名
        /// 預期結果：返回 false
        /// </summary>
        [Test]
        public void IsValidLogFileName_BadExtension_ReturnsFalse()
        {
            // Arrange（準備）：建立分析器實例
            LogAnalyzer analyzer = new LogAnalyzer();

            // Act（執行）：調用要測試的方法，傳入錯誤的副檔名
            bool result = analyzer.IsValidLogFileName("filewithbadextension.foo");

            // Assert（驗證）：檢查結果是否為 false
            Assert.False(result);
        }

        /// <summary>
        /// 測試 2：小寫副檔名 .slf 時應返回 true
        /// 測試場景：.slf（小寫）是有效的
        /// 預期結果：返回 true
        /// </summary>
        [Test]
        public void IsValidLogFileName_GoodExtensionLowercase_ReturnsTrue()
        {
            LogAnalyzer analyzer = new LogAnalyzer();

            bool result = analyzer.IsValidLogFileName("filewithgoodextension.slf");

            Assert.True(result);
        }

        /// <summary>
        /// 測試 3：大寫副檔名 .SLF 時應返回 true
        /// 測試場景：.SLF（大寫）是有效的
        /// 預期結果：返回 true
        /// 這驗證了大小寫不敏感的功能
        /// </summary>
        [Test]
        public void IsValidLogFileName_GoodExtensionUppercase_ReturnsTrue()
        {
            LogAnalyzer analyzer = new LogAnalyzer();

            bool result = analyzer.IsValidLogFileName("filewithgoodextension.SLF");

            Assert.True(result);
        }

        // ========== 參數化測試 ==========
        // [TestCase] 允許用相同的邏輯測試多個輸入
        // 這樣不用重複寫相同的代碼

        /// <summary>
        /// 測試 4 和 5：合併測試 2 和 3（用參數化方式）
        /// [TestCase] 讓同一個測試方法執行多次，每次用不同的參數
        /// 好處：減少代碼重複
        /// </summary>
        [TestCase("filewithgoodextension.SLF")]     // 測試用例 1：大寫
        [TestCase("filewithgoodextension.slf")]     // 測試用例 2：小寫
        public void IsValidLogFileName_ValidExtensions_ReturnsTrue(string file)
        {
            LogAnalyzer analyzer = new LogAnalyzer();

            bool result = analyzer.IsValidLogFileName(file);

            Assert.True(result);
        }

        /// <summary>
        /// 測試 6、7、8：合併前面三個基礎測試
        /// 這次用 [TestCase] 同時測試輸入和預期結果
        /// 參數：(輸入文件名, 預期結果)
        /// </summary>
        [TestCase("filewithgoodextension.SLF", true)]    // 大寫 .SLF → true
        [TestCase("filewithgoodextension.slf", true)]    // 小寫 .slf → true
        [TestCase("filewithbadextension.foo", false)]    // 錯誤副檔名 → false
        public void IsValidLogFileName_VariousExtensions_ChecksThem(string file, bool expected)
        {
            LogAnalyzer analyzer = new LogAnalyzer();

            bool result = analyzer.IsValidLogFileName(file);

            // 檢查結果是否等於預期值
            Assert.AreEqual(expected, result);
        }

        // ========== 異常測試 ==========
        // 測試方法在特定情況下是否拋出正確的異常

        /// <summary>
        /// 測試 9：已被下方更好的異常測試方式取代
        /// NUnit 3 不再支持 [ExpectedException] 屬性
        /// 改用 Assert.Throws 是更現代的做法
        /// </summary>

        /// <summary>
        /// 輔助方法：建立 LogAnalyzer 實例
        /// 這樣可以避免在每個測試中重複建立
        /// </summary>
        private LogAnalyzer MakeAnalyzer()
        {
            return new LogAnalyzer();
        }

        /// <summary>
        /// 測試 10：空文件名應拋出異常（新方式）
        /// Assert.Throws 捕獲異常，然後驗證異常訊息
        /// 好處：比 [ExpectedException] 更靈活，能在一個測試中驗證多個異常
        /// </summary>
        [Test]
        public void IsValidLogFileName_EmptyFileName_Throws()
        {
            LogAnalyzer la = MakeAnalyzer();

            // Assert.Throws 捕獲 ArgumentException 異常
            var ex = Assert.Throws<ArgumentException>(() => la.IsValidLogFileName(""));

            // StringAssert.Contains 檢查異常訊息中是否包含特定文本
            StringAssert.Contains("filename has to be provided", ex.Message);
        }

        /// <summary>
        /// 測試 11：空文件名應拋出異常（流暢方式）
        /// Is.StringContaining 是 NUnit 的流暢 API
        /// 功能與上一個測試相同，但寫法更簡潔優雅
        /// </summary>
        [Test]
        public void IsValidLogFileName_EmptyFileName_ThrowsFluent()
        {
            LogAnalyzer la = MakeAnalyzer();

            var ex = Assert.Throws<ArgumentException>(() => la.IsValidLogFileName(""));

            // 流暢 API 語法：Assert.That(實際值, Does.條件)
            // NUnit 3 中 StringContaining 改名為 Does.Contain
            Assert.That(ex.Message, Does.Contain("filename has to be provided"));
        }

        // ========== 狀態測試 ==========
        // 測試方法是否正確更新對象的內部狀態

        /// <summary>
        /// 測試 12：調用方法後，WasLastFileNameValid 屬性應更新
        /// 這測試的是副作用（side effect）：方法改變了對象的狀態
        /// 場景：驗證無效文件名後，屬性應設為 false
        /// </summary>
        [Test]
        public void IsValidLogFileName_WhenCalled_ChangesWasLastFileNameValid()
        {
            LogAnalyzer la = MakeAnalyzer();

            // 執行：驗證無效文件名
            la.IsValidLogFileName("badname.foo");

            // 驗證：WasLastFileNameValid 應該是 false
            Assert.IsFalse(la.WasLastFileNameValid);
        }

        /// <summary>
        /// 測試 13 和 14：WasLastFileNameValid 屬性的參數化測試
        /// 驗證無論驗證結果是什麼，屬性都會被正確設置
        /// 參數：(輸入文件名, 預期的 WasLastFileNameValid 值)
        /// </summary>
        [TestCase("badfile.foo", false)]      // 無效文件名 → 屬性應為 false
        [TestCase("goodfile.slf", true)]      // 有效文件名 → 屬性應為 true
        public void IsValidLogFileName_WhenCalled_ChangesWasLastFileNameValid(string file, bool expected)
        {
            LogAnalyzer la = MakeAnalyzer();

            // 執行：驗證文件名
            la.IsValidLogFileName(file);

            // 驗證：檢查屬性是否等於預期值
            Assert.AreEqual(expected, la.WasLastFileNameValid);
        }
    }
}
