using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Pedidos.Api.Application.Services;
using Pedidos.Api.Application.DTOs;
using Pedidos.Api.Domain.Entities;
using Pedidos.Api.Domain.Interfaces;
using Pedidos.Api.Infrastructure.Search.Services;
using Fcg.Common.Enums;
using Fcg.Common.Middleware.Exceptions;
using Pedidos.Api.Application.Mappers;

namespace Pedidos.Api.Tests.Services
{
    public class PedidoServiceTests
    {
        private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
        private readonly Mock<ISearchService> _searchServiceMock;
        private readonly Mock<ILogger<PedidoService>> _loggerMock;
        private readonly PedidoService _service;

        public PedidoServiceTests()
        {
            _pedidoRepositoryMock = new Mock<IPedidoRepository>();
            _searchServiceMock = new Mock<ISearchService>();
            _loggerMock = new Mock<ILogger<PedidoService>>();
            _service = new PedidoService(
                _pedidoRepositoryMock.Object,
                _searchServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task ObterPedido_DeveRetornarPedido_QuandoExiste()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var pedido = Pedido.CriarAlterar(pedidoId, usuarioId, "Marcio Henrique", Guid.NewGuid(), "Jogo Teste", 0, StatusPedido.Pendente);
            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId)).ReturnsAsync(pedido);

            // Act
            var result = await _service.ObterPedido(pedidoId, usuarioId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pedidoId, result.Id);
        }

        [Fact]
        public async Task ObterPedido_DeveLancarExcecao_QuandoNaoExiste()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId)).ReturnsAsync((Pedido)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ObterPedido(pedidoId, usuarioId));
        }

        [Fact]
        public async Task ObterPedidos_DeveRetornarTodosPedidos()
        {
            // Arrange
            var pedidos = new List<Pedido> { Pedido.CriarAlterar(null, Guid.NewGuid(), "Marcio Henrique", Guid.NewGuid(), "Jogo Teste", 0, StatusPedido.Pendente) };
            _pedidoRepositoryMock.Setup(r => r.ObterTodos()).ReturnsAsync(pedidos);

            // Act
            var result = await _service.ObterPedidos();

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task ObterPedidosAtivos_DeveRetornarPedidosDoUsuario()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var pedidos = new List<Pedido> { Pedido.CriarAlterar(null, usuarioId, "Marcio Henrique", Guid.NewGuid(), "Jogo Teste", 0, StatusPedido.Pendente) };
            _pedidoRepositoryMock.Setup(r => r.ObterTodosPorUsuario(usuarioId)).ReturnsAsync(pedidos);

            // Act
            var result = await _service.ObterPedidosAtivos(usuarioId);

            // Assert
            Assert.Single(result);
            Assert.Equal(usuarioId, result.First().UsuarioId);
        }

        [Fact]
        public async Task AdicionarPedido_DeveAdicionarPedido_QuandoNaoExiste()
        {
            // Arrange
            var dto = new PedidoAdicionarDto { UsuarioId = Guid.NewGuid(), JogoId = Guid.NewGuid() };
            var pedido = dto.ToDomain();
            _pedidoRepositoryMock.Setup(r => r.Existe(It.IsAny<Pedido>())).ReturnsAsync(false);
            _pedidoRepositoryMock.Setup(r => r.Adicionar(It.IsAny<Pedido>())).Returns(Task.CompletedTask);
            //_searchServiceMock.Setup(s => s.AtualizarPopularidadeAsync(dto.JogoId)).Returns(Task.CompletedTask);

            // Act
            await _service.AdicionarPedido(dto);

            // Assert
            _pedidoRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Pedido>()), Times.Once);
            //_searchServiceMock.Verify(s => s.AtualizarPopularidadeAsync(dto.JogoId), Times.Once);
        }

        [Fact]
        public async Task AdicionarPedido_DeveLancarConflito_QuandoPedidoJaExiste()
        {
            // Arrange
            var dto = new PedidoAdicionarDto { UsuarioId = Guid.NewGuid(), JogoId = Guid.NewGuid() };
            _pedidoRepositoryMock.Setup(r => r.Existe(It.IsAny<Pedido>())).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflitoException>(() => _service.AdicionarPedido(dto));
        }

        [Fact]
        public async Task AlterarPedido_DeveAlterarPedido_QuandoNaoExisteConflito()
        {
            // Arrange
            var dto = new PedidoAlterarDto { Id = Guid.NewGuid(), UsuarioId = Guid.NewGuid(), JogoId = Guid.NewGuid() };
            _pedidoRepositoryMock.Setup(r => r.Existe(It.IsAny<Pedido>())).ReturnsAsync(false);
            _pedidoRepositoryMock.Setup(r => r.Alterar(It.IsAny<Pedido>(), false)).Returns(Task.CompletedTask);

            // Act
            await _service.AlterarPedido(dto);

            // Assert
            _pedidoRepositoryMock.Verify(r => r.Alterar(It.IsAny<Pedido>(), false), Times.Once);
        }

        [Fact]
        public async Task AlterarPedido_DeveLancarConflito_QuandoPedidoJaExiste()
        {
            // Arrange
            var dto = new PedidoAlterarDto { Id = Guid.NewGuid(), UsuarioId = Guid.NewGuid(), JogoId = Guid.NewGuid() };
            _pedidoRepositoryMock.Setup(r => r.Existe(It.IsAny<Pedido>())).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflitoException>(() => _service.AlterarPedido(dto));
        }

        [Fact]
        public async Task AtivarPedido_DeveChamarRepositorio()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            _pedidoRepositoryMock.Setup(r => r.Ativar(pedidoId)).Returns(Task.CompletedTask);

            // Act
            await _service.AtivarPedido(pedidoId);

            // Assert
            _pedidoRepositoryMock.Verify(r => r.Ativar(pedidoId), Times.Once);
        }

        [Fact]
        public async Task DesativarPedido_DeveChamarRepositorio()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            _pedidoRepositoryMock.Setup(r => r.Desativar(pedidoId)).Returns(Task.CompletedTask);

            // Act
            await _service.DesativarPedido(pedidoId);

            // Assert
            _pedidoRepositoryMock.Verify(r => r.Desativar(pedidoId), Times.Once);
        }

        [Fact]
        public async Task AtualizarStatusAsync_DeveAlterarStatus_QuandoPedidoExiste()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var pedido = Pedido.CriarAlterar(pedidoId, Guid.NewGuid(), "Marcio Henrique", Guid.NewGuid(), "Jogo Teste", 0, StatusPedido.Pendente);
            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId)).ReturnsAsync(pedido);
            _pedidoRepositoryMock.Setup(r => r.Alterar(pedido, false)).Returns(Task.CompletedTask);

            // Act
            await _service.AtualizarStatusAsync(pedidoId, StatusPedido.Pago);

            // Assert
            _pedidoRepositoryMock.Verify(r => r.Alterar(pedido, false), Times.Once);
        }

        [Fact]
        public async Task AtualizarStatusAsync_NaoFazNada_QuandoPedidoNaoExiste()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            _pedidoRepositoryMock.Setup(r => r.ObterPorId(pedidoId)).ReturnsAsync((Pedido)null);

            // Act
            await _service.AtualizarStatusAsync(pedidoId, StatusPedido.Pago);

            // Assert
            _pedidoRepositoryMock.Verify(r => r.Alterar(It.IsAny<Pedido>(), false), Times.Never);
        }
    }
}
