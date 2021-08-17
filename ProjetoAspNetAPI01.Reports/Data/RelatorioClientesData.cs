using ProjetoAspNetAPI01.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoAspNetAPI01.Reports.Data
{
    //Modelagem dos dados que serão exibidos no relatorio..
    public class RelatorioClientesData
    {
        public DateTime DataGeracao { get; set; }
        public List<Cliente> Clientes { get; set; }
    }
}


