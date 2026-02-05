using System;

namespace LogAn
{
    /// <summary>
    /// 日誌文件分析器 - 驗證日誌文件名是否有效
    /// 主要用於檢查文件副檔名是否為 .slf
    /// </summary>
    public class LogAnalyzer
    {
        /// <summary>
        /// 屬性：記錄最後一次驗證的結果是否有效
        /// 這是一個"狀態"屬性，用於被測代碼保存中間狀態
        /// </summary>
        public bool WasLastFileNameValid { get; set; }

        /// <summary>
        /// 驗證日誌文件名是否有效
        /// 規則：文件名必須以 .slf 結尾（大小寫不敏感）
        /// </summary>
        /// <param name="fileName">要驗證的文件名</param>
        /// <returns>true 表示有效，false 表示無效</returns>
        /// <exception cref="ArgumentException">當文件名為空或 null 時拋出異常</exception>
        public bool IsValidLogFileName(string fileName)
        {
            // 第 1 步：初始化為 false（假設無效）
            WasLastFileNameValid = false;

            // 第 2 步：檢查文件名是否為空或 null
            // 如果為空，拋出異常（異常訊息會被測試驗證）
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("filename has to be provided");
            }

            // 第 3 步：檢查文件名是否以 .slf 結尾
            // StringComparison.CurrentCultureIgnoreCase 表示大小寫不敏感
            // 如果副檔名不對，直接返回 false
            if (!fileName.EndsWith(".SLF", StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }

            // 第 4 步：如果通過所有檢查，設定為 true 並返回 true
            WasLastFileNameValid = true;
            return true;
        }

    }
}
