using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using ProjetoAspNetAPI01.Reports.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoAspNetAPI01.Reports.Pdfs
{
    public class ClientesReportPDF
    {
        //método para desenhar o relatorio e retorna-lo em formato de arquivo
        public byte[] GerarRelatorio(RelatorioClientesData data)
        {
            var memoryStream = new MemoryStream();
            var pdf = new PdfDocument(new PdfWriter(memoryStream));

            using (var document = new Document(pdf))
            {
                //logotipo
                var logo = ImageDataFactory.Create("https://www.cotiinformatica.com.br/imagens/logo-coti-informatica.png");
                document.Add(new Image(logo));

                document.Add(new Paragraph("\n")); //quebra de linha

                //titulo do relatorio
                document.Add(new Paragraph("Relatório de Clientes").SetFontSize(24));

                //data de geração do relatorio
                document.Add(new Paragraph("Relatório gerado em: " + data.DataGeracao.ToString("dddd, dd/MM/yyyy HH:mm")));

                document.Add(new Paragraph("\n")); //quebra de linha

                //desenhar uma tabela para imprimir os clientes do relatorio..
                var table = new Table(3); //3 -> numero de colunas da tabela

                //cabeçalho das colunas
                table.AddHeaderCell("Nome do Cliente");
                table.AddHeaderCell("Email");
                table.AddHeaderCell("Data/Hora de Cadastro");

                //percorrer a lista de clientes
                foreach (var item in data.Clientes)
                {
                    table.AddCell(item.Nome);
                    table.AddCell(item.Email);
                    table.AddCell(item.DataCadastro.ToString("dd/MM/yyyy HH:mm"));
                }

                //adicionar a tabela no documento PDF
                document.Add(table);

                //imprimir a quantidade de clientes obtidos
                document.Add(new Paragraph("\n")); //quebra de linha
                document.Add(new Paragraph($"Quantidade de clientes exibidos: {data.Clientes.Count}"));
            }

            //retornar o conteudo do relatorio em formato de arquivo de memória
            return memoryStream.ToArray();
        }
    }
}


