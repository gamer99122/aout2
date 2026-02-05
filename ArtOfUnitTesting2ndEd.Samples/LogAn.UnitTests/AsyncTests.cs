using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LogAn.UnitTests
{
    // ========== 實作代碼（要被測試的代碼） ==========

    /// <summary>
    /// 被測試的類：演示異步操作
    /// 這個類模擬一個 500ms 後返回結果的異步操作
    /// </summary>
    public class ClassUnderTest
    {
        /// <summary>
        /// 延遲 500ms 後執行回調函數
        /// 這是一個使用回調模式的異步方法
        /// </summary>
        /// <param name="value">要傳遞的值</param>
        /// <param name="result">異步操作完成時的回調函數</param>
        public static void ReturnAfter500ms(int value, Action<int> result)
        {
            // 使用 TaskFacility 中的工廠來啟動新任務
            TaskFacility.Factory.StartNew(() =>
            {
                // 模擬長時間操作：睡眠 500ms
                Thread.Sleep(500);
                // 操作完成後，調用回調函數並傳入結果值
                result(value);
            });
        }
    }

    /// <summary>
    /// 任務工廠管理器
    /// 作用：允許在測試時替換 TaskFactory，以便控制異步行為
    /// 這是一個「依賴注入」的例子，便於測試異步代碼
    /// </summary>
    public class TaskFacility
    {
        /// <summary>
        /// 私有的 TaskFactory 實例
        /// 默認使用系統提供的 Task.Factory
        /// </summary>
        private static TaskFactory _factory;

        /// <summary>
        /// 重置工廠為默認值
        /// </summary>
        public static void Reset()
        {
            Factory = Task.Factory;
        }

        /// <summary>
        /// TaskFactory 屬性：getter 和 setter
        /// 允許外部代碼替換任務工廠
        /// 在測試時可以傳入自定義的 TaskFactory 來控制執行方式
        /// </summary>
        public static TaskFactory Factory
        {
            get
            {
                // 如果沒有設置，返回系統默認的 Task.Factory
                if (_factory == null) return Task.Factory;
                return _factory;
            }
            set
            {
                // 允許測試代碼設置自定義的 TaskFactory
                _factory = value;
            }
        }
    }

    // ========== 測試代碼 ==========

    /// <summary>
    /// 異步測試類
    /// [TestFixture] 標記這是一個 NUnit 測試類
    /// </summary>
    [TestFixture]
    class AsyncTests
    {
        /// <summary>
        /// 異步操作測試
        ///
        /// 這個測試展示如何測試異步代碼的技巧：
        /// 1. 注入自定義的 TaskFactory（依賴注入）
        /// 2. 設置 ExecuteSynchronously 讓異步代碼同步執行
        /// 3. 這樣就不用等待 500ms，測試會立即完成
        ///
        /// ⚠️ 注意：這個測試沒有 Assert 驗證語句
        /// 所以 NUnit 會標記它為「未執行」，偵錯程式也進不去
        /// 這是作者故意留下的不完整示例，讓學生思考如何完善
        /// </summary>
        [Test]
        public void TestAsync()
        {
            // 第 1 步：創建自定義的 TaskFactory
            // 參數說明：
            var tasks = new TaskFactory(
                new CancellationTokenSource().Token,           // 取消令牌：用於取消任務
                TaskCreationOptions.AttachedToParent,          // 創建選項：任務與父任務關聯
                TaskContinuationOptions.ExecuteSynchronously,  // 延續選項：★關鍵★ 同步執行（便於測試）
                TaskScheduler.Default);                        // 任務調度器：默認調度器

            // 第 2 步：將自定義工廠注入到 TaskFacility
            // 這樣 ClassUnderTest 使用的就是我們的工廠，不是默認的
            TaskFacility.Factory = tasks;

            // 第 3 步：調用要測試的異步方法
            // 傳入：值為 3，回調函數打印到 Console
            ClassUnderTest.ReturnAfter500ms(3, i => Console.WriteLine(i));

            // ❌ 缺少 Assert：這就是測試沒執行的原因
            // 一個完整的測試應該在這裡驗證結果
            // 例如：檢查回調是否被調用、是否傳入正確的值等
        }
    }
}
