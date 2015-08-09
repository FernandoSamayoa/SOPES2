using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Irony.Parsing;
using Irony.Ast;
using System.Threading;

namespace Practica1_sopes2
{
    public partial class Form1 : Form
    {
        Hashtable mis_procesos, mis_recursos;
        ArrayList cola_espera;
        List<String> orden_de_recursos;
        bool e_circular, d_espera;
        public Form1()
        {
            InitializeComponent();
            mis_procesos = new Hashtable();
            mis_recursos = new Hashtable();
            cola_espera = new ArrayList();
            orden_de_recursos = new List<String>();
            e_circular = false;
            d_espera = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                e_circular = true;
            else
                e_circular = false;
            if (checkBox2.Checked)
                d_espera = true;
            else
                d_espera = false;
            mis_procesos.Clear();
            mis_recursos.Clear();
            cola_espera.Clear();
            orden_de_recursos.Clear();
            this.textBox2.Clear();
            Gramatica gramatica = new Gramatica();
            Parser parser = new Parser(gramatica);
            ParseTree raiz = parser.Parse(this.textBox1.Text);
            if (raiz.Root != null)
            {
                //primera recorrida
                almacenar(raiz.Root);
                //ordena los recursos de forma ascendete
                ordenar_recursos();
                ejecutar(raiz.Root);
            }
        }
        

        private String almacenar(ParseTreeNode nodo)
        {
            String retorno = "";
            if (nodo.Term.Name.Equals("inicio"))
            {
                almacenar(nodo.ChildNodes[0]);
            }
            else if (nodo.Term.Name.Equals("instrucciones"))
                {
                    if (nodo.ChildNodes.Count == 1)
                    {
                        almacenar(nodo.ChildNodes[0]);
                    }
                    else
                    {
                         for (int i = 0; i < nodo.ChildNodes.Count; i++)
                    {
                        almacenar(nodo.ChildNodes[i]);//ejecuta el primer proceso
                    }
                
                    }
                }
            else if (nodo.Term.Name.Equals("instruccion"))
            {
                almacenar(nodo.ChildNodes[2]);
            }
            else if (nodo.Term.Name.Equals("procesos"))
            {
                if (nodo.ChildNodes.Count == 1)
                {
                    almacenar(nodo.ChildNodes[0]);
                }
                else
                {
                    for (int i = 0; i < nodo.ChildNodes.Count; i++)
                    {
                        almacenar(nodo.ChildNodes[i]);//ejecuta el primer proceso
                    }
                }
            }
            else if (nodo.Term.Name.Equals("proceso"))
            {
                if (nodo.ChildNodes.Count > 1)
                {
                    if (!nodo.ChildNodes[0].Term.Name.Equals("proceso"))
                    {
                        String nproc = nodo.ChildNodes[0].Token.Value.ToString();
                        String estado = nodo.ChildNodes[4].ChildNodes[0].Token.Value.ToString();
                        c_proceso(nproc);
                        almacenar(nodo.ChildNodes[2]);
                    }
                    else
                    {
                        for (int i = 0; i < nodo.ChildNodes.Count; i++)
                        {
                            almacenar(nodo.ChildNodes[i]);
                        }
                    }
                }
                else
                {
                    

                    if (nodo.ChildNodes.Count==0)
                    {
                        String nproc = nodo.Token.Value.ToString();
                        c_proceso(nproc);
                    }
                    else
                    {
                       
                            almacenar(nodo.ChildNodes[0]);
                        
                    }
                }

            }
            else if (nodo.Term.Name.Equals("recursos"))
            {
                if (nodo.ChildNodes.Count == 1)
                {
                    //almacenar(nodo.ChildNodes[0]);
                    c_recurso(nodo.ChildNodes[0].Token.Value.ToString());
                }
                else
                {
                    /*almacenar(nodo.ChildNodes[0]);//obtiene recursos
                    almacenar(nodo.ChildNodes[2]);*/
                    for (int i = 0; i < nodo.ChildNodes.Count; i++)
                    {
                        c_recurso(nodo.ChildNodes[i].Token.Value.ToString());
                       
                    }
                }
            }
            else if (nodo.Term.Name.Equals("rec"))
            {
                retorno = nodo.Token.Value.ToString();
            }
            return retorno;
        }

        private String ejecutar(ParseTreeNode nodo)
        {
            String retorno = "";
            if (nodo.Term.Name.Equals("inicio"))
            {
                ejecutar(nodo.ChildNodes[0]);
            }
            else if (nodo.Term.Name.Equals("instrucciones"))
            {
                if (nodo.ChildNodes.Count == 1)
                {
                    ejecutar(nodo.ChildNodes[0]);
                }
                else
                {
                    for (int i = 0; i < nodo.ChildNodes.Count; i++)
                    {
                        ejecutar(nodo.ChildNodes[i]);//ejecuta el primer proceso
                    }
                }
                if (d_espera)
                    verificar_ciclos();
            }
            else if (nodo.Term.Name.Equals("instruccion"))
            {
                String tact = nodo.ChildNodes[0].Token.Value.ToString();
                //System.Diagnostics.Debug.WriteLine(tact + "-------------------------------------------");
                this.textBox2.AppendText(tact + "-------------------------------------------\n");
                //chequeamos la cola de esperas para ejecutar, en caso de ser necesario
                verificar_procesos();
                verificar_esperas();
                ejecutar(nodo.ChildNodes[2]);
                Thread.Sleep(900);
            }
            else if (nodo.Term.Name.Equals("procesos"))
            {
                if (nodo.ChildNodes.Count == 1)
                {
                    ejecutar(nodo.ChildNodes[0]);
                   
                }
                else
                {
                    for (int i = 0; i < nodo.ChildNodes.Count; i++)
                    {
                        ejecutar(nodo.ChildNodes[i]);//ejecuta el primer proceso
                        
                    }
                        
                }
            }
            else if (nodo.Term.Name.Equals("proceso"))
            {
                if (nodo.ChildNodes.Count > 1)
                {
                    if (nodo.ChildNodes.Count==6)
                    {
                        String nproc = nodo.ChildNodes[0].Token.Value.ToString();
                        String estado = nodo.ChildNodes[4].ChildNodes[0].Token.Value.ToString();
                        String asignados="";
                        
                        asignados = ejecutar(nodo.ChildNodes[2]);
                        operar(nproc, asignados, estado);
                        despertar_proceso(nproc);

                    }
                    else
                    {
                        if (nodo.ChildNodes.Count == 4 && nodo.ChildNodes[2].Term.Name.Equals("f"))
                        {
                            if(terminar_proc(nodo.ChildNodes[0].Token.Value.ToString()))
                            this.textBox2.AppendText(nodo.ChildNodes[0].Token.Value.ToString() + " ha dejado de ejecutarse" + "\n");
                        }
                        else
                        {
                            for (int i = 0; i < nodo.ChildNodes.Count; i++)
                            {
                                ejecutar(nodo.ChildNodes[i]);
                            }
                        }
                    }
                }
                else
                {


                    if (nodo.ChildNodes.Count == 0)
                    {
                        String nproc = nodo.Token.Value.ToString();
                        despertar_proceso(nproc);
                        c_proceso(nproc);
                    }
                    else
                    {

                        ejecutar(nodo.ChildNodes[0]);

                    }
                }
       
            }
            else if (nodo.Term.Name.Equals("recursos"))
            {
                if (nodo.ChildNodes.Count == 1)
                {
                    //ejecutar(nodo.ChildNodes[0]);
                    retorno += nodo.ChildNodes[0].Token.Value.ToString();
                }
                else
                {
                    /*ejecutar(nodo.ChildNodes[0]);//obtiene recursos
                    ejecutar(nodo.ChildNodes[2]);*/
                    for (int i = 0; i < nodo.ChildNodes.Count; i++)
                    {
                        retorno+= nodo.ChildNodes[i].Token.Value.ToString() + ",";

                    }
                }
            }
            else if (nodo.Term.Name.Equals("rec"))
            {
                retorno = nodo.Token.Value.ToString();
            }
            return retorno;
        }

        private void c_proceso(String nombre)
        {
            if (!mis_procesos.ContainsKey(nombre))
            {
                Proceso p = new Proceso(nombre,false);
                mis_procesos.Add(nombre, p);
            }
        }

        private void c_recurso(String nombre)
        {
            if (!mis_recursos.ContainsKey(nombre))
            {
                Recurso r = new Recurso(nombre,false);
                mis_recursos.Add(nombre, r);
            }
        }
        private void operar(String proc, String rec, String accion)
        {
            string[] split = rec.Split(new Char[] { ',' });
            switch (accion)
            {
                case "s":
                    foreach (string s in split) {
                        if (!s.Equals(""))
                        {
                            if (tasignar(proc, s))
                            {
                                //System.Diagnostics.Debug.WriteLine("se asigno " + s + " a " + proc);
                                this.textBox2.AppendText("se asigno " + s + " a " + proc + "\n");
                            }
                            else if(!e_circular)
                            {
                                this.textBox2.AppendText(proc + " espera a que se libere " + s + "\n");
                            }
                        }
                    }
                   
                    break;
                case "l":
                    foreach (string s in split)
                    {
                        if (!s.Equals(""))
                            if (tliberar(proc, s, false))
                            {
                                this.textBox2.AppendText("se libero " + s + " de " + proc + "\n");
                            }
                            else
                            {
                                this.textBox2.AppendText("No se libero " + s + " de " + proc + "\n");
                            }
                    }
                    break;
            }
        }
        private bool tasignar(String proc, String recurso)
        {
            bool flag = false;
            if (mis_procesos.ContainsKey(proc) && mis_recursos.ContainsKey(recurso))
            {
                Recurso r = (Recurso)mis_recursos[recurso];
                if (r.get_estado() == false)//SI ESTA LIBRE
                {
                    if (!e_circular)
                    {
                        //asigno
                        r.set_estado(true);
                        r.set_ejecutor(proc);
                        Proceso p = (Proceso)mis_procesos[proc];
                        p.agregar(recurso);
                        flag = true;
                    }
                    else
                    {
                        //chequeo que cumpla con la condicion de la espera circular antes de asignar
                        int prioridad_asignar = orden_de_recursos.IndexOf(recurso);//indice de prioridad del recurso que queremos asignar
                        //ahora vamos a obtener el indice MAYOR de prioridad que tiene asignado el proceso en este momento
                        Proceso p = (Proceso)mis_procesos[proc];
                        string[] milistadeprocesos = p.get_usando();
                        if (milistadeprocesos.Length != 0)
                        {
                            string nombre = milistadeprocesos[milistadeprocesos.Length - 1];
                            int prioridad_asignados = orden_de_recursos.IndexOf(nombre);
                            if (prioridad_asignar > prioridad_asignados)
                            {
                                //asigno
                                r.set_estado(true);
                                r.set_ejecutor(proc);
                                Proceso p2 = (Proceso)mis_procesos[proc];
                                p2.agregar(recurso);
                                flag = true;
                            }
                            else
                            {
                                //no puedo asignar y entro a cola de espera
                                this.textBox2.AppendText("No se puede asignar " + recurso + " a " + proc + " espera circular \n");
                                /*Espera e = new Espera(proc, recurso);
                                cola_espera.Add(e);*/
                                flag = false;
                            }
                        }
                        else
                        {//asigno
                            r.set_estado(true);
                            r.set_ejecutor(proc);
                            Proceso p3 = (Proceso)mis_procesos[proc];
                            p3.agregar(recurso);
                            flag = true;
                        }
                    }
                }
                else
                {
                    //ingresa a cola de espera
                    Espera e = new Espera(proc, recurso);
                    cola_espera.Add(e);
                }
            }   
            return flag;
        }

        private bool tliberar(String proc, String recurso, bool directo)
        {
            bool flag = false;
            if (mis_procesos.ContainsKey(proc) && mis_recursos.ContainsKey(recurso))
            {
                Recurso r = (Recurso)mis_recursos[recurso];
                if (r.get_estado() == true)//SI ESTA ocupado
                {
                    //libero
                    r.set_estado(false);
                    String actual = r.get_ejecutor();
                    r.set_ejecutor("");
                    Proceso p = (Proceso)mis_procesos[proc];
                    if (p.get_alive())
                    {
                        p.eliminar(recurso);
                        //mataria el recurso
                        //elimino las peticiones de espera del proceso que estoy apagando
                       /* for (int w = 0; w < cola_espera.Count; w++)
                         {
                             Espera es = (Espera)cola_espera[w];
                             if (es.get_proc().Equals(proc))
                             {
                                 cola_espera.RemoveAt(w);
                             }
                         }*/
                        //if (!directo)
                          //  terminar_proc(proc);
                        flag = true;
                    }
                    else 
                    {
                        flag = false;
                    }

                  
                        
                }
                else
                {
                    //ingresa a cola de espera
                }
            }
            return flag;
        }

        private bool terminar_proc(String nombre_proc)
        {
            bool flag = false;
            Proceso p = (Proceso)mis_procesos[nombre_proc];
          //  if (p.get_alive())
            //{
                string[] myarray = p.get_usando();
                p.limpiar();
                foreach (string s in myarray)
                {
                    tliberar(nombre_proc, s, true);
                    this.textBox2.AppendText("se libero " + s + " de " + nombre_proc + "\n");
                }
                p.set_alive(false);

                //elimino las peticiones de espera del proceso que estoy apagando
                 for (int w = 0; w < cola_espera.Count; w++)
                  {
                      Espera es = (Espera)cola_espera[w];
                      if (es.get_proc().Equals(nombre_proc))
                      {
                          cola_espera.RemoveAt(w);
                      }
                  }
                flag = true;
            //}
            //else
              //  this.textBox2.AppendText(nombre_proc + " ya se encuentra suspendido\n");
            return flag;
        }
        private void verificar_esperas()
        {
            if (cola_espera.Count != 0)
            {//si la cola de espera no es vacia
                for (int i = 0; i < cola_espera.Count; i++)
                {
                    Espera e = (Espera)cola_espera[i];
                    
                Recurso r = (Recurso)mis_recursos[e.get_rec()];
                if (r.get_estado() == false)//SI ESTA LIBRE
                {
                    if (tasignar(e.get_proc(), e.get_rec()))
                    {
                        cola_espera.RemoveAt(i);
                        this.textBox2.AppendText("se asigno " + e.get_rec() + " a " + e.get_proc() + "\n");
                    }
                }
                else
                {
                    this.textBox2.AppendText(e.get_proc() + " espera a que se libere " + e.get_rec() + "\n");
                }
                }
            }
        }

        private void despertar_proceso(String nombre)
        {
            Proceso p = (Proceso)mis_procesos[nombre];
            if ((!p.get_alive() || p.get_alive()) && !tiene_pendientes(nombre))
            {
                p.set_alive(true);
            }
            else
            {
                p.set_alive(false);
            }
        }

        private bool tiene_pendientes(String nproc)
        {
            bool flag = true;
            int tam = cola_espera.Count;
            if (tam == 0)
                flag = false;
            else
            {
                for (int i = 0; i < tam; i++)
                {
                    Espera e = (Espera)cola_espera[i];
                    if (e.get_proc().Equals(nproc))
                    {
                        flag = true;
                        break;
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }
                return flag;
        }
        private void verificar_procesos() 
        {
            foreach(DictionaryEntry llave in mis_procesos)
            {
                Proceso p = (Proceso)mis_procesos[llave.Key.ToString()];
                if (p.get_alive() && !tiene_pendientes(llave.Key.ToString()))
                    this.textBox2.AppendText(llave.Key.ToString() + " se esta ejecutando \n");
            }
            
        }
        private void verificar_ciclos()
        {
            //for a cola de espera, para formar circulo -> cuadrado
            //ver poseedor de cuadrado para formar cuadrado -> circulo
            //con estos formar el arbol de interdependencia de procesos
            int tam_cola_espera = cola_espera.Count;
            Stack<string> pasos_ciclo = new Stack<string>();
            for (int i= 0; i < tam_cola_espera; i++)
            {
                Espera e = (Espera)cola_espera[i];
                String papa = e.get_proc();
                Recurso r = (Recurso)mis_recursos[e.get_rec()];
                String hijo = r.get_ejecutor();
                //this.textBox2.AppendText("papa: " + papa + " hijo: " + hijo + "\n");
                if(!pasos_ciclo.Contains(papa))
                pasos_ciclo.Push(papa);
                if (!pasos_ciclo.Contains(hijo))
                    pasos_ciclo.Push(hijo);
                else
                    this.textBox2.AppendText("*** hay interbloqueo *** \n");
            }
        }
        private void ordenar_recursos()
        {
            if (e_circular)
            {
                foreach (DictionaryEntry llave in mis_recursos)
                {
                    orden_de_recursos.Add(llave.Key.ToString());
                }
                orden_de_recursos.Sort();
            }
        }
    }
}
