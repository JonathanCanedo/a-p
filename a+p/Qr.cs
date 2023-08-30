using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QRCoder.PayloadGenerator;
using MySql.Data.MySqlClient;

namespace a_p{
    public partial class Qr : Form{
        public string a="";
        public int xClick = 0, yClick = 0;
        public Qr(){
            InitializeComponent();
        }
        private void Qr_Load(object sender, EventArgs e){
            lblFecha.Text = DateTime.Now.ToShortDateString();
            qr();
            Envios();
        }
        private void timer1_Tick(object sender, EventArgs e){
            lblHora.Text = DateTime.Now.ToString("HH:mm");
        }
        private void timer2_Tick(object sender, EventArgs e){
            qr();
        }
        private void qr(){
            QRCodeGenerator qrGenerador = new QRCodeGenerator();
            QRCodeData qrdat = qrGenerador.CreateQrCode(lblHora.Text = DateTime.Now.ToString("HH:mm"), QRCodeGenerator.ECCLevel.H);
            QRCode codigo = new QRCode(qrdat);

            Bitmap qrI = codigo.GetGraphic(9, Color.Blue, Color.Empty, true);
            pbQr.Image = qrI;
        }
        private void iconCerrar_Click(object sender, EventArgs e){
            DialogResult res = MessageBox.Show("¿ Realmente desea cerrar la aplicación ?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes){
                Environment.Exit(0);
            }else { }
        }
        private void iconMin_Click(object sender, EventArgs e){
            this.WindowState = FormWindowState.Minimized;
        }
        private void label3_MouseMove(object sender, MouseEventArgs e){
            if (e.Button != MouseButtons.Left){
                xClick = e.X; yClick = e.Y;
            }else{
                this.Left = this.Left + (e.X - xClick); this.Top = this.Top + (e.Y - yClick);
            }
        }
        private void Envios(){
            char[] cadena = lblHora.Text.ToCharArray();
            a = cadena[0] + "" + cadena[1];

            if (Convert.ToInt32(a) == 13){
                try{
                    MySqlConnection con = new MySqlConnection("datasource=127.0.0.1;port=3306;username=root;password=;database=a+p");
                    int c = Contar();
                    List<string> macs = new List<string>();

                    con.Open();
                    string Query = ("select mac from trab");
                    MySqlCommand cmd = new MySqlCommand(Query, con);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()){
                        macs.Add(Convert.ToString(reader["mac"]));
                    }
                    con.Close();
                    for (int i = 0; i < c; i++){
                        if (!existe(macs[i],lblFecha.Text)){
                            string consu = "INSERT INTO registros (mac,fecha,entrada,salida) VALUES (@mac,@fecha,@entrada,@salida)";
                            cmd = new MySqlCommand(consu, con);
                            
                            #region Datos
                            cmd.Parameters.AddWithValue("@mac", macs[i]);
                            cmd.Parameters.AddWithValue("@fecha", lblFecha.Text);
                            cmd.Parameters.AddWithValue("@entrada", "0");
                            cmd.Parameters.AddWithValue("@salida", "0");                            
                            #endregion

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }else { }
                    }
                }catch { }
            }else{}
        }
        private int Contar(){
            int s = 0;
            try{
                MySqlConnection con = new MySqlConnection("datasource=127.0.0.1;port=3306;username=root;password=;database=a+p");
                con.Open();
                string Query = ("select count(mac) from trab");
                MySqlCommand cmd = new MySqlCommand(Query, con);
                s = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
            }catch { }
            return s;
        }
        private void timer3_Tick(object sender, EventArgs e){
            Envios();
        }
        private bool existe(string mac, string fecha){
            MySqlConnection con = new MySqlConnection("datasource=127.0.0.1;port=3306;username=root;password=;database=a+p");
            string query = "SELECT COUNT(*) FROM registros WHERE mac=@mac AND fecha=@fecha";
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("mac", mac);
            cmd.Parameters.AddWithValue("fecha", fecha);
            con.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count == 0){
                return false;
            }else{
                return true;
            }
        }
            
    }
}