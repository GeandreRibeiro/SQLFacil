using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using Dados.Formes;

namespace Dados.Classes
{
    public class Conexao
    {
        public SqlConnection Cnn { get; set; }
        public string ConnectionString { get; set; }
        public bool ConnectionOk { get; set; }

        private string NomeServidor;
        private string BaseDeDados;
        private string Usuario;
        private string Senha;
        private bool usarSenha;
        public Conexao()
        {
            gerarStringDeConexao();
        }

        public bool gerarStringDeConexao()
        {
            //caminho do arquivo de configuração da conexao
            string caminho = AppContext.BaseDirectory + @"Config.txt";
            //iniciando a cnn
            this.Cnn = new SqlConnection();
            //existe este arquivo
            if (File.Exists(caminho))
            {
                File.Decrypt(caminho);
                int i = 0;

                foreach (string item in File.ReadLines(caminho))
                {
                    i += 1;

                    if (i == 1)
                        NomeServidor = UtilArquivo.Descriptografa(item);

                    if (i == 2)
                        BaseDeDados = UtilArquivo.Descriptografa(item);

                    if (i == 3)
                        Usuario = UtilArquivo.Descriptografa(item);

                    if (i == 4)
                        Senha = UtilArquivo.Descriptografa(item);

                    if (i == 5)
                        usarSenha = bool.Parse(UtilArquivo.Descriptografa(item));
                }

                this.ConnectionString = "Server = " + NomeServidor + " ; Database = " + BaseDeDados + "; ";
                if (usarSenha)
                {
                    this.ConnectionString += " User Id = " + Usuario + "; Password = " + Senha + ";";
                }
                else
                {
                    this.ConnectionString += "integrated security = true;";
                }

                try
                {
                    this.Cnn.ConnectionString = this.ConnectionString;
                    this.Cnn.Open();
                    this.ConnectionOk = true;
                }
                catch (Exception)
                {
                    MessageBox.Show("Ocorreu algum problema com a conexão verifique a configuração.", "Conexao");
                }
            }
            else//esse arquivo não existe
            {
                frmConexao frm = new frmConexao();
                frm.ShowDialog();

                if (!frm.Cancelou)
                {
                    this.ConnectionString = "Server = " + frm.Servidor + " ; Database = " + frm.BaseDados + "; ";
                    if (frm.UsaSenha)
                    {
                        this.ConnectionString += "User Id = " + Usuario + "; Password = " + Senha + ";";
                    }
                    else
                    {
                        this.ConnectionString += "integrated security = true;";
                    }
                    try
                    {
                        this.Cnn.ConnectionString = this.ConnectionString;
                        Cnn.Open();

                        //se não der erro na cnn vou gravar o arquivo de configuração
                        List<string> campos = new List<string>();
                        campos.Add(UtilArquivo.Criptografa(frm.Servidor));
                        campos.Add(UtilArquivo.Criptografa(frm.BaseDados));
                        campos.Add(UtilArquivo.Criptografa(frm.Usuario));
                        campos.Add(UtilArquivo.Criptografa(frm.Senha));
                        campos.Add(UtilArquivo.Criptografa(frm.UsaSenha.ToString()));
                        File.WriteAllLines(caminho, campos);

                        this.ConnectionOk = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Conexão");
                    }
                }
            }

            //File.OpenRead(AppContext.BaseDirectory + "Config.txt");

            return true;
        }

    }
}


