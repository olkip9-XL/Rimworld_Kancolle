using FixedPawnGenerate;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Verse;
using Verse.Sound;

namespace FPG_Kancolle
{
    [StaticConstructorOnStartup]
    public static class ModFixer
    {
        static ModFixer()
        {
            Mod_FPG_Kancolle.settings.ResolveDefs();

            //LogCharacterCodes();
        }

        private static void LogCharacterCodes()
        {
            //generate character code

            List<string> strs = new List<string>();
            string res = "Character Codes:\n";
            foreach (var item in DefDatabase<FixedPawnDef>.AllDefs.Where(x => x.tags.Contains("Kancolle")))
            {
                string hash = ConvertTo12Digit(item.defName);

                res += $"{item.defName}: {hash}\n";

                strs.Add(hash);
            }

            Log.Warning(res);

            //check duplicate codes
            var duplicates = strs.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicates.Count > 0)
            {
                string dupRes = "Duplicate Character Codes Found:\n";
                foreach (var dup in duplicates)
                {
                    dupRes += $"{dup}\n";
                }
                Log.Error(dupRes);
            }
        }

        private static string ConvertTo12Digit(string input)
        {
            // 使用MD5获得16字节哈希
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                int[] numbers = new int[4];

                for (int i = 0; i < 4; i++)
                {
                    // 取每组4字节转int（避免溢出，使用 BitConverter.ToUInt32）
                    uint part = BitConverter.ToUInt32(hash, i * 4);
                    // 映射到30~300
                    numbers[i] = (int)(part % 271) + 30;
                }

                // 拼接成12位字符串
                return $"{numbers[0]:D3};{numbers[1]:D3};{numbers[2]:D3};{numbers[3]:D3}";
            }
        }

    }
}
