using ProjetoAspNetAPI01.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoAspNetAPI01.Data.Interfaces
{
    public interface IClienteRepository
    {
        void Inserir(Cliente cliente);
        void Alterar(Cliente cliente);
        void Excluir(Cliente cliente);

        List<Cliente> Consultar();
        Cliente ObterPorId(Guid idCliente);
    }
}
