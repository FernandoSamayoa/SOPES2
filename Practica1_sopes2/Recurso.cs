using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica1_sopes2
{
    class Recurso
    {
        String nombre;
        String ejecutor;
        bool estado;
        public Recurso(String n, bool b)
        {
            nombre = n;
            estado = b;
            ejecutor = "";
        }

        public bool get_estado()
        {
            return this.estado;
        }
        public void set_estado(bool s)
        {
            this.estado = s;
        }
        public String get_ejecutor()
        {
            return this.ejecutor;
        }
        public void set_ejecutor(String name)
        {
            this.ejecutor = name;
        }
    }
}
