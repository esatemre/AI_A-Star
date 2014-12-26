using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YapayZeka1
{
    public partial class Form1 : Form
    {
        
        private Panel[,] _chessBoardPanels;// Dama şeklinde boyamak için panel matrisi
        private byte first;// başlangıç ve bitiş noktalarını ayırmak için flag       
        private int basX, basY, bitX, bitY;// başlangıç ve bitiş koordinatları
        private Dictionary<Point, Int32> list;// gidebildiğimiz noktalar yani ağacımız      
        private List<Point> used;// gittiğimiz noktalar
        /// <summary>
        /// at için offsetler
        /// </summary>
        private static readonly List<Point> positions = new List<Point>()
        {
            new Point(){x=-1, y=-4},
            new Point(){x=-4, y=-1},
            new Point(){x=-4, y= 1},
            new Point(){x=-1, y= 4},
            new Point(){x= 1, y= 4},
            new Point(){x= 4, y= 1},
            new Point(){x= 4, y=-1},
            new Point(){x= 1, y=-4},
        };
        private const int tileSize = 40;
        private const int gridSize = 12;// 12x12 tahta

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            first = 0;
            list = new Dictionary<Point, Int32>();
            used = new List<Point>();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Paint();
        }

        /// <summary>
        /// panele tıkladığında baş ve bit noktalarını almak için
        /// </summary>
        void newPanel_Click(object sender, EventArgs e)
        {
            Panel pan = ((Panel)sender);
            string[] str = pan.Name.Split('_');
            if (first == 0)
            {
                pan.BackColor = Color.SteelBlue;
                pan.Controls.Add(new Label() { Text = "START" });
                txtBas.Text = (Int32.Parse(str[0]) + 1).ToString() + "," + (Int32.Parse(str[1]) + 1).ToString();
                first = 1;
            }
            else if (first == 1)
            {
                pan.BackColor = Color.SteelBlue;
                pan.Controls.Add(new Label() { Text = "END" });
                txtBit.Text = (Int32.Parse(str[0]) + 1).ToString() + "," + (Int32.Parse(str[1]) + 1).ToString();
                first = 2;
            }
        }

        /// <summary>
        /// temizle butonu
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            first = 0;
            txtBas.Text = "";
            txtBit.Text = "";
            txtTime.Text = "";
            txtMax.Text = "";
            list = new Dictionary<Point, Int32>();
            used = new List<Point>();
            panel1.Controls.Clear();
            Paint();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Run();
        }

        /// <summary>
        /// help
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Damalı karelerin üstüne tıklayarak veya \nkoordinatları kutucuğa girerek\nistediğiniz başlangıç ve bitiş noktasını seçebilirsiniz.\nİlk Nokta İçin Koordinat Örneği : 1,1");
        }

        /// <summary>
        /// dama şekli oluşturmak için fonk
        /// </summary>
        private void Paint()
        {
            var clr1 = Color.DarkGray;
            var clr2 = Color.White;

            // initialize the "chess board"
            _chessBoardPanels = new Panel[gridSize, gridSize];

            // double for loop to handle all rows and columns
            for (var n = 0; n < gridSize; n++)
            {
                for (var m = 0; m < gridSize; m++)
                {
                    // create new Panel control which will be one
                    // chess board tile
                    var newPanel = new Panel
                    {
                        Name = Convert.ToString(n + "_" + m),
                        Size = new Size(tileSize, tileSize),
                        Location = new System.Drawing.Point(tileSize * n, tileSize * m)
                    };
                    newPanel.Click += newPanel_Click;
                    // add to Form's Controls so that they show up
                    panel1.Controls.Add(newPanel);

                    // add to our 2d array of panels for future use
                    _chessBoardPanels[n, m] = newPanel;

                    // color the backgrounds
                    if (n % 2 == 0)
                        newPanel.BackColor = m % 2 != 0 ? clr1 : clr2;
                    else
                        newPanel.BackColor = m % 2 != 0 ? clr2 : clr1;
                }
            }
        }

        /// <summary>
        /// başlangıça uzaklık alma
        /// </summary>
        private int BasDistance(int x, int y)
        {
            return Math.Abs(basX - x) + Math.Abs(basY - y);
        }

        /// <summary>
        /// bitişe uzaklık alma
        /// </summary>
        private int BitDistance(int x, int y)
        {
            return Math.Abs(bitX - x) + Math.Abs(bitY - y);
        }

        /// <summary>
        /// bir noktadan daha önce geçilmemiş hangi noktalara gidilebilinir
        /// </summary>
        private void CanGoWhere(Point nok)
        {
            foreach (var item in positions)
            {
                int a = item.x + nok.x;
                int b = item.y + nok.y;
                if ((a < gridSize) && (b < gridSize) && (b >= 0) && (a >= 0))
                {
                    if (!used.Any(p => (p.x == a) && (p.y == b)))
                    {
                        list.Add(new Point(a, b, (nok.level + 1), nok), (BasDistance(a, b) + BitDistance(a, b)));
                    }
                }
            }
        }

        /// <summary>
        /// Yol bulma öncesi kontroller
        /// </summary>
        private void Run()
        {
            if (txtBas.Text.Split(',').Length != 2 || txtBit.Text.Split(',').Length != 2)
            {
                MessageBox.Show("Lütfen Düzgün Giriniz");
            }
            else
            {
                string[] str1 = txtBas.Text.Split(',');
                string[] str2 = txtBit.Text.Split(',');
                bitX = Int32.Parse(str2[0]) - 1;
                bitY = Int32.Parse(str2[1]) - 1;
                basX = Int32.Parse(str1[0]) - 1;
                basY = Int32.Parse(str1[1]) - 1;
                if (!(bitX > -1 && bitX < 12 && basX > -1 && basX < 12 && bitY > -1 && bitY < 12 && basY > -1 && basY < 12))
                {
                    MessageBox.Show("Lütfen Düzgün Giriniz");
                }
                else
                {
                    FindPath();
                }
            }
        }

        /// <summary>
        /// Yolu bulur
        /// </summary>
        private void FindPath() 
        {
            DateTime time1 = DateTime.Now; //başlangıç zamanı
            Point where = new Point(basX, basY, 0, null); //agactaki kok nokta belirlenir
            while (where.x != bitX || where.y != bitY) // hedef bulunana kadar don
            {
                CanGoWhere(where); // buradan nerelere gidilir
                Point node = new Point();
                used.Add(where); //burayı kullanılmıslara attık
                foreach (var item in list.OrderBy(p => p.Value)) // en dusuk maliyete sahip ve
                {
                    if (!used.Any(p => p.x == item.Key.x && p.y == item.Key.y)) //daha once kullanılmayan
                    {
                        node = item.Key; //noktayı sec
                        break;
                    }
                }
                where = node;
            }
            DateTime time2 = DateTime.Now; // bitis zamanı
            bool isEnd = false;
            while (where.root != null)
            {
                //Panel'e yazdır!
                Panel pnl = (Panel)panel1.Controls.Find(where.x + "_" + where.y, true).First();
                if (!isEnd)
                {
                    pnl.Controls.Add(new Label() { Text = "END" });
                    pnl.BackColor = Color.SteelBlue;
                    isEnd = true;
                }
                else
                    pnl.Controls.Add(new Label() { Text = where.level.ToString() });
                Point tmp = where.root;
                where = tmp;
            }
            Panel pane = (Panel)panel1.Controls.Find(basX + "_" + basY, true).First();
            pane.Controls.Add(new Label() { Text = "START" });
            pane.BackColor = Color.SteelBlue;
            TimeSpan time = (time2 - time1);
            txtTime.Text = time.Minutes + ":" + time.Seconds + ":" + time.Milliseconds;
            txtMax.Text = (GC.GetTotalMemory(true) / 1024).ToString() + " kB";
        }


        


    }
}
