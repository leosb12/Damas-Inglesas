using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace DamasInglesas
{
    class Clase_Ficha
    {
        public class Ficha
        {
            public string Color
            {
                get; set;
            }
            public string Tipo
            {
                get; set;
            }
            public Point Posicion
            {
                get; set;
            }
            public bool Reina
            {
                get; set;
            }

            public Ficha(string color, string tipo, Point posicion)
            {
                Color = color;
                Tipo = tipo;
                Posicion = posicion;
                Reina = false;

            }

            private void reina(string color)
            {
                if ((color == "azul" && Posicion.Y == 400) || (color == "roja" && Posicion.Y == 50))
                {
                    Reina = true;
                }

            }


        }

    }
}
