using Dapper;
using ProjetoAspNetAPI01.Data.Entities;
using ProjetoAspNetAPI01.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoAspNetAPI01.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        //atributo
        private readonly string _connectionstring;

        //construtor para inicializar o atributo connectionstring
        public ClienteRepository(string connectionstring)
        {
            _connectionstring = connectionstring;
        }

        public void Inserir(Cliente cliente)
        {
            var query = @"
                        INSERT INTO CLIENTE(IDCLIENTE, NOME, EMAIL, DATACADASTRO)
                        VALUES(NEWID(), @Nome, @Email, GETDATE())
                    ";

            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Execute(query, cliente);
            }
        }

        public void Alterar(Cliente cliente)
        {
            var query = @"
                        UPDATE CLIENTE 
                        SET
                            NOME = @Nome,
                            EMAIL = @Email
                        WHERE
                            IDCLIENTE = @IdCliente
                    ";

            using(var connection = new SqlConnection(_connectionstring))
            {
                connection.Execute(query, cliente);
            }
        }

        public void Excluir(Cliente cliente)
        {
            var query = @"
                        DELETE FROM CLIENTE
                        WHERE IDCLIENTE = @IdCliente
                    ";

            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Execute(query, cliente);
            }
        }

        public List<Cliente> Consultar()
        {
            var query = @"
                    SELECT * FROM CLIENTE
                    ORDER BY NOME
                ";

            using (var connection = new SqlConnection(_connectionstring))
            {
                return connection
                    .Query<Cliente>(query)
                    .ToList();
            }
        }

        public Cliente ObterPorId(Guid idCliente)
        {
            var query = @"
                        SELECT * FROM CLIENTE
                        WHERE IDCLIENTE = @idCliente
                    ";

            using (var connection = new SqlConnection(_connectionstring))
            {
                return connection
                    .Query<Cliente>(query, new { idCliente })
                    .FirstOrDefault();
            }
        }
    }
}
