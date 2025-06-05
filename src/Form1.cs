
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace DamasInglesas
{
    public partial class Form1 : Form
    {
        int turno = 0;
        int movimientosRojo = 0;
        int movimientosAzul = 0;
        bool movExtra = false;
        PictureBox seleccionado = null;
        PictureBox[] azules = new PictureBox[13];
        PictureBox[] rojas = new PictureBox[13];
        Stopwatch cronometro = new Stopwatch();
        Timer timer = new Timer();

        public Form1()
        {
            InitializeComponent();
            cargarArreglos();

            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            label17.Text = "Turno del equipo ROJO";
            timer.Interval = 1;
            timer.Tick += Timer_Tick;
        }

        private void cargarArreglos()
        {
            azules[1] = azul1; azules[2] = azul2; azules[3] = azul3; azules[4] = azul4;
            azules[5] = azul5; azules[6] = azul6; azules[7] = azul7; azules[8] = azul8;
            azules[9] = azul9; azules[10] = azul10; azules[11] = azul11; azules[12] = azul12;

            rojas[1] = roja1; rojas[2] = roja2; rojas[3] = roja3; rojas[4] = roja4;
            rojas[5] = roja5; rojas[6] = roja6; rojas[7] = roja7; rojas[8] = roja8;
            rojas[9] = roja9; rojas[10] = roja10; rojas[11] = roja11; rojas[12] = roja12;
        }

        public void seleccion(object objeto)
        {
            if (!movExtra)
            {
                try { seleccionado.BackColor = Color.Black; } catch { }
                seleccionado = (PictureBox)objeto;
                seleccionado.BackColor = Color.Lime;
            }
        }

        private void cuadroClick(object sender, MouseEventArgs e)
        {
            movimiento((PictureBox)sender);
        }

        private string ConvertirAPosicion(Point posicion)
        {
            char letra = (char)('A' + (posicion.X - 50) / 50);
            int numero = 8 - (posicion.Y - 50) / 50;
            return $"{letra}{numero}";
        }

        private void MostrarMovimiento(Point desde, Point hacia, string color)
        {
            string movimiento = $"{ConvertirAPosicion(desde)} - {ConvertirAPosicion(hacia)}";
            if (color == "roja") textBox1.AppendText(movimiento + Environment.NewLine);
            else textBox2.AppendText(movimiento + Environment.NewLine);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (cronometro.IsRunning)
            {
                TimeSpan t = cronometro.Elapsed;
                label18.Text = $"{t.Minutes:00}:{t.Seconds:00}:{t.Milliseconds / 10:00}";
            }
        }

        private bool FinDelJuego()
        {
            bool sinAzul = azules.All(f => f == null);
            bool sinRojo = rojas.All(f => f == null);

            if (sinAzul || sinRojo)
            {
                cronometro.Stop();
                string ganador = sinAzul ? "ROJO" : "AZUL";

                MessageBox.Show($"\u00a1El equipo {ganador} ha ganado!", "Juego Terminado");
                DialogResult reiniciar = MessageBox.Show("¿Desea iniciar una nueva partida?", "Reiniciar", MessageBoxButtons.YesNo);
                if (reiniciar == DialogResult.Yes)
                {
                    Application.Restart();
                }
                else
                {
                    Application.Exit();
                }
                return true;
            }
            return false;
        }

        private void MostrarTurno()
        {
            label17.Text = movExtra ? "\u00a1Debe continuar comiendo!" :
                (turno % 2 == 0 ? "Turno del equipo ROJO" : "Turno del equipo AZUL");
        }

        private void movimiento(PictureBox cuadro)
        {
            if (!cronometro.IsRunning)
            {
                cronometro.Start();
                timer.Start();
            }

            if (seleccionado != null)
            {
                string color = seleccionado.Name.StartsWith("roja") ? "roja" : "azul";

                if (validacion(seleccionado, cuadro, color))
                {
                    Point anterior = seleccionado.Location;
                    seleccionado.Location = cuadro.Location;

                    new System.Media.SoundPlayer("Sonidos/movimientowav.wav").Play();

                    MostrarMovimiento(anterior, cuadro.Location, color);
                    ifqueen(color);

                    int avance = Math.Abs(anterior.Y - cuadro.Location.Y);
                    if (avance == 100 && movimientoExtras(color))
                    {
                        movExtra = true;
                    }
                    else
                    {
                        if (color == "roja") movimientosRojo++;
                        else movimientosAzul++;

                        turno++;
                        seleccionado.BackColor = Color.Black;
                        seleccionado = null;
                        movExtra = false;
                    }

                    ActualizarMovimientoEnPantalla(color);
                    if (FinDelJuego()) return;
                    MostrarTurno();
                }
            }
        }

        private void ActualizarMovimientoEnPantalla(string color)
        {
            label17.Text = $"Turno actual: {(movExtra ? color.ToUpper() : (turno % 2 == 0 ? "ROJO" : "AZUL"))}";
            labelMovRojo.Text = $"Movimientos ROJO: {movimientosRojo}";
            labelMovAzul.Text = $"Movimientos AZUL: {movimientosAzul}";
        }

        private void ifqueen(string color)
        {
            if (color == "azul" && seleccionado.Location.Y == 400)
            {
                new System.Media.SoundPlayer("Sonidos/reina.wav").Play();
                seleccionado.BackgroundImage = Image.FromFile("Imagenes/AzulC.png");
                seleccionado.Tag = "reina";
            }
            if (color == "roja" && seleccionado.Location.Y == 50)
            {
                new System.Media.SoundPlayer("Sonidos/reina.wav").Play();
                seleccionado.BackgroundImage = Image.FromFile("Imagenes/RojoC.png");
                seleccionado.Tag = "reina";
            }
        }

        private bool movimientoExtras(string color)
        {
            Point[] posiciones = new Point[seleccionado.Tag == "reina" ? 4 : 2];
            int dir = color == "roja" ? -100 : 100;

            posiciones[0] = new Point(seleccionado.Location.X + 100, seleccionado.Location.Y + dir);
            posiciones[1] = new Point(seleccionado.Location.X - 100, seleccionado.Location.Y + dir);

            if (seleccionado.Tag == "reina")
            {
                posiciones[2] = new Point(seleccionado.Location.X + 100, seleccionado.Location.Y - dir);
                posiciones[3] = new Point(seleccionado.Location.X - 100, seleccionado.Location.Y - dir);
            }

            PictureBox[] bandoContrario = color == "roja" ? azules : rojas;

            foreach (Point pos in posiciones)
            {
                if (pos.X >= 50 && pos.X <= 400 && pos.Y >= 50 && pos.Y <= 400)
                {
                    Point mid = new Point(promedio(pos.X, seleccionado.Location.X), promedio(pos.Y, seleccionado.Location.Y));
                    if (!ocupado(pos, bandoContrario) && ocupado(mid, bandoContrario)) return true;
                }
            }
            return false;
        }

        private bool ocupado(Point punto, PictureBox[] bando)
        {
            foreach (var ficha in bando)
                if (ficha != null && ficha.Location == punto) return true;
            return false;
        }

        private int promedio(int n1, int n2) => Math.Abs((n1 + n2) / 2);

        private bool validacion(PictureBox origen, PictureBox destino, string color)
        {
            int avance = origen.Location.Y - destino.Location.Y;
            avance = color == "roja" ? avance : -avance;
            if (origen.Tag == "reina") avance = Math.Abs(avance);

            if (avance == 50)
            {
                if (PuedeComer(color))
                {
                    MessageBox.Show("Debe capturar si es posible.");
                    return false;
                }
                return true;
            }

            if (avance == 100)
            {
                Point mid = new Point(promedio(destino.Location.X, origen.Location.X), promedio(destino.Location.Y, origen.Location.Y));
                PictureBox[] bandoContrario = color == "roja" ? azules : rojas;
                for (int i = 1; i < bandoContrario.Length; i++)
                {
                    if (bandoContrario[i] != null && bandoContrario[i].Location == mid)
                    {
                        new System.Media.SoundPlayer("Sonidos/error.wav").Play();
                        bandoContrario[i].Location = new Point(0, 0);
                        bandoContrario[i].Visible = false;
                        bandoContrario[i] = null;
                        return true;
                    }
                }
            }
            return false;
        }

        private void seleccionRoja(object sender, MouseEventArgs e)
        {
            if (turno % 2 == 0 || (movExtra && seleccionado?.Name.StartsWith("roja") == true))
                seleccion(sender);
            else
            MessageBox.Show("Turno del equipo AZUL");

        }

        private void seleccionAzul(object sender, MouseEventArgs e)
        {
            if (turno % 2 == 1 || (movExtra && seleccionado?.Name.StartsWith("azul") == true))
                seleccion(sender);
            else
                MessageBox.Show("Turno del equipo ROJO");
        }

        private bool PuedeComer(string color)
        {
            PictureBox[] fichas = color == "roja" ? rojas : azules;
            PictureBox[] enemigos = color == "roja" ? azules : rojas;
            int dir = color == "roja" ? -100 : 100;

            foreach (var ficha in fichas)
            {
                if (ficha == null) continue;
                bool esReina = ficha.Tag?.ToString() == "reina";
                Point[] posiblesSaltos = new Point[esReina ? 4 : 2];

                posiblesSaltos[0] = new Point(ficha.Location.X + 100, ficha.Location.Y + dir);
                posiblesSaltos[1] = new Point(ficha.Location.X - 100, ficha.Location.Y + dir);
                if (esReina)
                {
                    posiblesSaltos[2] = new Point(ficha.Location.X + 100, ficha.Location.Y - dir);
                    posiblesSaltos[3] = new Point(ficha.Location.X - 100, ficha.Location.Y - dir);
                }

                foreach (var destino in posiblesSaltos)
                {
                    if (destino.X < 50 || destino.X > 400 || destino.Y < 50 || destino.Y > 400) continue;
                    Point medio = new Point(promedio(ficha.Location.X, destino.X), promedio(ficha.Location.Y, destino.Y));
                    if (!ocupado(destino, rojas) && !ocupado(destino, azules) && ocupado(medio, enemigos))
                    {
                        return true;
                    }
                }
            }
            return false;
        }



        private void azul12_Click(object sender, EventArgs e)
        {

        }

        private void azul1_Click(object sender, EventArgs e)
        {

        }

        private void azul5_Click(object sender, EventArgs e)
        {

        }

        private void azul9_Click(object sender, EventArgs e)
        {

        }

        private void azul10_Click(object sender, EventArgs e)
        {

        }

        private void azul2_Click(object sender, EventArgs e)
        {

        }

        private void azul6_Click(object sender, EventArgs e)
        {

        }

        private void azul3_Click(object sender, EventArgs e)
        {

        }

        private void azul11_Click(object sender, EventArgs e)
        {

        }

        private void azul7_Click(object sender, EventArgs e)
        {

        }

        private void azul8_Click(object sender, EventArgs e)
        {

        }

        private void azul4_Click(object sender, EventArgs e)
        {

        }

        private void roja8_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox49_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox58_Click(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

    }
}
