using System;
using System.IO;

namespace ScoreManager
{
    class Token
    {
        private int token;
        private bool valid;
        public Token()
        {
            valid = false;
        }
        public bool Generate()
        {
            try
            {
                var ran = new Random((int)DateTime.Now.Ticks);
                token = ran.Next(1000000);
                valid = true;
                var writer = new StreamWriter(Program.ballancePath + "\\Bin\\Token.txt");
                writer.Write(token);
                writer.Close();
                return true;
            }
            catch (Exception)
            {
                valid = false;
                return false;
            }

        }
        public bool Verify(int given)
        {
            if (valid == true && token == given)
            {
                valid = false;
                return true;
            }
            return false;
        }
        public bool Destroy()
        {
            valid = false;
            try
            {
                if (File.Exists(Program.ballancePath + "\\Bin\\Token.txt"))
                {
                    File.Delete(Program.ballancePath + "\\Bin\\Token.txt");
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
