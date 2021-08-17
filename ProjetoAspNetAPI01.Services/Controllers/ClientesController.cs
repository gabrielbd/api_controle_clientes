using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetoAspNetAPI01.Data.Entities;
using ProjetoAspNetAPI01.Data.Interfaces;
using ProjetoAspNetAPI01.Reports.Data;
using ProjetoAspNetAPI01.Reports.Pdfs;
using ProjetoAspNetAPI01.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAspNetAPI01.Services.Controllers
{
    [Route("api/[controller]")] //endereço do serviço /api/clientes
    [ApiController] //definindo que o controlador é do tipo API
    public class ClientesController : ControllerBase
    {
        //atributo para que possamos utilizar a camada de repositorio..
        private readonly IClienteRepository _clienteRepository;

        //método construtor para que o atributo possa ser inicializado
        public ClientesController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        //criando um método que permita realizar o cadastro de um cliente
        //serviço da API para cadastro de cliente
        [HttpPost]
        public IActionResult Post(ClienteCadastroModel model)
        {
            try
            {
                //criando um objeto do tipo cliente
                var cliente = new Cliente();

                //capturar os dados que a API receber atraves da model
                cliente.Nome = model.Nome;
                cliente.Email = model.Email;

                //cadastrando o cliente
                _clienteRepository.Inserir(cliente);

                //retornando mensagem de sucesso!
                return Ok($"Cliente {cliente.Nome}, cadastrado com sucesso.");
            }
            catch (Exception e)
            {
                //retornando mensagem de erro!
                //HTTP 500 (Código de erro) -> INTERNAL SERVER ERROR
                return StatusCode(500, $"Ocorreu um erro: {e.Message}");
            }
        }

        //criando um método que permita realizar a edição de um cliente
        //serviço da API para atualização de cliente
        [HttpPut]
        public IActionResult Put(ClienteEdicaoModel model)
        {
            try
            {
                //verificar se o id do cliente informado existe no banco de dados
                if (_clienteRepository.ObterPorId(model.IdCliente) != null)
                {
                    //atualizando os dados do cliente
                    var cliente = new Cliente();

                    cliente.IdCliente = model.IdCliente;
                    cliente.Nome = model.Nome;
                    cliente.Email = model.Email;

                    _clienteRepository.Alterar(cliente);

                    return Ok($"Cliente {cliente.Nome}, atualizado com sucesso.");
                }
                else //cliente não foi encontrado..
                {
                    //retornar o erro HTTP 422 (Unprocessable Entity)
                    return UnprocessableEntity(
                        "O cliente informado não está cadastrado no sistema, por favor, verifique o ID enviado.");
                }
            }
            catch (Exception e)
            {
                //retornando mensagem de erro!
                //HTTP 500 (Código de erro) -> INTERNAL SERVER ERROR
                return StatusCode(500, $"Ocorreu um erro: {e.Message}");
            }
        }

        //criando um método que permita realizar a consulta de todos os clientes cadastrados
        //serviço da API para consulta de clientes
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                //consultar todos os clientes cadastrados na base de dados
                var clientes = _clienteRepository.Consultar();

                //retornar a lista de clientes..
                return Ok(clientes);
            }
            catch (Exception e)
            {
                //retornando mensagem de erro!
                //HTTP 500 (Código de erro) -> INTERNAL SERVER ERROR
                return StatusCode(500, $"Ocorreu um erro: {e.Message}");
            }
        }

        //criando um método que permita realizar a exclusão de um cliente cadastrado
        //serviço da API para exclusão de cliente
        [HttpDelete("{idCliente}")] //o campo idCliente será enviado na URL do serviço
        public IActionResult Delete(Guid idCliente)
        {
            try
            {
                //verificando se o id enviado está cadastrado na base de dados..
                //buscando o cliente no banco de dados atraves do id..
                var cliente = _clienteRepository.ObterPorId(idCliente);

                //verificar se o cliente foi encontrado
                if (cliente != null)
                {
                    //excluindo o cliente obtido
                    _clienteRepository.Excluir(cliente);

                    return Ok($"Cliente {cliente.Nome}, excluído com sucesso.");
                }
                else
                {
                    //retornar o erro HTTP 422 (Unprocessable Entity)
                    return UnprocessableEntity(
                        "O cliente informado não está cadastrado no sistema, por favor, verifique o ID enviado.");
                }
            }
            catch (Exception e)
            {
                //retornando mensagem de erro!
                //HTTP 500 (Código de erro) -> INTERNAL SERVER ERROR
                return StatusCode(500, $"Ocorreu um erro: {e.Message}");
            }
        }

        //criando um método que permita realizar a consulta de 1 cliente atraves do ID
        //serviço da API para consulta de cliente por ID
        [HttpGet("{idCliente}")] //receber um parametro ID na URL do serviço
        public IActionResult GetById(Guid idCliente)
        {
            try
            {
                //consultar no banco de dados 1 cliente atraves do ID..
                var cliente = _clienteRepository.ObterPorId(idCliente);

                //retornar os dados do cliente
                return Ok(cliente);
            }
            catch (Exception e)
            {
                //retornando mensagem de erro!
                //HTTP 500 (Código de erro) -> INTERNAL SERVER ERROR
                return StatusCode(500, $"Ocorreu um erro: {e.Message}");
            }
        }

        //criando um método que retorne o total de clientes cadastrados por data
        [HttpGet]
        [Route("SomatorioDatas")]
        public IActionResult GetSomatorioDatas()
        {
            try
            {
                //consultar todos os clientes na base de dados
                var clientes = _clienteRepository.Consultar();

                //capturar os nomes e datas de clientes cadastrados
                var dados = clientes.Select(c =>
                    new
                    {
                        NomeCliente = c.Nome,
                        DataCadastro = c.DataCadastro.ToString("dd/MM/yyyy")
                    }).ToList();

                //totalizar a quantidade de clientes por data
                var result = dados.GroupBy(c => c.DataCadastro)
                    .Select(g => new { DataCadastro = g.Key, Quantidade = g.Count() })
                    .ToList();

                return Ok(result);
            }
            catch (Exception e)
            {
                //retornando mensagem de erro!
                //HTTP 500 (Código de erro) -> INTERNAL SERVER ERROR
                return StatusCode(500, $"Ocorreu um erro: {e.Message}");
            }
        }

        //método para retornar os dados do relatorio de clientes
        [HttpGet]
        [Route("Relatorio/{dataMin}/{dataMax}")]
        public IActionResult GetRelatorio(DateTime dataMin, DateTime dataMax)
        {
            try
            {
                //criando o objeto que irá levar os dados para o relatorio
                var data = new RelatorioClientesData();
                data.DataGeracao = DateTime.Now; //data do sistema
                data.Clientes = _clienteRepository.Consultar(dataMin, dataMax);
                //consulta de clientes do banco de dados

                //gerar o arquivo PDF do relatorio..
                var report = new ClientesReportPDF();
                var pdf = report.GerarRelatorio(data);

                //retornar o arquivo para download..
                return File(pdf, "application/pdf");
            }
            catch (Exception e)
            {
                //retornando mensagem de erro!
                //HTTP 500 (Código de erro) -> INTERNAL SERVER ERROR
                return StatusCode(500, $"Ocorreu um erro: {e.Message}");
            }
        }
    }
}


