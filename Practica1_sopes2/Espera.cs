using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica1_sopes2
{
    class Espera
    {
        String proc, rec;
        public Espera(String p, String r){
            proc = p;
            rec = r;
        }

        public String get_proc()
        {
            return this.proc;
        }
        public String get_rec()
        {
            return this.rec;
        }
    }
}
