using JmalHnd_Tsunami.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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

        private void Form1_Load(object sender, EventArgs e)
        {//
            Draw();
            //Draw("https://www.gpvweather.com/jmaxml-view.php?k=%E6%B4%A5%E6%B3%A2%E8%AD%A6%E5%A0%B1%E3%83%BB%E6%B3%A8%E6%84%8F%E5%A0%B1%E3%83%BB%E4%BA%88%E5%A0%B1a&p=%E6%B0%97%E8%B1%A1%E5%BA%81&ym=2022-01&f=2022-01-15T15%3A15%3A19-20220115151519_0_VTSE41_010000.xml");
            //Draw("F:\\ダウンロード\\201611220614.xml");
            //Draw("F:\\ダウンロード\\20220115175401_0_VTSE41_010000.xml");
            //Draw("https://www.data.jma.go.jp/developer/xml/data/20230716072840_0_VTSE41_270000.xml");
        }
        public void Draw(string Uri = "https://www.data.jma.go.jp/developer/xml/feed/eqvol_l.xml")
        {
            Console.Clear();
            Console.WriteLine($"処理を開始します。({DateTime.Now:HH:mm:ss.ff})");
            XmlDocument xml = new XmlDocument();
            Console.WriteLine($"取得中…({Uri})");
            xml.Load(Uri);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            if (Uri == "https://www.data.jma.go.jp/developer/xml/feed/eqvol.xml" || Uri == "https://www.data.jma.go.jp/developer/xml/feed/eqvol_l.xml")
            {
                Console.WriteLine("feedから津波情報を検索中…");
                bool stop = true;
                while (stop)
                    try
                    {
                        foreach (XmlNode node in xml.SelectNodes("atom:feed/atom:entry", nsmgr))
                        {
                            if (node.SelectSingleNode("atom:title", nsmgr).InnerText == "津波警報・注意報・予報a")
                            {
                                string URL2 = node.SelectSingleNode("atom:id", nsmgr).InnerText;
                                Console.WriteLine($"見つかりました。取得中…({URL2})");
                                xml.Load(URL2);
                                stop = false;
                                break;
                            }
                        }
                        if (stop)
                            if (Uri == "https://www.data.jma.go.jp/developer/xml/feed/extra.xml")
                            {
                                Console.WriteLine("見つかりませんでした。https://www.data.jma.go.jp/developer/xml/feed/extra_l.xmlで再試行します。");
                                Uri = "https://www.data.jma.go.jp/developer/xml/feed/extra_l.xml";
                                xml.Load(Uri);
                            }
                            else
                            {
                                Console.WriteLine("見つかりませんでした。");
                                return;
                            }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
            }
            nsmgr.AddNamespace("jmx", "http://xml.kishou.go.jp/jmaxml1/");
            nsmgr.AddNamespace("jmx_ib", "http://xml.kishou.go.jp/jmaxml1/informationBasis1/");
            nsmgr.AddNamespace("jmx_se", "http://xml.kishou.go.jp/jmaxml1/body/seismology1/");
            nsmgr.AddNamespace("jmx_eb", "http://xml.kishou.go.jp/jmaxml1/elementBasis1/");
            Dictionary<string, string> CodeWarn = new Dictionary<string, string>();
            List<TsunamiInfo> Infos = new List<TsunamiInfo>();
            foreach (XmlNode infos in xml.SelectNodes("jmx:Report/jmx_se:Body/jmx_se:Tsunami/jmx_se:Forecast/jmx_se:Item", nsmgr))
            {
                string name = infos.SelectSingleNode("jmx_se:Area/jmx_se:Name", nsmgr).InnerText;
                string code = infos.SelectSingleNode("jmx_se:Area/jmx_se:Code", nsmgr).InnerText;
                //(未到達はnull)　ただちに津波来襲と予測　津波到達中と推測　第１波の到達を確認
                string level = infos.SelectSingleNode("jmx_se:Category/jmx_se:Kind/jmx_se:Name", nsmgr).InnerText.Replace("（若干の海面変動）", "");
                XmlNode ArrivalTime = infos.SelectSingleNode("jmx_se:FirstHeight/jmx_se:ArrivalTime", nsmgr);
                string time = ArrivalTime == null ? "" : DateTime.Parse(ArrivalTime.InnerText).ToString("MM/dd HH:mm");
                XmlNode Condition = infos.SelectSingleNode("jmx_se:FirstHeight/jmx_se:Condition", nsmgr);
                string state = Condition == null ? "" : Condition.InnerText; ;
                string height = ((XmlElement)infos.SelectSingleNode("jmx_se:MaxHeight/jmx_eb:TsunamiHeight", nsmgr)).GetAttribute("description");
                Console.WriteLine($"{name} {level} {time} {state} {height}");
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
            double LatSta = 21;
            double LatEnd = 48;
            double LonSta = 122.5;
            double LonEnd = 149.5;
            int MapSize = 1080;////27*40
            double Zoom = 40;
            Bitmap bitmap = new Bitmap(1920, 1080);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.FromArgb(0, 30, 60));

            Console.WriteLine("津波情報描画中…");
            JObject geojson2 = JObject.Parse(Resources.AreaTsunami_GIS_0_1);
            GraphicsPath Maps_MajorWarn = new GraphicsPath();
            GraphicsPath Maps_Warn = new GraphicsPath();
            GraphicsPath Maps_Advisory = new GraphicsPath();
            GraphicsPath Maps_Forecast = new GraphicsPath();
            foreach (JToken json_1 in geojson2.SelectToken("features"))
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
            Pen pen = new Pen(Color.FromArgb(0, 0, 255), 10)
            {
                LineJoin = LineJoin.Round
            };
            g.DrawPath(pen, Maps_Forecast);
            pen.Color = Color.FromArgb(192, 192, 0);
            g.DrawPath(pen, Maps_Advisory);
            pen.Color = Color.FromArgb(255, 0, 0);
            g.DrawPath(pen, Maps_Warn);
            pen.Color = Color.FromArgb(128, 0, 128);
            g.DrawPath(pen, Maps_MajorWarn);


            Console.WriteLine("都道府県描画中…");
            JObject geojson1 = JObject.Parse(Resources.AreaForecastLocalEEW_GIS_0_05);
            GraphicsPath Maps = new GraphicsPath();
            Maps.Reset();
            foreach (JToken json_1 in geojson1.SelectToken("features"))
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
            g.FillRectangle(Brushes.Black, 1080, 0, 840, 1080);
            int py = 10;
            SolidBrush RectColor = new SolidBrush(Color.FromArgb(30, 30, 60));
            foreach (TsunamiInfo i in Infos)
            {
                g.FillRectangle(RectColor, 1090, py, 820, 96);
                g.DrawRectangle(Level2Pen(i.Level), 1090, py, 820, 96);
                string level = i.Level == "津波予報" || i.Level == "津波警報" ? $"　{i.Level}" : i.Level;
                string state = i.State == "津波到達中と推測" ? $"　　　{i.State}" : i.State == "第１波の到達を確認" ? $"　　{i.State}" : i.State;
                string time = i.Time == "" ? "到達予想時刻: -- / --  -- : -- " : $"到達予想時刻:{i.Time}";
                g.DrawString(i.Name, new Font("Koruri Regular", 28), Brushes.White, 1095, py + 5);
                g.DrawString(level, new Font("Koruri Regular", 28), Brushes.White, 1495, py + 5);
                g.DrawString(i.Height.Replace("．", "."), new Font("Koruri Regular", 28), Brushes.White, 1695, py + 5);
                g.DrawString(i.State, new Font("Koruri Regular", 20), Brushes.White, 1255, py + 55);
                g.DrawString(time, new Font("Koruri Regular", 20), Brushes.White, 1585, py + 55);
                py += 107;
                if (py > 1080)
                    break;
            }
            string Time = DateTime.Parse(xml.SelectSingleNode("jmx:Report/jmx_se:Body/jmx_se:Earthquake/jmx_se:OriginTime", nsmgr).InnerText).ToString("yyyy/MM/dd HH:mm");
            string Hypo = xml.SelectSingleNode("jmx:Report/jmx_se:Body/jmx_se:Earthquake/jmx_se:Hypocenter/jmx_se:Area/jmx_se:Name", nsmgr).InnerText;
            //国内
            XmlNode NameFromMark = xml.SelectSingleNode("jmx:Report/jmx_se:Body/jmx_se:Earthquake/jmx_se:Hypocenter/jmx_se:Area/jmx_se:NameFromMark", nsmgr);
            //海外
            XmlNode DetailedName = xml.SelectSingleNode("jmx:Report/jmx_se:Body/jmx_se:Earthquake/jmx_se:Hypocenter/jmx_se:Area/jmx_se:DetailedName", nsmgr);
            string SubHypo = NameFromMark != null ? $"({NameFromMark.InnerText})" : DetailedName != null ? $"({DetailedName.InnerText})" : "";
            string Location = ((XmlElement)xml.SelectSingleNode("jmx:Report/jmx_se:Body/jmx_se:Earthquake/jmx_se:Hypocenter/jmx_se:Area/jmx_eb:Coordinate", nsmgr)).GetAttribute("description");
            string Magnitude = ((XmlElement)xml.SelectSingleNode("jmx:Report/jmx_se:Body/jmx_se:Earthquake/jmx_eb:Magnitude", nsmgr)).GetAttribute("description");
            XmlNode Text = xml.SelectSingleNode("jmx:Report/jmx_se:Body/jmx_se:Text", nsmgr);
            string Comment = Text == null ? "" : Text.InnerText;
            XmlNode ForeEnd = xml.SelectSingleNode("jmx:Report/jmx_ib:Head/jmx_ib:ValidDateTime", nsmgr);//津波予報の失効時刻
            string ValidDateTime = ForeEnd == null ? "" : $"\n{DateTime.Parse(ForeEnd.InnerText):yyyy/MM/dd HH:mm}まで有効";
            g.DrawString(Zen2Han($"{Time}    {Hypo}{SubHypo}\n{Location.Replace("深さ　", "深さ").Replace("　", " ")}  {Magnitude}\n{Comment.Replace("。　", "。\n").Replace("　", "")}{ValidDateTime}"), new Font("Koruri Regular", 18), Brushes.White, 10, 10);
            g.DrawString("気象データ・地図データ:気象庁", new Font("Koruri Regular", 20), Brushes.White, 680, 1040);
            Console.WriteLine("描画完了");

            bitmap.Save("img.png", ImageFormat.Png);
            BackgroundImage = bitmap;
            g.Dispose();
            //throw new Exception("デバック用");
            Console.WriteLine($"処理が完了しました。({DateTime.Now:HH:mm:ss.ff})");
        }
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
        /// <param name="text"></param>
        /// <returns></returns>
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
