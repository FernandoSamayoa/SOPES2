using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;
namespace Practica1_sopes2
{
    class Gramatica : Irony.Parsing.Grammar
    {
        public Gramatica()
            : base(false)
        {



            RegexBasedTerminal tiempo = new RegexBasedTerminal("tiempo", "T[0-9]+");
            RegexBasedTerminal pro = new RegexBasedTerminal("proceso", "P[0-9]+");
            RegexBasedTerminal rec = new RegexBasedTerminal("recurso", "R[0-9]+");

            NonTerminal inicio = new NonTerminal("inicio");
            NonTerminal instruccion = new NonTerminal("instruccion");
            NonTerminal instrucciones = new NonTerminal("instrucciones");
            NonTerminal proceso = new NonTerminal("proceso");
            NonTerminal procesos = new NonTerminal("proceso");
            NonTerminal recurso = new NonTerminal("recurso");
            NonTerminal recursos = new NonTerminal("recursos");
            NonTerminal Tipo = new NonTerminal("tipo");

            inicio.Rule = instrucciones;

            instrucciones.Rule = MakeStarRule(instrucciones, instruccion);

            instruccion.Rule = tiempo + "(" + procesos + ")";

            procesos.Rule = MakeStarRule(procesos, ToTerm(","), proceso);
            proceso.Rule = pro + "(" + recursos + "|" + Tipo + ")"
                | pro + "(" + ToTerm("f") + ")";
            recursos.Rule = MakeStarRule(recursos, ToTerm(","), rec);

            Tipo.Rule = ToTerm("S") | ToTerm("L");

            this.Root = inicio;


        }
    }
}
