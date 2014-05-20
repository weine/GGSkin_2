using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace GGSkin
{
    public partial class GGSkin : Form
    {
        public string CurrentPath;
        public string lolExe;
        CFGFile GGSINI;
        public string LolExeRoot;
        Version lolver;
        Version locver;

        public GGSkin()
        {
            InitializeComponent();
            this.CurrentPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            this.lolExe = "League of Legends.exe";
        }

        private void GGSkin_Load(object sender, EventArgs e)
        {
            string INIdoc = Directory.GetCurrentDirectory() + @"\config\GGSkin.ini";

            if (!File.Exists(INIdoc))
            {
                MessageBox.Show("GGSKin設定檔遺失");
                Application.Exit();
                return;
            }

            GGSINI = new CFGFile(INIdoc);
            LolExeRoot = GGSINI.GetValue("Init", "Lolpath");

            if (!String.IsNullOrEmpty(LolExeRoot))
            {
                lolFolder.Text = LolExeRoot;
            }

            gold.Text = GGSINI.GetValue("Fonts", "gold");
            atk.Text = GGSINI.GetValue("Fonts", "atk");
            beatk.Text = GGSINI.GetValue("Fonts", "beatk");
            cri.Text = GGSINI.GetValue("Fonts", "cri");
            lvup.Text = GGSINI.GetValue("Fonts", "lvup");

            locver = new Version(GGSINI.GetValue("Version", "lol"));
        }

        public string _output_status(int status_code)
        {
            string[] status_info = { 
                                       "面板套用成功!", 
                                       "遺失UI檔案", 
                                       "未設定LOL目錄!", 
                                       "資料夾未存在!", 
                                       "不正確的LOL目錄!", 
                                       "程式執行異常",
                                       "檔案文件遺失",
                                       "無法複製檔案"
                                   };

            return status_info[status_code];
        }

        private void lolBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderSelector = new FolderBrowserDialog();

            folderSelector.ShowDialog();

            if (String.IsNullOrEmpty(folderSelector.SelectedPath))
            {
                MessageBox.Show("未設定LOL執行檔目錄");
            }
            else
            {
                GGSINI.SetValue("Init", "Lolpath", folderSelector.SelectedPath);
                lolFolder.Text = folderSelector.SelectedPath;
            }
        }

        //安裝面板
        private int setupLolUI(string UIname)
        {
            string targetFolder = lolFolder.Text + @"\";
            string targetFile = lolFolder.Text + @"\GGSkin.zip";
            string sourceFile = System.IO.Path.Combine(this.CurrentPath, @"UI\" + UIname);
            int UIresult = 0;
            FileInfo UIFile = new FileInfo(sourceFile);

            //MessageBox.Show(sourceFile);
            //Environment.Exit(Environment.ExitCode);

            //判斷是否有面板檔
            if (!UIFile.Exists)
            {
                Console.WriteLine(this.CurrentPath + UIname);
                UIresult = 1;
            }

            //判斷LOL路徑是否為空
            else if (String.IsNullOrEmpty(lolFolder.Text))
            {
                Console.WriteLine("未設定LOL目錄!");
                UIresult = 2;
            }

            //判斷資料夾存在
            else if (!Directory.Exists(lolFolder.Text))
            {
                Console.WriteLine("資料夾未存在!");
                UIresult = 3;
            }

            //判斷是否為LOL執行檔目錄下
            else if (!File.Exists(System.IO.Path.Combine(targetFolder, this.lolExe)))
            {
                Console.WriteLine("不正確的LOL目錄!");
                UIresult = 4;
            }

            //檔案複製
            else
            {
                try
                {
                    UIFile.CopyTo(targetFile, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    UIresult = 5;
                }
            }

            return UIresult;
            /*
            using (StreamWriter ClipzipFile = new StreamWriter(targetFolder + "test.TXT"))
            {
                ClipzipFile.WriteLine("GGSkin.zip");

                MessageBox.Show("面板套用成功!");
            }
             * */
        }

        //設定文件
        private void setupDoc(int DocNo, int UIcode)
        {
            string targetFolder = lolFolder.Text + @"\";
            string targetFile = lolFolder.Text + @"\ClientZips.TXT";
            string sourceFile = this.CurrentPath;

            if (UIcode != 0)
            {
                Console.WriteLine(UIcode);
                MessageBox.Show("面板套用失敗: " + this._output_status(UIcode));
                return;
            }

            //遊戲版本檢查
            StreamReader sr = new StreamReader(targetFolder + @"\client.ver", Encoding.Default);

            string get_ver = sr.ReadLine().ToString();
            lolver = new Version(get_ver);

            if (locver.CompareTo(lolver) < 0)
            {
                this.zip_handle(targetFile, sourceFile, get_ver);
            }

            //依文件編號判斷
            switch (DocNo)
            {
                case 1:
                    sourceFile += @"zDoc\Default.TXT";
                    break;

                case 2:
                    sourceFile += @"zDoc\GGSkin.TXT";
                    break;

                default:
                    sourceFile += @"zDoc\Default.TXT";
                    break;
            }

            FileInfo lolCZ = new FileInfo(sourceFile);

            //判斷zDoc是否有檔
            if (!lolCZ.Exists)
            {
                Console.WriteLine("zDoc資料夾內無源檔");
                MessageBox.Show("面板套用失敗: " + this._output_status(6));
                return;
            }

            //檔案複製
            else
            {
                try
                {
                    lolCZ.CopyTo(targetFile, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine("無法複製檔案");
                    MessageBox.Show("面板套用失敗:" + this._output_status(7));
                    return;
                }
            }
            MessageBox.Show(this._output_status(0));
        }

        //zDoc文件處理
        private void zip_handle(string sourceFile, string targetFile, string lolcode)
        {
            string f_default;
            string f_gg;

            f_default = targetFile + @"zDoc\Default.TXT";
            f_gg = targetFile + @"zDoc\GGSkin.TXT";

            FileInfo fi = new FileInfo(sourceFile);


            //檔案複製
            try
            {
                fi.CopyTo(f_default, true);
                fi.CopyTo(f_gg, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("無法複製檔案");
                return;
            }

            using (StreamWriter sw = File.AppendText(f_gg))
            {
                sw.WriteLine("GGSkin.zip");
            }

            GGSINI.SetValue("Version", "lol", lolcode);
        }

        //原廠
        private void lolSkinDef_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("功能尚未完成");
            this.setupDoc(1, 0);
        }

        //Foxe UI
        private void lolSkinFoxe_Click(object sender, EventArgs e)
        {
            string FoxeUI = @"Foxe.zip";

            this.setupDoc(2, this.setupLolUI(FoxeUI));
        }

        //Peb UI
        private void lolSkinPeb_Click(object sender, EventArgs e)
        {
            string PebUI = @"Peb.zip";
            this.setupDoc(2, this.setupLolUI(PebUI));
        }

        //Dean UI
        private void lolSkinDean_Click(object sender, EventArgs e)
        {
            string DeanUI = @"Dean.zip";
            this.setupDoc(2, this.setupLolUI(DeanUI));
        }

        //D-Hero UI
        private void lolSkinHero_Click(object sender, EventArgs e)
        {
            string DHeroUI = @"D-Heroes.zip";
            this.setupDoc(2, this.setupLolUI(DHeroUI));
        }

        //安裝字型
        private void setupFont(string fontType)
        {
            string sourceFont = String.Format(@"{0}Fonts\{1}.ttf", this.CurrentPath, fontType);
            string targetFont = String.Format(@"{0}\DATA\Fonts\FZXHYSZK.ttf", lolFolder.Text);

            FileInfo lolfont = new FileInfo(sourceFont);

            //驗證
            if (!File.Exists(sourceFont))
            {
                Console.WriteLine("沒有字型檔!");
                Console.WriteLine(sourceFont);
                MessageBox.Show("字型變更失敗!");
                return;
            }

            if (!Directory.Exists(lolFolder.Text))
            {
                Console.WriteLine("找不到LOL的FontS目錄");
                Console.WriteLine(lolFolder.Text);
                MessageBox.Show("字型變更失敗!");
                return;
            }

            try
            {
                lolfont.CopyTo(targetFont, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show("字型變更失敗!");
                return;
            }

            if (!File.Exists(targetFont))
            {
                Console.WriteLine("無法複製字型至目標資料夾!");
                MessageBox.Show("字型變更失敗!");
                return;
            }


            MessageBox.Show("字型變更成功!");


        }

        //原廠字型
        private void fontBtn1_Click(object sender, EventArgs e)
        {
            this.setupFont("default");
        }

        //華康彩帶體
        private void fontBtn2_Click(object sender, EventArgs e)
        {
            this.setupFont("font2");
        }

        //華康皮皮體
        private void fontBtn3_Click(object sender, EventArgs e)
        {
            this.setupFont("font3");
        }

        //王漢宗綜藝體
        private void fontBtn4_Click(object sender, EventArgs e)
        {
            this.setupFont("font4");
        }

        //方正蘭亭粗黑
        private void fontBtn5_Click(object sender, EventArgs e)
        {
            this.setupFont("font5");
        }

        //喵嗚繁體
        private void fontBtn6_Click(object sender, EventArgs e)
        {
            this.setupFont("font6");
        }

        private string getFontSize(string FontName)
        {
            string ResFilePath = lolFolder.Text + @"\DATA\CFG\defaults\FontResolutions.xml";
            string FontSize = "";

            if (!Directory.Exists(lolFolder.Text))
            {
                Console.WriteLine("找不到LOL的FontS目錄");
                Console.WriteLine(lolFolder.Text);
                return "0";
            }

            XmlDocument Resxml = new XmlDocument();
            Resxml.Load(ResFilePath);

            XmlNodeList ResNodes = Resxml["FontResolutions"].ChildNodes;

            foreach (XmlNode ft in ResNodes)
            {
                if (ft.Attributes != null)
                {
                    if (ft.Attributes["Name"].Value == FontName)
                    {
                        XmlNode root = ft.FirstChild;
                        XmlNode fontNode = root.ChildNodes[0];
                        FontSize = fontNode.Attributes["FontSize"].Value;
                    }
                }
            }

            return FontSize;
        }

        private void setFontSize(string FontName, string FontSize)
        {
            string ResFilePath = lolFolder.Text + @"\DATA\CFG\defaults\FontResolutions.xml";

            if (!Directory.Exists(lolFolder.Text))
            {
                Console.WriteLine("找不到LOL的FontS目錄");
                Console.WriteLine(lolFolder.Text);
                return;
            }

            XmlDocument Resxml = new XmlDocument();
            Resxml.Load(ResFilePath);

            XmlNodeList ResNodes = Resxml["FontResolutions"].ChildNodes;

            foreach (XmlNode ft in ResNodes)
            {
                if (ft.Attributes != null)
                {
                    if (ft.Attributes["Name"].Value == FontName)
                    {
                        XmlNode root = ft.FirstChild;
                        XmlNode fontNode = root.ChildNodes[0];
                        fontNode.Attributes["FontSize"].Value = FontSize;
                    }
                }
            }

            Resxml.Save(ResFilePath);
        }

        private void checkKeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            string NewGoldFontSize = gold.Text;
            string NewAtkFontSize = atk.Text;
            string NewBeatkFontSize = beatk.Text;
            string NewCriFontSize = cri.Text;
            string NewLvupFontSize = lvup.Text;


            //設定金錢
            if (!String.IsNullOrEmpty(NewGoldFontSize))
            {
                this.setFontSize("gold_toast", NewGoldFontSize);
                string NowGoldFontSize = this.getFontSize("gold_toast");

                if (NowGoldFontSize == NewGoldFontSize)
                {
                    GGSINI.SetValue("Fonts", "gold", NewGoldFontSize);
                    Console.WriteLine("gold_toast更改成功");
                }
                else
                {
                    Console.WriteLine("gold_toast更改失敗");
                    MessageBox.Show("Error:101 更改失敗");
                    return;
                }
            }

            //設定輸出傷害
            if (!String.IsNullOrEmpty(NewAtkFontSize))
            {
                this.setFontSize("27_1_Auto", NewAtkFontSize);
                string NowAtkFontSize = this.getFontSize("27_1_Auto");

                if (NewAtkFontSize == NowAtkFontSize)
                {
                    GGSINI.SetValue("Fonts", "atk", NowAtkFontSize);
                    Console.WriteLine("輸出傷害更改成功");
                }
                else
                {
                    Console.WriteLine("輸出傷害更改失敗");
                    MessageBox.Show("Error:102 更改失敗");
                    return;
                }
            }

            //被傷害
            if (!String.IsNullOrEmpty(NewBeatkFontSize))
            {
                this.setFontSize("19_1_Auto", NewBeatkFontSize);
                string NowBeatkFontSize = this.getFontSize("19_1_Auto");

                if (NewBeatkFontSize == NowBeatkFontSize)
                {
                    GGSINI.SetValue("Fonts", "beatk", NowBeatkFontSize);
                    Console.WriteLine("被傷害更改成功");
                }
                else
                {
                    Console.WriteLine("被傷害更改失敗");
                    MessageBox.Show("Error:103 更改失敗");
                    return;
                }
            }

            //爆擊
            if (!String.IsNullOrEmpty(NewCriFontSize))
            {
                this.setFontSize("36_4_Auto", NewCriFontSize);
                string NowCriFontSize = this.getFontSize("36_4_Auto");

                if (NewCriFontSize == NowCriFontSize)
                {
                    GGSINI.SetValue("Fonts", "cri", NowCriFontSize);
                    Console.WriteLine("爆擊更改成功");
                }
                else
                {
                    Console.WriteLine("爆擊更改失敗");
                    MessageBox.Show("Error:104 更改失敗");
                    return;
                }
            }

            //升級
            if (!String.IsNullOrEmpty(NewLvupFontSize))
            {
                this.setFontSize("24_2_Auto", NewLvupFontSize);
                string NowLvupFontSize = this.getFontSize("24_2_Auto");

                if (NewLvupFontSize == NowLvupFontSize)
                {
                    GGSINI.SetValue("Fonts", "lvup", NowLvupFontSize);
                    Console.WriteLine("升級更改成功");
                }
                else
                {
                    Console.WriteLine("升級更改失敗");
                    MessageBox.Show("Error:105 更改失敗");
                    return;
                }
            }

            MessageBox.Show("已儲存!");
        }

    }
}
