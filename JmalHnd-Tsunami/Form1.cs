using JmalHnd_Tsunami.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace JmalHnd_Tsunami
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static FontFamily font;
        public string LastURL = "";
        public bool LastExist = true;
        public static readonly RectangleF drawRect = new RectangleF(10, 10, 1060, 1060);
        public static bool debugging = false;
        public static string LastAreas = "";

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("////////////////////////\n/JmalHnd-Tsunami v1.0.4/\n////////////////////////\n準備中...");
            if (!Directory.Exists("Font"))
                Directory.CreateDirectory("Font");
            if (!File.Exists("Font\\Koruri-Regular.ttf"))
                File.WriteAllBytes("Font\\Koruri-Regular.ttf", Resources.Koruri_Regular);
            if (!File.Exists("Font\\LICENSE"))
                File.WriteAllText("Font\\LICENSE", Resources.Koruri_LICENSE);
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile("Font\\Koruri-Regular.ttf");
            font = pfc.Families[0];

            Draw(); return;

            //以下デバッグ用
            debugging = true;
            //Draw(1);//スキップ
            //Draw("F:\\ダウンロード\\201611220614.xml");
            //Draw("F:\\ダウンロード\\20220115175401_0_VTSE41_010000.xml");
            //Draw("F:\\ダウンロード\\20220316143936_0_VTSE41_270000.xml");
        }

        /// <summary>
        /// デバッグで古い情報取得用(feedのみ)
        /// </summary>
        /// <param name="skip">スキップする数</param>
        /// <param name="Uri">URL 既定は高頻度feed</param>
        public void Draw(int skip, string Uri = "https://www.data.jma.go.jp/developer/xml/feed/eqvol.xml")
        {
            Draw(Uri, true, skip);
        }

        /// <summary>
        /// 描画します。
        /// </summary>
        /// <param name="Uri">URL 既定は高頻度feed</param>
        /// <param name="feed">feedの場合true</param>
        /// <param name="skip">(デバッグ用、Draw(int skip, [string Uri])を推奨)スキップする数 既定は0</param>
        public void Draw(string Uri = "https://www.data.jma.go.jp/developer/xml/feed/eqvol.xml", bool feed = false, int skip = 0)
        {
            try
            {
                Console.Clear();
                Console.Write("\x1b[3J");//環境によりすべてクリアされないことがあるため
                bool Exist = true;
                Console.WriteLine($"処理を開始します。({DateTime.Now:HH:mm:ss.ff})");
                XmlDocument xml = new XmlDocument();
                Console.WriteLine($"取得中…({Uri})");
                xml.Load(Uri);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);
                nsmgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                if (feed || Uri == "https://www.data.jma.go.jp/developer/xml/feed/eqvol.xml" || Uri == "https://www.data.jma.go.jp/developer/xml/feed/eqvol_l.xml")
                {
                    Exist = false;
                    Console.WriteLine("feedから津波情報を検索中…");
                    foreach (XmlNode node in xml.SelectNodes("atom:feed/atom:entry", nsmgr))
                    {
                        if (node.SelectSingleNode("atom:title", nsmgr).InnerText == "津波警報・注意報・予報a")
                        {
                            if (skip != 0)
                            {
                                skip--;
                                continue;
                            }
                            string URL2 = node.SelectSingleNode("atom:id", nsmgr).InnerText;
                            if (URL2 == LastURL)
                            {
                                Console.WriteLine($"見つかりました。前回と同一のため取得はしません。");
                                Console.WriteLine($"処理が完了しました。({DateTime.Now:HH:mm:ss.ff})");
                                Console.WriteLine($"取得間隔:{GetTimer.Interval}  次回取得:{DateTime.Now.AddMilliseconds(GetTimer.Interval):HH:mm:ss}ごろ");
                                Console.WriteLine($"前回の情報:{LastAreas}");
                                
                                return;
                            }
                            Console.WriteLine($"見つかりました。取得中…({URL2})");
                            xml.Load(URL2);
                            LastURL = URL2;
                            Exist = true;
                            break;
                        }
                    }
                    if (!Exist && LastExist)
                        Console.WriteLine("見つかりませんでした。");
                    if (!Exist && !LastExist)
                    {
                        Console.WriteLine("見つかりませんでした。描画はしません。");
                        Console.WriteLine($"処理が完了しました。({DateTime.Now:HH:mm:ss.ff})");
                        Console.WriteLine($"取得間隔:{GetTimer.Interval}  次回取得:{DateTime.Now.AddMilliseconds(GetTimer.Interval):HH:mm:ss}ごろ");
                        return;
                    }
                }
                //未発表再現用
                //Exist = false;

                Dictionary<string, string> CodeWarn = new Dictionary<string, string>();
                List<TsunamiInfo> Infos = new List<TsunamiInfo>();
                if (Exist)
                {
                    nsmgr.AddNamespace("jmx", "http://xml.kishou.go.jp/jmaxml1/");
                    nsmgr.AddNamespace("jmx_ib", "http://xml.kishou.go.jp/jmaxml1/informationBasis1/");
                    nsmgr.AddNamespace("jmx_se", "http://xml.kishou.go.jp/jmaxml1/body/seismology1/");
                    nsmgr.AddNamespace("jmx_eb", "http://xml.kishou.go.jp/jmaxml1/elementBasis1/");
                    LastAreas = "";
                    foreach (XmlNode infos in xml.SelectNodes("jmx:Report/jmx_se:Body/jmx_se:Tsunami/jmx_se:Forecast/jmx_se:Item", nsmgr))
                    {
                        //エリア名とコード
                        string name = infos.SelectSingleNode("jmx_se:Area/jmx_se:Name", nsmgr).InnerText;
                        string code = infos.SelectSingleNode("jmx_se:Area/jmx_se:Code", nsmgr).InnerText;
                        //大津波警報　津波警報　津波注意報　津波予報
                        string level = infos.SelectSingleNode("jmx_se:Category/jmx_se:Kind/jmx_se:Name", nsmgr).InnerText.Replace("（若干の海面変動）", "");
                        //(到達後はnull)
                        XmlNode ArrivalTime = infos.SelectSingleNode("jmx_se:FirstHeight/jmx_se:ArrivalTime", nsmgr);
                        string time = ArrivalTime == null ? "" : DateTime.Parse(ArrivalTime.InnerText).ToString("MM/dd HH:mm");
                        XmlNode Condition = infos.SelectSingleNode("jmx_se:FirstHeight/jmx_se:Condition", nsmgr);
                        //(未到達はnull)　ただちに津波来襲と予測　津波到達中と推測　第１波の到達を確認
                        string state = Condition == null ? "" : Condition.InnerText; ;
                        //(津波予報に引き下げられたとき?(要確認)null) 巨大(要確認) １０ｍ　０．２ｍ未満
                        string height = infos.SelectSingleNode("jmx_se:MaxHeight/jmx_eb:TsunamiHeight", nsmgr) == null ? "" : ((XmlElement)infos.SelectSingleNode("jmx_se:MaxHeight/jmx_eb:TsunamiHeight", nsmgr)).GetAttribute("description");
                        Console.WriteLine($"{name} {level} {time} {state} {height}");
                        LastAreas += $"\n{name} {level} {time} {state} {height}";
                        CodeWarn.Add(code, level);
                        Infos.Add(new TsunamiInfo
                        {
                            Name = name,
                            Level = level,
                            Time = time,
                            State = state,
                            Height = height
                        });
                    }
                }
                double LatSta = 21;
                double LatEnd = 48;
                double LonSta = 122.5;
                double LonEnd = 149.5;
                int MapSize = 1080;////27*40
                double Zoom = 40;
                Bitmap bitmap = new Bitmap(1920, 1080);
                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(Color.FromArgb(0, 30, 60));

                Pen pen = new Pen(Color.FromArgb(0, 0, 255), 10)
                {
                    LineJoin = LineJoin.Round
                };
                if (Exist)
                {
                    Console.WriteLine("津波情報描画中…");
                    JObject json_tsunami = JObject.Parse(Resources.AreaTsunami_GIS_0_1);
                    GraphicsPath Maps_MajorWarn = new GraphicsPath();
                    GraphicsPath Maps_Warn = new GraphicsPath();
                    GraphicsPath Maps_Advisory = new GraphicsPath();
                    GraphicsPath Maps_Forecast = new GraphicsPath();
                    foreach (JToken json_1 in json_tsunami.SelectToken("features"))
                    {
                        Maps_MajorWarn.StartFigure();
                        Maps_Warn.StartFigure();
                        Maps_Advisory.StartFigure();
                        Maps_Forecast.StartFigure();
                        if (json_1.SelectToken("geometry.coordinates") == null)
                            continue;
                        if ((string)json_1.SelectToken("geometry.type") == "LineString")
                        {
                            List<Point> points = new List<Point>();
                            foreach (JToken json_2 in json_1.SelectToken($"geometry.coordinates"))
                                points.Add(new Point((int)(((double)json_2.SelectToken("[0]") - LonSta) * Zoom), (int)((LatEnd - (double)json_2.SelectToken("[1]")) * Zoom)));
                            if (points.Count < 2)
                                continue;
                            if (CodeWarn.Keys.Contains((string)json_1.SelectToken($"properties.code")))
                            {
                                string level = CodeWarn[(string)json_1.SelectToken($"properties.code")];
                                switch (level)
                                {
                                    case "津波予報":
                                        Maps_Forecast.AddLines(points.ToArray());
                                        break;
                                    case "津波注意報":
                                        Maps_Advisory.AddLines(points.ToArray());
                                        break;
                                    case "津波警報":
                                        Maps_Warn.AddLines(points.ToArray());
                                        break;
                                    case "大津波警報":
                                        Maps_MajorWarn.AddLines(points.ToArray());
                                        break;
                                }
                            }
                        }
                        else
                        {
                            foreach (JToken json_2 in json_1.SelectToken($"geometry.coordinates"))
                            {
                                Maps_MajorWarn.StartFigure();
                                Maps_Warn.StartFigure();
                                Maps_Advisory.StartFigure();
                                Maps_Forecast.StartFigure();
                                List<Point> points = new List<Point>();
                                foreach (JToken json_3 in json_2)
                                    points.Add(new Point((int)(((double)json_3.SelectToken("[0]") - LonSta) * Zoom), (int)((LatEnd - (double)json_3.SelectToken("[1]")) * Zoom)));
                                if (points.Count < 2)
                                    continue;
                                if (CodeWarn.Keys.Contains((string)json_1.SelectToken($"properties.code")))
                                {
                                    string level = CodeWarn[(string)json_1.SelectToken($"properties.code")];
                                    switch (level)
                                    {
                                        case "津波予報":
                                            Maps_Forecast.AddLines(points.ToArray());
                                            break;
                                        case "津波注意報":
                                            Maps_Advisory.AddLines(points.ToArray());
                                            break;
                                        case "津波警報":
                                            Maps_Warn.AddLines(points.ToArray());
                                            break;
                                        case "大津波警報":
                                            Maps_MajorWarn.AddLines(points.ToArray());
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    g.DrawPath(pen, Maps_Forecast);
                    pen.Color = Color.FromArgb(192, 192, 0);
                    g.DrawPath(pen, Maps_Advisory);
                    pen.Color = Color.FromArgb(255, 0, 0);
                    g.DrawPath(pen, Maps_Warn);
                    pen.Color = Color.FromArgb(128, 0, 128);
                    g.DrawPath(pen, Maps_MajorWarn);
                }

                Console.WriteLine("都道府県描画中…");
                JObject json_Map = JObject.Parse(Resources.AreaForecastLocalEEW_GIS_0_05);
                GraphicsPath Maps = new GraphicsPath();
                Maps.Reset();
                foreach (JToken json_1 in json_Map.SelectToken("features"))
                {
                    Maps.StartFigure();
                    if (json_1.SelectToken("geometry.coordinates") == null)
                        continue;
                    if ((string)json_1.SelectToken("geometry.type") == "Polygon")
                    {
                        List<Point> points = new List<Point>();
                        foreach (JToken json_2 in json_1.SelectToken($"geometry.coordinates[0]"))
                            points.Add(new Point((int)(((double)json_2.SelectToken("[0]") - LonSta) * Zoom), (int)((LatEnd - (double)json_2.SelectToken("[1]")) * Zoom)));
                        if (points.Count > 2)
                            Maps.AddPolygon(points.ToArray());
                    }
                    else
                    {
                        foreach (JToken json_2 in json_1.SelectToken($"geometry.coordinates"))
                        {
                            List<Point> points = new List<Point>();
                            foreach (JToken json_3 in json_2.SelectToken("[0]"))
                                points.Add(new Point((int)(((double)json_3.SelectToken("[0]") - LonSta) * Zoom), (int)((LatEnd - (double)json_3.SelectToken("[1]")) * Zoom)));
                            if (points.Count > 2)
                                Maps.AddPolygon(points.ToArray());
                        }
                    }
                }
                g.FillPath(new SolidBrush(Color.FromArgb(30, 60, 90)), Maps);
                pen.Color = Color.FromArgb(128, 128, 128);
                pen.Width = 1;
                g.DrawPath(pen, Maps);

                Console.WriteLine("詳細情報描画中…");
                string EventID = "";
                string AnoT = "";
                g.FillRectangle(Brushes.Black, 1080, 0, 840, 1080);
                if (Exist)
                {
                    int py = 10;
                    SolidBrush RectColor = new SolidBrush(Color.FromArgb(30, 30, 60));
                    foreach (TsunamiInfo i in Infos)
                    {
                        g.FillRectangle(RectColor, 1090, py, 820, 96);
                        g.DrawRectangle(Level2Pen(i.Level), 1090, py, 820, 96);
                        string level = i.Level == "津波予報" || i.Level == "津波警報" ? $"　{i.Level}" : i.Level;
                        string state = i.State == "津波到達中と推測" ? $"　　　{i.State}" : i.State == "第１波の到達を確認" ? $"　　{i.State}" : i.State;
                        string time = i.Time == "" ? "到達予想時刻: -- / --  -- : -- " : $"到達予想時刻:{i.Time}";
                        g.DrawString(i.Name, new Font(font, 28), Brushes.White, 1095, py + 5);
                        g.DrawString(level, new Font(font, 28), Brushes.White, 1495, py + 5);
                        g.DrawString(i.Height.Replace("．", "."), new Font(font, 28), Brushes.White, 1695, py + 5);
                        g.DrawString(i.State, new Font(font, 20), Brushes.White, 1255, py + 55);
                        g.DrawString(time, new Font(font, 20), Brushes.White, 1585, py + 55);
                        py += 107;
                        if (py > 1000)
                            break;
                    }
                    string AnoTime = DateTime.Parse(xml.SelectSingleNode("jmx:Report/jmx_ib:Head/jmx_ib:ReportDateTime", nsmgr).InnerText).ToString("yyyy/MM/dd HH:mm:ss");
                    AnoT = AnoTime.Replace("/", "").Replace(":", "").Replace(" ", "");
                    string Office = xml.SelectSingleNode("jmx:Report/jmx:Control/jmx:EditorialOffice", nsmgr).InnerText;
                    EventID = xml.SelectSingleNode("jmx:Report/jmx_ib:Head/jmx_ib:EventID", nsmgr).InnerText;

                    string Comment1 = xml.SelectSingleNode("jmx:Report/jmx_ib:Head/jmx_ib:Headline/jmx_ib:Text", nsmgr).InnerText;
                    XmlNode Text = xml.SelectSingleNode("jmx:Report/jmx_se:Body/jmx_se:Text", nsmgr);
                    string Comment2 = Text == null ? "" : Text.InnerText;
                    XmlNode ForeEnd = xml.SelectSingleNode("jmx:Report/jmx_ib:Head/jmx_ib:ValidDateTime", nsmgr);//津波予報の失効時刻(ないときもある)
                    string ValidDateTime = ForeEnd == null ? "" : $"\n{DateTime.Parse(ForeEnd.InnerText):yyyy/MM/dd HH:mm}まで有効";
                    string hypoInfo = "<震源要素>";
                    foreach (XmlNode hypo_ in xml.SelectNodes("jmx:Report/jmx_se:Body/jmx_se:Earthquake", nsmgr))
                    {
                        string EqTime = DateTime.Parse(hypo_.SelectSingleNode("jmx_se:OriginTime", nsmgr).InnerText).ToString("yyyy/MM/dd HH:mm");
                        string Hypo = hypo_.SelectSingleNode("jmx_se:Hypocenter/jmx_se:Area/jmx_se:Name", nsmgr).InnerText;
                        //国内
                        XmlNode NameFromMark = hypo_.SelectSingleNode("jmx_se:Hypocenter/jmx_se:Area/jmx_se:NameFromMark", nsmgr);
                        //海外
                        XmlNode DetailedName = hypo_.SelectSingleNode("jmx_se:Hypocenter/jmx_se:Area/jmx_se:DetailedName", nsmgr);
                        string SubHypo = NameFromMark != null ? $"({NameFromMark.InnerText})" : DetailedName != null ? $"({DetailedName.InnerText})" : "";
                        string Location = ((XmlElement)hypo_.SelectSingleNode("jmx_se:Hypocenter/jmx_se:Area/jmx_eb:Coordinate", nsmgr)).GetAttribute("description");
                        string Magnitude = ((XmlElement)hypo_.SelectSingleNode("jmx_eb:Magnitude", nsmgr)).GetAttribute("description");
                        hypoInfo += $"\n{EqTime}    {Hypo}{SubHypo}\n{Location.Replace("深さ　", "深さ").Replace("　", " ")}  {Magnitude}";
                    }

                    g.DrawString(Zen2Han($"{AnoTime}発表  {Office}  ID:{EventID}\n{hypoInfo}\n<詳細情報>\n{Comment1}\n{Comment2.Replace("。　", "。\n").Replace("　", "")}\n{ValidDateTime}"), new Font(font, 18), Brushes.White, drawRect);
                    g.DrawString("気象データ・地図データ:気象庁", new Font(font, 20), Brushes.White, 680, 1040);
                    if (ForeEnd == null)//基本津波注意報以上
                    {
                        if (debugging)
                        {
                            g.FillRectangle(new SolidBrush(Color.FromArgb(64, 0, 0, 0)), 0, 0, 1920, 1080);
                            g.DrawString("現在のデータではありません。", new Font(font, 40), Brushes.White, 576, 500);
                        }
                        GetTimer.Interval = 60000;//1m
                    }
                    else
                    {
                        GetTimer.Interval = 300000;//5m
                        if (DateTime.Parse(ForeEnd.InnerText) < DateTime.Now)
                        {
                            g.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), 0, 0, 1920, 1080);
                            g.DrawString("この情報は既に失効しています", new Font(font, 40), Brushes.White, 576, 500);
                        }
                    }
                }
                else
                {
                    //中心確認    
                    //g.DrawString("されていません", new Font(font, 40), Brushes.Red, 960, 500);
                    g.DrawString("津波情報は発表されていません", new Font(font, 40), Brushes.White, 576, 500);
                    g.DrawString("気象データ・地図データ:気象庁", new Font(font, 20), Brushes.White, 680, 1040);
                }

                Console.WriteLine("描画完了");
                if (Exist)
                    if (Directory.Exists("output"))
                    {
                        xml.Save($"output\\{(xml.BaseURI.Contains("/") ? xml.BaseURI.Split('/').Last() : xml.BaseURI.Split('\\').Last())}");
                        bitmap.Save($"output\\{EventID}.{AnoT}.png", ImageFormat.Png);
                        Console.WriteLine($"保存完了({(xml.BaseURI.Contains("/") ? xml.BaseURI.Split('/').Last() : xml.BaseURI.Split('\\').Last())},{EventID}.{AnoT}.png)");
                    }
                    else
                        Console.WriteLine("[お知らせ]outputフォルダを作るとxmlファイル・画像ファイルが保存されます。");
                BackgroundImage = null;
                BackgroundImage = bitmap;
                g.Dispose();
                //throw new Exception("デバック用");
                Console.WriteLine($"処理が完了しました。({DateTime.Now:HH:mm:ss.ff})");
                Console.WriteLine($"取得間隔:{GetTimer.Interval}  次回取得:{DateTime.Now.AddMilliseconds(GetTimer.Interval):HH:mm:ss}ごろ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"//////////////////////////////////////////////////\nエラーが発生しました。わからない場合開発者に報告してください。このスクリーンショットがあると助かります。" +
                    $"\n<エラーの例>\nオブジェクト参照がオブジェクト インスタンスに設定されていません。/値をnullにすることはできません:処理ミスです。" +
                    $"\n//////////////////////////////////////////////////\n内容:{ex}");
            }
        }


        /// <summary>
        /// 津波情報のグレード別に色を返します。
        /// </summary>
        /// <remarks>地区ごとの情報一覧の四角用です、</remarks>
        /// <param name="level">津波予報/津波注意報/津波警報/大津波警報</param>
        /// <returns>グレード別の色のPen</returns>
        public static Pen Level2Pen(string level)
        {
            switch (level)
            {
                case "津波予報":
                    return new Pen(Color.FromArgb(0, 0, 255), 4);
                case "津波注意報":
                    return new Pen(Color.FromArgb(192, 192, 0), 4);
                case "津波警報":
                    return new Pen(Color.FromArgb(255, 0, 0), 4);
                case "大津波警報":
                    return new Pen(Color.FromArgb(128, 0, 128), 4);
                default:
                    return Pens.Black;
            }
        }

        /// <summary>
        /// 全角から半角に無理やり変換します。
        /// </summary>
        /// <param name="text">変換元</param>
        /// <returns>変換後</returns>
        public static string Zen2Han(string text)
        {
            return text.Replace("０", "0").Replace("１", "1").Replace("２", "2").Replace("３", "3").Replace("４", "4")
                .Replace("５", "5").Replace("６", "6").Replace("７", "7").Replace("８", "8").Replace("９", "9")
                .Replace("．", ".").Replace("ｋｍ", "km").Replace("Ｍ", "M");
        }

        private void GetTimer_Tick(object sender, EventArgs e)
        {
            Draw();
        }

        private void TSM_LongFeed_Click(object sender, EventArgs e)
        {
            Draw("https://www.data.jma.go.jp/developer/xml/feed/eqvol_l.xml");
        }

        private void TSM_ReleaseSite_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Ichihai1415/JmalHnd-Tsunami/releases");
        }

        private void TSMGetnow_Click(object sender, EventArgs e)
        {
            Draw();
        }
    }
    public class TsunamiInfo
    {
        public string Name { get; set; }
        public string Level { get; set; }
        public string Time { get; set; }
        public string State { get; set; }
        public string Height { get; set; }
    }
}
