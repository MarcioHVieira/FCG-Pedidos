using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;
using Pedidos.Api.Application.Services;
using Fcg.Common.Controllers;
using Pedidos.Api.Application.DTOs;
using Fcg.Common.Enums;

namespace Pedidos.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PedidosController : MainController
    {
        private readonly IPedidoService _pedido;

        public PedidosController(IPedidoService pedido)
        {
            _pedido = pedido;
        }

        //[Authorize(Roles = "Usuario,Administrador")]
        [HttpGet("ObterPedido")]
        [SwaggerOperation(Summary = "Obtém um pedido pelo ID", 
                          Description = "Retorna um pedido específico com base no ID informado.")]
        public async Task<IActionResult> ObterPedido(Guid pedidoId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            var usuarioId = (User.IsInRole("Usuario") && claim != null && Guid.TryParse(claim.Value, out var guidUsuario))
                ? guidUsuario
                : Guid.Empty;
            var pedido = await _pedido.ObterPedido(pedidoId, usuarioId);
            return CustomResponse(pedido);
        }

        //[Authorize(Roles = "Administrador")]
        [HttpGet("ObterPedidos")]
        [SwaggerOperation(Summary = "Obtém todos os pedidos", 
                          Description = "Retorna uma lista de todos os pedidos cadastrados (ativos e não ativos).")]
        public async Task<IActionResult> ObterPedidos()
        {
            var pedidos = await _pedido.ObterPedidos();
            return pedidos.Any()
                ? CustomResponse(pedidos)
                : CustomResponse("Nenhum pedido encontrado", StatusCodes.Status404NotFound);
        }

        //[Authorize(Roles = "Usuario,Administrador")]
        [HttpGet("ObterPedidosAtivos")]
        [SwaggerOperation(Summary = "Obtém pedidos ativos do usuário", 
                          Description = "Retorna uma lista de pedidos ativos para o usuário autenticado.")]
        public async Task<IActionResult> ObterPedidosAtivos()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            var usuarioId = (User.IsInRole("Usuario") && claim != null && Guid.TryParse(claim.Value, out var guidUsuario))
                ? guidUsuario
                : Guid.Empty;
            var pedidos = await _pedido.ObterPedidosAtivos(usuarioId);
            return pedidos.Any()
                ? CustomResponse(pedidos)
                : CustomResponse("Nenhum pedido encontrado", StatusCodes.Status404NotFound);
        }

        //[Authorize(Roles = "Usuario,Administrador")]
        [HttpPost("AdicionarPedido")]
        [SwaggerOperation(Summary = "Adiciona um novo pedido", 
                          Description = "Permite que usuários adicionem um novo pedido.")]
        public async Task<IActionResult> AdicionarPedido(PedidoAdicionarDto pedido)
        {
            //if (!ValidarModelo() || !ValidarPermissao(pedido.UsuarioId))
            //    return CustomResponse();

            await _pedido.AdicionarPedido(pedido);
            return CustomResponse("Pedido adicionado com sucesso");
        }

        //[Authorize(Roles = "Usuario,Administrador")]
        [HttpPut("CancelarPedido")]
        [SwaggerOperation(Summary = "Cancela um pedido existente", 
                          Description = "Realiza o cancelamento de um pedido.")]
        public async Task<IActionResult> CancelarPedido(Guid pedidoId)
        {
            await _pedido.AtualizarStatusAsync(pedidoId, StatusPedido.Cancelado);
            return CustomResponse("Pedido alterado com sucesso");
        }

        //[Authorize(Roles = "Usuario,Administrador")]
        [HttpPut("AlterarPedido")]
        [SwaggerOperation(Summary = "Altera um pedido existente", 
                          Description = "Permite que usuários alterem um pedido já criado.")]
        public async Task<IActionResult> AlterarPedido(PedidoAlterarDto pedido)
        {
            if (!ValidarModelo() || !ValidarPermissao(pedido.UsuarioId))
                return CustomResponse();

            await _pedido.AlterarPedido(pedido);
            return CustomResponse("Pedido alterado com sucesso");
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("AtivarPedido")]
        [SwaggerOperation(Summary = "Ativa um pedido", 
                          Description = "Permite que administradores ativem um pedido.")]
        public async Task<IActionResult> AtivarPedido(Guid pedidoId)
        {
            await _pedido.AtivarPedido(pedidoId);
            return CustomResponse("Pedido ativado com sucesso");
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("DesativarPedido")]
        [SwaggerOperation(Summary = "Desativa um pedido", 
                          Description = "Permite que administradores desativem um pedido.")]
        public async Task<IActionResult> DesativarPedido(Guid pedidoId)
        {
            await _pedido.DesativarPedido(pedidoId);
            return CustomResponse("Pedido desativado com sucesso");
        }
    }
}
    