using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica1_sopes2
{
    class Arbol
    {
        String nombre;
        ArrayList asociados;
        public Arbol(String no)
        {
            asociados = new ArrayList();
            nombre = no;
        }

        public void set_nombre(String valor){
            this.nombre = valor;
        }
        public String get_nombre()
        {
            return this.nombre;
        }
        public void agregar(Arbol nodo)
        {
            asociados.Add(nodo);
        }
        public Arbol[] get_asociados()
        {
            return (Arbol[])this.asociados.ToArray(typeof(Arbol));
        }
    }
}
