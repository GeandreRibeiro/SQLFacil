using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dados.Classes
{
    public class TabelaSQL
    {
        public TabelaSQL(string nome)
        {
            this.Nome = nome;
        }
        public TabelaSQL()
        {

        }

        public string Nome { get; set; }
        public List<CamposSql> Campos = new List<CamposSql>();
        public List<OndeSql> Onde = new List<OndeSql>();
        public List<OrdemSQL> Ordem = new List<OrdemSQL>();
        public List<juncao> Jucao = new List<juncao>();
    }

    public class CamposSql
    {
        public string Nome { get; set; }
        public string Valor { get; set; }
        public string Tipo { get; set; }

        public CamposSql(string nome, string valor, string tipo)
        {
            this.Nome = nome;
            this.Valor = valor;
            this.Tipo = tipo;
        }
    }
    public class CamposJuncao
    {
        public string CampoPrincipal { get; set; }
        public string CamposSecundario { get; set; }

        public CamposJuncao(string campoPrincipal, string campoSecundario)
        {
            this.CampoPrincipal = campoPrincipal;
            this.CamposSecundario = campoSecundario;
        }
    }
    public class OndeSql
    {
        public string Campo { get; set; }
        public string Operador { get; set; }
        public string Valor { get; set; }
        public string Tipo { get; set; }
        public string EOu { get; set; }

        public OndeSql(string campo, string operador, string valor, string tipo, string eou)
        {
            this.Campo = campo;
            this.Operador = operador;
            this.Valor = valor;
            this.Tipo = tipo;
            this.EOu = eou;
        }
        public OndeSql(string campo, string operador, string valor, string tipo)
        {
            this.Campo = campo;
            this.Operador = operador;
            this.Valor = valor;
            this.Tipo = tipo;
        }
    }

    public static class OperadorSQL
    {
        public static string Igual() { return "="; }
        public static string Maior() { return ">"; }
        public static string Menor() { return "<"; }
        public static string Diferente() { return "<>"; }
        public static string Como() { return "LIKE";}
        public static string MaiorIgual() { return ">="; }
        public static string MenorIgual() { return "<="; }
    }

    public static class EOuSQL
    {
        public static string E() { return "AND"; }
        public static string Ou() { return "OR"; }
    }

    public class OrdemSQL
    {
        public OrdemSQL(string campo)
        {
            this.Campo = campo;
        }
        public OrdemSQL()
        {
        }

        public string Campo { get; set; }
    }

    public class juncao
    {
        //construtor vazio
        public juncao()
        {

        }
        //construtor simples
        public juncao(string tabelaPrincipal, string campoPrincipal, string tabelaSecundaria, string campoSecundario)
        {
            //Tabela e campos principais
            this.TabelaPrincipal = tabelaPrincipal;
            this.Campos.Add(new CamposJuncao(campoPrincipal, campoSecundario));
            
            //tabela secundaria
            this.TabelaSecundaria = tabelaSecundaria;
            
        }
        //construtor complexo
        public juncao(string tabelaPrincipal, List<CamposJuncao> campos, string tabelaSecundaria)
        {
            //tabela e campos principais
            this.TabelaPrincipal = tabelaPrincipal;
            //campos da junção
            foreach (CamposJuncao item in campos)
            {
                this.Campos.Add(item);
            }

            //tabelas e campos secundarios
            this.TabelaSecundaria = tabelaSecundaria;
            
        }

        public string TabelaPrincipal { get; set; }
        public List<CamposJuncao> Campos = new List<CamposJuncao>();
        public string TabelaSecundaria { get; set; }
        
    }
}
