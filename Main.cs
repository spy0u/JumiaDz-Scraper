using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;
using Utilities.BunifuButton.Transitions;


namespace JumiaDz_Scraper
{
    public partial class Main : Form
    {
        // Source Code is Available for Developpers

        #region Constructor
        public Main()
        {
            InitializeComponent();
        }
        #endregion

        #region Fields

        private bool _drag;
        private int _mousey;
        private int _mousex;
        private bool _stop = false;
        
        #endregion

        #region Events
        private void pb_About(object sender, EventArgs e)
        {
            MessageBox.Show("Coded By SPYOU" + Environment.NewLine+ "LA ILAHA ILLA ALLAH" , "Infos",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void pb_Minimize(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Show();
        }

        private void pb_Exit(object sender, EventArgs e)
        {
            Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstPrix.SelectedIndex = lstPrd.SelectedIndex;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstPrd.SelectedIndex = lstPrix.SelectedIndex;

        }

        private void ButtonJDZS_Search_Click_1(object sender, EventArgs e)
        {
            tlsUpdate();
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                GetInfos("h3", "class", "name", "div", "class", "prc");
            }
            else
            {
                MessageBox.Show("Please type a product name", "Search is Empty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int count = lstPrd.Items.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                if (lstPrix.Items[i].ToString() == "") { lstPrix.Items.RemoveAt(i); }
                if (lstPrd.Items[i].ToString() == "") { lstPrd.Items.RemoveAt(i); }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            tstpStat.Text = Counetr.Text;
        }


        #endregion

        #region Methods

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            _drag = false;
            Cursor = Cursors.Default;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_drag)
            {
                Cursor = Cursors.SizeAll;
                ShowVisualMovementCues();

                Top = Cursor.Position.Y - _mousey;
                Left = Cursor.Position.X - _mousex;
            }
            else
            {
                Cursor = Cursors.Default;
                HideVisualMovementCues();
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            _drag = true;
            Cursor = Cursors.Default;

            _mousex = Cursor.Position.X - Left;
            _mousey = Cursor.Position.Y - Top;
        }

        private void HideVisualMovementCues()
        {
            Opacity = 1.0;
        }

        private void ShowVisualMovementCues()
        {
            Opacity = 0.8;
        }

        public new void Show()
        {
            Opacity = 0.0;
            base.Show();
            Transition transition = new Transition(new TransitionType_EaseInEaseOut(700));
            transition.add(this, "Opacity", 1.0);
            transition.run();
        }

        public new void Close()
        {
            Transition transition = new Transition(new TransitionType_EaseInEaseOut(220));
            transition.add(this, "Opacity", 0.0);
            transition.run();
            transition.TransitionCompletedEvent += delegate
            {
                base.Close();
                Application.Exit();
            };

            Transition.run(this, "Top", Top + (Height / 4), new TransitionType_EaseInEaseOut(270));
        }

        private async void GetInfos(string a1, string a2, string a3, string b1, string b2, string b3)
        {
            lstPrd.Items.Clear();
            lstPrix.Items.Clear();
            _stop = false;



            int PageNumber = 1;
            string JumiDz = "https://www.jumia.dz/catalog/?q=";
            string Search = txtSearch.Text;
            HttpClient httpClient = new HttpClient();

            while (_stop == false)
            {

                string CompleteLink = JumiDz + Search + "&page=" + PageNumber + "#catalog-listing";
                string html = await httpClient.GetStringAsync(CompleteLink);
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                List<HtmlNode> Produits = doc.DocumentNode.Descendants(a1)
                   .Where(node => node.GetAttributeValue(a2, "").Equals(a3)).ToList();// you can use contains also

                List<HtmlNode> Prix = doc.DocumentNode.Descendants(b1)
                   .Where(node => node.GetAttributeValue(b2, "").Equals(b3)).ToList();// you can use contains also


                foreach (HtmlNode Prod in Produits)
                {
                    if (Produits.Count <= 0)
                    {
                        _stop = true;
                    }

                    //dataGridView1.Rows.Add(Prod.InnerText);
                    lstPrd.Items.Add(Prod.InnerText);
                }

                foreach (HtmlNode Pri in Prix)
                {

                    if (Prix.Count <= 0)
                    {
                        //tstpStat.Text = "Status : Done.";
                        _stop = true;

                    }

                    lstPrix.Items.Add(Pri.InnerText);
                    Counetr.Text = "Scraped Products: " + Convert.ToString(lstPrd.Items.Count);
                }

                PageNumber = PageNumber + 1;
            }
        }

        public void tlsUpdate()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
        }


        #endregion

        
    }



}
