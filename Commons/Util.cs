namespace SakaguraAGFWebApi.Commons
{
    public static class Util
    {
        // 共通関数
        // 参考：https://gist.github.com/Buravo46/49c34e77ff1a75177340
        
        /// <summary>
        /// ハンディ入力の数量・箱数と、製品ラベルの入数から、登録すべき合計数量を計算
        /// </summary>
        /// <param name="inputQuantity"></param>
        /// <param name="inputPackingCount"></param>
        /// <param name="labelQuantity"></param>
        /// <returns></returns>
        public static int GetRegistTotalQuantity(int inputQuantity, int inputPackingCount, int labelQuantity)
        {
            int totalQuantity = 0;

            // 合計数量を計算
            if (inputQuantity == 0 && inputPackingCount == 0)
            {
                // 製品ラベルの数量をそのまま合計数量に
                totalQuantity = labelQuantity;
            }
            else if (inputQuantity > 0 && inputPackingCount == 0)
            {
                // 入力した数量をそのまま合計数量に
                totalQuantity = inputQuantity;
            }
            else if (inputQuantity == 0 && inputPackingCount > 0)
            {
                // 【製品ラベルの箱数】と【入力した箱数】で合計数量を計算
                totalQuantity = labelQuantity * inputPackingCount;
            }
            else if (inputQuantity > 0 && inputPackingCount > 0)
            {
                // 【入力した数量】と【入力した箱数】で合計数量を計算
                totalQuantity = inputQuantity * inputPackingCount;
            }

            return totalQuantity;
        }

    }
}
