using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dados.Classes
{
    public class Tabela
    {
        public string Nome { get; set; }
        public List<Campo> Campos = new List<Campo>();
        
        public Tabela(string nome)
        {
            this.Nome = nome;
        }

        public Tabela()
        {

        }

        public bool existeChavePrimaria(ref string campo)
        {
            foreach (Campo item in Campos)
            {
                if (item.ChavePrimaria)
                {
                    campo = item.Nome;
                    return true;
                }
            }

            return false;
        }

        public bool existeChaveComposta(ref List<string> campos)
        {
            foreach (Campo item in this.Campos)
            {
                if (item.ChavePrimaria)
                {
                    campos.Add(item.Nome);
                }
            }

            if (campos.Count > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

    }

    public class Campo
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Tamanho { get; set; }
        public bool AceitaNullo { get; set; }
        public bool ChavePrimaria { get; set; }
        public bool AutoNumercao { get; set; }
        public string ValorPadrao { get; set; }
        public Campo(string nome, string tipo, string tamanho)
        {
            this.Nome = nome;
            this.Tamanho = tamanho;
            this.Tipo = tipo;
        }

        public Campo(string nome, string tipo, string tamanho, bool aceitaNulo)
        {
            this.Nome = nome;
            this.Tamanho = tamanho;
            this.AceitaNullo = aceitaNulo;
            this.Tipo = tipo;
        }

        public Campo(string nome, string tipo, string tamanho, bool aceitaNulo, string valorPadrao)
        {
            this.Nome = nome;
            this.Tamanho = tamanho;
            this.AceitaNullo = aceitaNulo;
            this.Tipo = tipo;
            this.ValorPadrao = valorPadrao;
        }

        public Campo(string nome, string tipo, string tamanho, bool aceitaNulo, bool chavePrimaria)
        {
            this.Nome = nome;
            this.Tamanho = tamanho;
            this.AceitaNullo = aceitaNulo;
            this.ChavePrimaria = chavePrimaria;
            this.Tipo = tipo;
        }
        public Campo(string nome, string tipo, string tamanho, bool aceitaNulo, bool chavePrimaria, bool autoNumeracao)
        {
            this.Nome = nome;
            this.Tamanho = tamanho;
            this.AceitaNullo = aceitaNulo;
            this.ChavePrimaria = chavePrimaria;
            this.AutoNumercao = autoNumeracao;
            this.Tipo = tipo;
        }
    }

    public static class tipoSQL
    {
        public static string Varchar() { return "VARCHAR"; }
        public static string Inteiro() { return "INT"; }
        public static string Dinheiro() { return "DECIMAL"; }
        public static string DataHora() { return "DATETIME"; }
        public static string Data() { return "DATE"; }
        public static string Imagem() { return "IMAGE"; }
        public static string VerdadeiroFalso() { return "BIT"; }
    }

    
}
