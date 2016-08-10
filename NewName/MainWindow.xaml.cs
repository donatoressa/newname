using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewName
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region atributos

        public string caminhoGlobal;
        public int ultimoContador = 0;
        private Extensoes [] deParaExtensoes; 

        #endregion

        #region eventos

        private void btnMascarar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MascararArquivos(caminhoGlobal, true, ObterDeParaExtensoes()))
            {
                MessageBox.Show("Arquivos e pastas renomeados com sucesso.", "Operação concluída", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDesmascarar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DesmascararArquivos(caminhoGlobal, true, ObterDeParaExtensoes()))
            {
                MessageBox.Show("Arquivos e pastas renomeados com sucesso.", "Operação concluída", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDiretorio_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string path = System.IO.Path.GetDirectoryName(dlg.FileName);
                // Open document 
                tbCaminho.Text = path;
                caminhoGlobal = path;
            }
        }

        #endregion

        #region métodos

        private bool MascararArquivos(string caminho, bool renomeiaPastas, Extensoes [] extensoes)
        {
           try 
           {
                // Caminho literal da pasta informada 
                DirectoryInfo dirInfo = new DirectoryInfo(@caminho);
                int contadorPastas;
                // Lista com todas as informações das pastas 
                DirectoryInfo[] pastas = dirInfo.GetDirectories();

                if (pastas.Length > 0)
                {
                    contadorPastas = 0;

                    //Se existem pastas, faz o loop percorrendo as pastas
                    while (contadorPastas < pastas.Length)
                    {
                        //Realiza chamada recursiva até o último nível de pasta dentro da pasta atual
                        MascararArquivos(string.Concat(caminho, "\\", pastas[contadorPastas].Name), true, extensoes);
                        contadorPastas++;
                    }

                    Renomear(dirInfo, caminho, true, pastas, true, extensoes);
                }
                else
                {
                    Renomear(dirInfo, caminho, true, pastas, true, extensoes);
                }
                return true;
           }
           catch(Exception ex)
           {
               // Se der erro, exibe o que aconteceu 
               MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
               return false;
           }
       }

        private bool DesmascararArquivos(string caminho, bool renomeiaPastas, Extensoes[] extensoes)
        {
            try
            {
                // Caminho literal da pasta informada 
                DirectoryInfo dirInfo = new DirectoryInfo(@caminho);
                int contadorPastas;
                // Lista com todas as informações das pastas 
                DirectoryInfo[] pastas = dirInfo.GetDirectories();

                if (pastas.Length > 0)
                {
                    contadorPastas = 0;

                    //Se existem pastas, faz o loop percorrendo as pastas
                    while (contadorPastas < pastas.Length)
                    {
                        //Realiza chamada recursiva até o último nível de pasta dentro da pasta atual
                        DesmascararArquivos(string.Concat(caminho, "\\", pastas[contadorPastas].Name), true, extensoes);
                        contadorPastas++;
                    }

                    Renomear(dirInfo, caminho, true, pastas, false, extensoes);
                }
                else
                {
                    Renomear(dirInfo, caminho, true, pastas, false, extensoes);
                }
                return true;
                
            }
            catch (Exception ex)
            {
                // Se der erro, exibe o que aconteceu 
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void Renomear(DirectoryInfo dirInfo, string caminho, bool renomeiaPastas, DirectoryInfo[] pastas, bool estaMascarando, Extensoes[] extensoes)
        {
            try
            {
                // Pega todas as informações dos arquivos dentro do diretório informado 
                FileInfo[] arquivos = dirInfo.GetFiles();

                string antigoNome;
                string novoNome;
                string novoNomeComNovaExtensao;

                int arqAfet = 0, pastasAfet = 0;

                for (int i = 0; i < arquivos.Length; i++)
                {
                    //if(arquivos[i].Name.Contains(" "))
                    //{

                    antigoNome = @caminho + "\\" + arquivos[i].Name;

                    if (estaMascarando)
                    {
                        novoNome = @caminho + "\\" + dirInfo.Name + "--" + arquivos[i].Name.Replace(" ", "_");
                        novoNomeComNovaExtensao = AlterarExtensao(novoNome, estaMascarando, extensoes);
                    }
                    else
                    {
                        novoNome = @caminho + "\\" + arquivos[i].Name.Replace(dirInfo.Name, "").Replace("--","");
                        novoNomeComNovaExtensao = AlterarExtensao(novoNome, estaMascarando, extensoes);
                    }

                    // Move o arquivo para a mesma pasta com os carateres substituídos 
                    File.Move(antigoNome, novoNomeComNovaExtensao);

                    // Contagem dos arquivos renomeados 
                    arqAfet++;
                    //}
                }
                // Se caso foi selecionada a opção de renomear as pastas 
                if (renomeiaPastas)
                {
                    // itera (efetua um loop) dentro do vetor das pastas 
                    for (int i = 0; i < pastas.Length; i++)
                    {
                        antigoNome = @caminho + "\\" + pastas[i].Name;

                        if (estaMascarando)
                        {
                            novoNome = @caminho + "\\" + dirInfo.Name + "--" + pastas[i].Name.Replace(" ", "_");
                        }
                        else
                        {
                            novoNome = @caminho + "\\" + pastas[i].Name.Replace(dirInfo.Name, "").Replace("--", "");
                        }

                        Directory.Move(antigoNome, novoNome);

                        // Contagem das pastas renomeadas 
                        pastasAfet++;
                        //}
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private string AlterarExtensao(string arquivoEntrada, bool estaMascarando, Extensoes [] extensoes)
        {
            string extensaoArquivo = System.IO.Path.GetExtension(arquivoEntrada);
            string arquivoSaida = arquivoEntrada;

            foreach (Extensoes e in extensoes)
            {
                if(estaMascarando)
                {
                    if (extensaoArquivo.Equals(e.extensaoSemMascara))
                    {
                        arquivoSaida = System.IO.Path.ChangeExtension(arquivoEntrada, e.extensaoComMascara);
                        break;
                    }
                }
                else
                {
                    if (extensaoArquivo.Equals(e.extensaoComMascara))
                    {
                        arquivoSaida = System.IO.Path.ChangeExtension(arquivoEntrada, e.extensaoSemMascara);
                        break;
                    }
                }
            }

            return arquivoSaida;
        }

        private Extensoes [] ObterDeParaExtensoes()
        {
            Extensoes [] deParaExtensoes = new Extensoes [7];
            deParaExtensoes[0] = new Extensoes(".js", ".jr");
            deParaExtensoes[1] = new Extensoes(".css", ".crr");
            deParaExtensoes[2] = new Extensoes(".png", ".pnh");
            deParaExtensoes[3] = new Extensoes(".jpg", ".jpj");
            deParaExtensoes[4] = new Extensoes(".gif", ".gig");
            deParaExtensoes[5] = new Extensoes(".html", ".htmk");
            deParaExtensoes[6] = new Extensoes(".crx", ".ctt");

            return deParaExtensoes;
        }

        #endregion
    }

    public partial class Extensoes
    {
        public Extensoes(string extensaoSemMascara, string extensaoComMascara)
        {
            this.extensaoComMascara = extensaoComMascara;
            this.extensaoSemMascara = extensaoSemMascara;
        }

        public string extensaoSemMascara;
        public string extensaoComMascara;
    }
}
