using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Dados.Classes;
using System.Data;

namespace Dados
{
    public class Erros
    {
        private const string _tabela = "Erros";
        
        public int Id { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        
        public static void catalogarErro(string mensagem, string objeto, string evento, Conexao conexao)
        {
            string msg = mensagem + ", em " + objeto + "->" + evento;
            MessageBox.Show(msg, "Erp", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Erros err = new Erros(DateTime.Now, msg, conexao);
        }

        public List<Erros> listErro = new List<Erros>();

        private Conexao Conexao;

        public Erros(DateTime data, string descricao, Conexao conexao)
        {
            this.Conexao = conexao;
            manutencaoTabela();
            gravarDados(data, descricao);
        }

        public Erros(Conexao conexao)
        {
            this.Conexao = conexao;
        }

        private bool manutencaoTabela()
        {
            try
            {
                if(!MeuSQL.existeTabela(_tabela, this.Conexao))
                {
                    Tabela tabela = new Tabela(_tabela);
                    tabela.Campos.Add(new Campo("Id", tipoSQL.Inteiro(), "", false, true, true));
                    tabela.Campos.Add(new Campo("Data", tipoSQL.Data(), "", false));
                    tabela.Campos.Add(new Campo("Descricao", tipoSQL.Varchar(), "255", false));
                    
                    if(!MeuSQL.criarTabela(tabela, this.Conexao))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Erros.catalogarErro(ex.Message, "Erros", "manutencaoTabela", this.Conexao);
                return false;
            }
        }

        private bool gravarDados(DateTime data, string descricao)
        {
            try
            {
                TabelaSQL tabela = new TabelaSQL(_tabela);
                tabela.Campos.Add(new CamposSql("Data", data.ToString(), tipoSQL.Data()));
                tabela.Campos.Add(new CamposSql("Descricao", descricao.Replace("'", ""), tipoSQL.Varchar()));
                
                if(!MeuSQL.gravarDados(tabela, this.Conexao))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {

                Erros.catalogarErro(ex.Message, _tabela, "gravarDados", this.Conexao);
                return false;
            }
        }

        public bool excluirDados(int id )
        {
            try
            {
                TabelaSQL tabela = new TabelaSQL(_tabela);
                tabela.Onde.Add(new OndeSql("Id", OperadorSQL.Igual(), id.ToString(), tipoSQL.Inteiro()));
                
                if(!MeuSQL.excluirDados(tabela, this.Conexao))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Erros.catalogarErro(ex.Message, _tabela, "excluirDados", this.Conexao);
                return false;
            }

        }

        public bool listarDados(DateTime dataInicial, DateTime dataFinal)
        {
            try
            {
                TabelaSQL tabela = new TabelaSQL(_tabela);
                tabela.Onde.Add(new OndeSql("Data", OperadorSQL.MaiorIgual(), dataInicial.ToString(), tipoSQL.Data(), EOuSQL.E()));
                tabela.Onde.Add(new OndeSql("Data", OperadorSQL.MenorIgual(), dataFinal.ToString(), tipoSQL.Data()));

                DataSet ds = MeuSQL.buscarDados(tabela, this.Conexao);

                if(ds.Tables[_tabela].Rows.Count == 0)
                {
                    return false;
                }

                listErro.Clear();

                foreach (DataRow item in ds.Tables[_tabela].Rows)
                {
                    Erros err = new Erros(this.Conexao);
                    err.Id = (int)item["Id"];
                    err.Descricao = (string)item["Descricao"];
                    err.Data = (DateTime)item["Data"];
                    listErro.Add(err);
                }

                return true;
            }
            catch (Exception ex)
            {
                Erros.catalogarErro(ex.Message, _tabela, "listarDados", this.Conexao);
                return false;
            }
        }
    }
}
