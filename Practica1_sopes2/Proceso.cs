using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Practica1_sopes2
{
    class Proceso
    {
        String nombre;
        ArrayList usando;
        bool alive;

        public Proceso(String n, bool v)
        {
            nombre = n;
            usando = new ArrayList();
            alive = v;
        }

        public bool get_alive()
        {
            return alive;
        }
        public void set_alive(bool a)
        {
            this.alive = a;
        }
        public bool contiene(String nombre)
        {
            return usando.Contains(nombre);
        }
        public void agregar(String nombrerecurso)
        {
            this.usando.Add(nombrerecurso);
            usando.Sort();
        }
        public void eliminar(String nombrerecurso)
        {
            this.usando.Remove(nombrerecurso);
        }
        public void limpiar()
        {
            this.usando.Clear();
        }
        public string[] get_usando()
        {
            return (string[])this.usando.ToArray(typeof(string));
        }
    }
}
