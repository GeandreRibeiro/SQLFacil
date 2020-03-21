using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace Dados.Classes
{
    public class MeuSQL
    {
        public static bool existeTabela(string tabela, Conexao conexao)
        {
            try
            {
                if(conexao == null)
                {
                    Erros.catalogarErro("Conexão não pode ser nula", tabela, "existeTabela", conexao);
                    return false;
                }

                //criando uma conexao
                Conexao objConexao = conexao;
                
                //definindo o sql
                string strSQL = "select object_id('" + tabela + "')";
                
                //executando o sql
                
                SqlDataAdapter da = new SqlDataAdapter(strSQL, objConexao.Cnn);

                DataSet ds = new DataSet();
                da.Fill(ds, tabela);

                foreach (DataRow item in ds.Tables[tabela].Rows )
                {
                    if (item[0].ToString() == string.Empty)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                return false; 
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool existeCampo(string tabela, string Campo, Conexao conexao)
        {
            try
            {
                if (conexao == null)
                {
                    Erros.catalogarErro("Conexão não pode ser nula", tabela, "existeCampo", conexao);
                    return false;
                }

                Conexao objconexao = conexao;

                //definindo o sql
                string strSQL = "select b.name as 'Coluna1' from sys.tables a join sys.columns b on a.object_id = b.object_id where a.name = '" + tabela + "' and b.name = '" + Campo + "'";

                //executando o sql
                SqlDataAdapter da = new SqlDataAdapter(strSQL, objconexao.Cnn);
                DataSet ds = new DataSet();
                da.Fill(ds, tabela);

                int i = 0;
                foreach (DataRow item in ds.Tables[tabela].Rows)
                {
                    i += 1;
                }

                if (i > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Erros.catalogarErro(ex.Message, "MeuSql", "existeCampo", conexao);
                return false;
            }
        }

        public static bool criarTabela(Tabela tabela, Conexao conexao)
        {
            try
            {
                if (conexao == null)
                {
                    Erros.catalogarErro("Conexão não pode ser nula", tabela.Nome, "criarTabela", conexao);
                    return false;
                }

                //definir o sql
                string strSQL = "CREATE TABLE " + tabela.Nome + " (";
                int cont = 0;
                foreach (Campo item in tabela.Campos)
                {
                    cont += 1;

                    strSQL += item.Nome + " " + item.Tipo;

                    if(item.Tamanho != string.Empty )
                    {
                        strSQL += " (" + item.Tamanho + ") ";
                    }

                    if (item.AutoNumercao)
                    {
                        strSQL += " IDENTITY(1,1) ";
                    }

                    if (item.AceitaNullo )
                    {
                        strSQL += " NULL ";
                    }
                    else
                    {
                        strSQL += " NOT NULL ";
                    }

                    if (cont < tabela.Campos.Count)
                    {
                        strSQL += ", ";
                    }

                    
                }

                string campo = "";
                List<string> campos = new List<string>();

                if (tabela.existeChaveComposta(ref campos))
                {
                    strSQL += "CONSTRAINT [PK_" + tabela.Nome + "] PRIMARY KEY CLUSTERED(";
                    int i = 0;
                    foreach (string item in campos)
                    {
                        i += 1;

                        if (i == campos.Count)
                        {
                            strSQL += item + " ASC ";
                        }
                        else
                        {
                            strSQL += item + " ASC, ";
                        }
                        
                    }

                    strSQL += ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]";
                }
                else
                {
                    if (tabela.existeChavePrimaria(ref campo))
                    {
                        strSQL += "CONSTRAINT [PK_" + tabela.Nome + "] PRIMARY KEY CLUSTERED(";
                        strSQL += campo + " ASC";
                        strSQL += ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]";
                    }
                }

                //executando o sql
                Conexao objConexao = conexao;
                
                SqlCommand comando = new SqlCommand(strSQL, objConexao.Cnn);
                comando.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                Erros.catalogarErro(ex.Message, "MeuSQL", "criarTabela", conexao);
                return false;
            }

        }

        public static bool criarCampo(Tabela tabela, Conexao conexao)
        {
            try
            {

                if (conexao == null)
                {
                    Erros.catalogarErro("Conexão não pode ser nula", tabela.Nome, "criarCampo", conexao);
                    return false;
                }

                //definir o sql
                string strSQL = "ALTER TABLE " + tabela.Nome + " ADD ";
                int cont = 0;
                foreach (Campo item in tabela.Campos)
                {
                    cont += 1;

                    strSQL += item.Nome + " " + item.Tipo;

                    if (item.Tamanho != string.Empty)
                    {
                        strSQL += " (" + item.Tamanho + ") ";
                    }

                    if (item.AceitaNullo)
                    {
                        strSQL += " NULL ";
                    }
                    else
                    {
                        strSQL += " NOT NULL ";
                    }
                    
                    if(item.ValorPadrao != null)
                    {
                        if (item.Tipo == tipoSQL.Varchar())
                        {
                            strSQL += " DEFAULT '" + item.ValorPadrao + "'";
                        }
                        else
                        {
                            strSQL += " DEFAULT " + item.ValorPadrao;
                        }
                    }

                    if (cont < tabela.Campos.Count)
                    {
                        strSQL += ", ";
                    }
                }

                //executando o sql
                Conexao objConexao = conexao;

                SqlCommand comando = new SqlCommand(strSQL, objConexao.Cnn);
                comando.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                Erros.catalogarErro(ex.Message, "MeuSQL", "criarTabela", conexao);
                return false;
            }

        }

        public static bool gravarDados(TabelaSQL tabela, out int Id, Conexao conexao)
        {
            try
            {
                if (conexao == null)
                {
                    Erros.catalogarErro("Conexão não pode ser nula", tabela.Nome, "gravarDados", conexao);
                    Id = 0;
                    return false;
                }
                //Monta o sql 
                string strSQL = criaSQLInsert(tabela, true);

                //iniciar uma conexao 
                Conexao objConexao = conexao;

                SqlCommand comando = new SqlCommand(strSQL, objConexao.Cnn);

                //retorno o id
                Id = (int)comando.ExecuteScalar();

                return true;
            }
            catch (Exception ex)
            {
                Erros.catalogarErro(ex.Message + " Tabela: " + tabela.Nome, "MeuSQL", "gravarDados", conexao);
                Id = 0;
                return false;
            }
        }

        public static bool gravarDados(TabelaSQL tabela, Conexao conexao)
        {
            try
            {
                if (conexao == null)
                {
                    Erros.catalogarErro("Conexão não pode ser nula", tabela.Nome, "gravarDados", conexao);
                    return false;
                }
                //Monta o sql 
                string strSQL = criaSQLInsert(tabela, false);
                
                Conexao objConexao = conexao;

                SqlCommand comando = new SqlCommand(strSQL, objConexao.Cnn);
                comando.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Erros.catalogarErro(ex.Message + " Tabela: " + tabela.Nome, "MeuSQL", "gravarDados", conexao);
                return false;
            }
        }

        public static bool alteraDados(TabelaSQL tabela, Conexao conexao)
        {
            try
            {
                if (conexao == null)
                {
                    Erros.catalogarErro("Conexão não pode ser nula", tabela.Nome, "alteraDados", conexao);
                    return false;
                }
                //definindo o sql
                string strSQL = "UPDATE " + tabela.Nome + " SET ";
                int i = 0;
                //campos e valores
                foreach (CamposSql item in tabela.Campos)
                {
                    //se o tipo vor varchar
                    if (item.Tipo == tipoSQL.Varchar())
                    {
                        strSQL += item.Nome + " = '" + item.Valor + "'";
                    }
                    //se o tipo for inteiro
                    if (item.Tipo == tipoSQL.Inteiro())
                    {
                        strSQL += item.Nome + " = " + item.Valor;
                    }
                    //se o tipo for decimal
                    if (item.Tipo == tipoSQL.Dinheiro())
                    {
                        strSQL += item.Nome + " = " + item.Valor.Replace(",", ".");
                    }
                    //se o tipo for data
                    if (item.Tipo == tipoSQL.Data())
                    {
                        strSQL += item.Nome + " = " + "'" + item.Valor + "'";
                    }
                    //se o tipo for data hora
                    if (item.Tipo == tipoSQL.DataHora())
                    {
                        strSQL += item.Nome + " = " + "'" + item.Valor + "'";
                    }
                    //se o tipo for verdadeiro falso
                    if(item.Tipo == tipoSQL.VerdadeiroFalso())
                    {
                        strSQL += item.Nome + " = " + item.Valor;
                    }

                    i++;
                    if (i != tabela.Campos.Count)
                    {
                        strSQL += ", ";
                    }
                }
                
                //definindo as condições
                strSQL += montarOnde(tabela.Onde);

                //Iniciando uma conexao
                Conexao objConexao = conexao;

                SqlCommand comando = new SqlCommand(strSQL, objConexao.Cnn);
                comando.ExecuteNonQuery();
                //retornando a função
                return true;
            }
            catch (Exception ex)
            {

                Erros.catalogarErro(ex.Message + " Tabela: " + tabela.Nome, "MeuSQL", "alteraDados", conexao);
                return false;
            }

        }

        public static bool excluirDados(TabelaSQL tabela, Conexao conexao)
        {
            try
            {
                if (conexao == null)
                {
                    Erros.catalogarErro("Conexão não pode ser nula", tabela.Nome, "excluirDados", conexao);
                    return false;
                }
                //Definindo o sql
                string strSQL = "DELETE FROM " + tabela.Nome;

                //Condições
                strSQL += montarOnde(tabela.Onde);

                //Iniciando uma conexo
                Conexao objconexao = conexao;

                //executando o sql
                SqlCommand comando = new SqlCommand(strSQL, objconexao.Cnn);
                comando.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                Erros.catalogarErro(ex.Message + " Tabela: " + tabela.Nome, "MeuSQL", "excluirDados", conexao);
                return false;
            }
        }

        public static DataSet buscarDados(TabelaSQL tabela, Conexao conexao)
        {
            try
            {
                if (conexao == null)
                {
                    Erros.catalogarErro("Conexão não pode ser nula", tabela.Nome, "buscarDados", conexao);
                    return new DataSet();
                }
                string strSQL = "SELECT ";
                //definindo o sql 
                if (tabela.Campos.Count == 0)
                {
                    strSQL += "*";
                }
                else
                {
                    int icampos = 0;
                    foreach (CamposSql item in tabela.Campos)
                    {
                        strSQL += item.Nome;
                        icampos++;
                        if (icampos!= tabela.Campos.Count)
                        {
                            strSQL += ", ";
                        }
                    }
                }

                strSQL += " FROM " + tabela.Nome;

                //montar junção
                strSQL += montarJuncao(tabela.Jucao);

                //montando onde
                strSQL += montarOnde(tabela.Onde);
                
                //Ordenando o sql
                if (tabela.Ordem.Count > 0)
                {
                    strSQL += " ORDER BY ";
                }
                
                //preenchendo os campos de ordem
                int i = 0;
                foreach (OrdemSQL item in tabela.Ordem)
                {
                    strSQL += item.Campo;

                    i++;
                    if (i!= tabela.Ordem.Count)
                    {
                        strSQL += ", ";
                    }
                }

                SqlDataAdapter da;
                Conexao objconexao = conexao;
                //Inicando um conexao
                da = new SqlDataAdapter(strSQL, objconexao.Cnn);
                
                
                //executando o sql
                DataSet ds = new DataSet();
                da.Fill(ds, tabela.Nome);
                
                return ds;
            }
            catch (Exception ex)
            {

                Erros.catalogarErro(ex.Message + " Tabela: " + tabela.Nome, "MeuSQL", "buscarDados", conexao);
                return new DataSet();
            }

        }

        public static bool excluirTabela(Tabela tabela, Conexao conexao)
        {
            try
            {
                if (conexao == null)
                {
                    Erros.catalogarErro("Conexão não pode ser nula", tabela.Nome, "excluirTabela", conexao);
                    return false;
                }
                //montando o sql
                string strSQL = "DROP TABLE " + tabela.Nome;
                //iniciando uma conexao
                Conexao objconexao = conexao;

                //executando o sql
                SqlCommand comando = new SqlCommand(strSQL, objconexao.Cnn);
                comando.ExecuteNonQuery();
                //retornando a função
                return true;
            }
            catch (Exception ex)
            {

                Erros.catalogarErro(ex.Message + " Tabela: " + tabela.Nome, "MeuSQL", "excluirTabela", conexao);
                return false;
            }


        }

        private static string montarOnde(List<OndeSql> Onde)
        {
            string strSQL = "";
            int i = 0;
            if (Onde.Count > 0)
            {
                strSQL += " WHERE ";
            }
            foreach (OndeSql item in Onde)
            {
                //se for do tipo varchar
                if (item.Tipo == tipoSQL.Varchar())
                {
                    strSQL += item.Campo + " " + item.Operador + " '" + item.Valor + "'";
                }
                //se o tipo for inteiro
                if (item.Tipo == tipoSQL.Inteiro())
                {
                    strSQL += item.Campo + " " + item.Operador + " " + item.Valor;
                }
                //se o tipo for decimal
                if (item.Tipo == tipoSQL.Dinheiro())
                {
                    strSQL += item.Campo + " " + item.Operador + " " + item.Valor.Replace(",", ".");
                }
                //se o tipo for data
                if (item.Tipo == tipoSQL.Data())
                {
                    strSQL += "CONVERT(Date, " + item.Campo + ", 103) " + item.Operador + " '" + item.Valor + "'";
                }
                //se o tipo for data hora
                if (item.Tipo == tipoSQL.DataHora())
                {
                    strSQL += item.Campo + " " + item.Operador + " '" + item.Valor + "'";
                }

                i++;
                if (i != Onde.Count)
                {
                    strSQL += " " + item.EOu + " ";
                }
            }

            return strSQL;
        }

        private static string montarJuncao(List<juncao> juncao)
        {
            string strSQL = "";
            if (juncao.Count == 0)
            {
                return strSQL;
            }

            foreach (juncao item in juncao)
            {
                //tabela secundaria
                strSQL += " INNER JOIN " + item.TabelaSecundaria + " ON ";
                
                //campos da junção
                int i = 0;
                foreach (CamposJuncao campo in item.Campos)
                {
                    if(i == 0)
                    {
                        strSQL += item.TabelaPrincipal + "." + campo.CampoPrincipal;
                        strSQL += " = " + item.TabelaSecundaria + "." + campo.CamposSecundario;
                    }
                    else//se mais de um campo de junção
                    {
                        strSQL += " AND " + item.TabelaPrincipal + "." + campo.CampoPrincipal;
                        strSQL += " = " + item.TabelaSecundaria + "." + campo.CamposSecundario;
                    }

                    i++;
                }
            }

            return strSQL;
        }
        private static string criaSQLInsert(TabelaSQL tabela, bool retornaId)
        {
            //definindo o sql 
            string strSQL = "INSERT INTO " + tabela.Nome + "(";
            //percorrendo os campos e atribuindo no sql
            int i = 0;
            foreach (CamposSql item in tabela.Campos)
            {
                //pegando o nome do campo 
                strSQL += item.Nome;

                i++;
                if (i != tabela.Campos.Count)//colocando a virgula para o proximo campo
                {
                    strSQL += ", ";
                }

            }

            strSQL += " ) VALUES ( ";

            //percorrendo os campos e atribuindo os valors no sql
            i = 0;
            foreach (CamposSql item in tabela.Campos)
            {
                //se o tipo vor varchar
                if (item.Tipo == tipoSQL.Varchar())
                {
                    strSQL += "'" + item.Valor + "'";
                }
                //se o tipo for inteiro
                if (item.Tipo == tipoSQL.Inteiro())
                {
                    strSQL += item.Valor;
                }
                //se o tipo for decimal
                if (item.Tipo == tipoSQL.Dinheiro())
                {
                    strSQL += item.Valor.Replace(",", ".");
                }
                //se o tipo for data
                if (item.Tipo == tipoSQL.Data())
                {
                    strSQL += "'" + item.Valor + "'";
                }
                //se o tipo for data hora
                if (item.Tipo == tipoSQL.DataHora())
                {
                    strSQL += "'" + item.Valor + "'";
                }
                //se o tipo for verdadeiro falso
                if (item.Tipo == tipoSQL.VerdadeiroFalso())
                {
                    strSQL += item.Valor;
                }
                i++;
                if (i != tabela.Campos.Count)
                {
                    strSQL += ", ";
                }

            }

            //finalizando 
            strSQL += ")";

            //se retorna id
            if (retornaId)
            {
                strSQL += " SELECT CAST(scope_identity() AS int) ";
            }

            return strSQL;
        }
    }
}
