using System.IO;

namespace ScoreManager
{
    class TxtReader
    {
        public string Data { get; set; }

        public TxtReader(Stream FileStream)
        {
            var reader = new StreamReader(FileStream);
            Data = reader.ReadToEnd();
        }

        public TxtReader(string Text)
        {
            Data = Text;
        }

        public ExportTxt GetExport()
        {
            string[] strarr = Data.Split(',');
            if (strarr.Length != 12)
            {
                return null;
            }
            var exportTxt = new ExportTxt
            {
                LvID = int.Parse(strarr[0]),
                HSScore = int.Parse(strarr[1]),
                SRTime = int.Parse(strarr[2]),
                Token = int.Parse(strarr[3]),
                LifeUp = int.Parse(strarr[4]),
                ExtraPoints = int.Parse(strarr[5]),
                SubExtraPoints = int.Parse(strarr[6]),
                LifeLost = int.Parse(strarr[7]),
                Trafo = int.Parse(strarr[8]),
                Checkpoint = int.Parse(strarr[9]),
                ReferenceTime = int.Parse(strarr[10]),
                HashCode = int.Parse(strarr[11])
            };
            return exportTxt;
        }
    }

    class ExportTxt
    {
        public int LvID { get; set; }
        public int HSScore { get; set; }
        public int SRTime { get; set; }
        public int Token { get; set; }
        public int LifeUp { get; set; }
        public int ExtraPoints { get; set; }
        public int SubExtraPoints { get; set; }
        public int LifeLost { get; set; }
        public int Trafo { get; set; }
        public int Checkpoint { get; set; }
        public int TravelDistance { get; set; }
        public int ReferenceTime { get; set; }
        public int HashCode { get; set; }
        public bool Verify(Token token)
        {
            return Verifier.VerifyTxt(this) && token.Verify(Token);
        }
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", LvID, HSScore, SRTime, Token, LifeUp, ExtraPoints, SubExtraPoints, LifeLost, Trafo, Checkpoint, ReferenceTime);
        }
        public string ToHashString()
        {
            return string.Concat(HSScore, ",", SRTime);
        }
        public override int GetHashCode()
        {
            return Verifier.GetHash(ToString());
        }
    }

    class Verifier
    {
        private static int GetStrHash(string str)
        {
            int len = str.Length;
            if (len == 0) return 0;
            int res = 1;
            for (int i = 0; i < len; ++i)
            {
                res = (res * 257 + str[i]) % 4999999;
            }
            res = (res + res % 3 + res % 11 + res % 101) % 4999999;
            return res;
        }

        public static int GetHash(string data)
        {
            return GetStrHash(GetStrHash(data).ToString());
        }

        public static bool VerifyTxt(ExportTxt data)
        {
            return GetHash(data.ToString()) == data.HashCode;
        }
    }

}
