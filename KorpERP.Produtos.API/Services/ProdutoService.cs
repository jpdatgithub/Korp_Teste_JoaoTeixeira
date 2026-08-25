
using KorpERP.Produtos.API.Models;
using KorpERP.Produtos.API.Persistence;
using KorpERP.Produtos.API.Interfaces;
using KorpERP.Shared.Events;
using Microsoft.EntityFrameworkCore;
using KorpERP.Shared.Contracts.Produto;
using KorpERP.Shared.Contracts.NotaFiscal;
using Npgsql;

namespace KorpERP.Produtos.API.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly ProdutosDbContext _context;
        private readonly IEventPublisher _eventPublisher;

        public ProdutoService(ProdutosDbContext context, IEventPublisher eventPublisher)
        {
            _context = context;
            _eventPublisher = eventPublisher;
        }

        public async Task<Produto> CreateProdutoAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();

            await _eventPublisher.PublishAsync(new ProdutoCriadoEvent
            {
                ProdutoId = produto.Id,
                Codigo = produto.Codigo,
                Descricao = produto.Descricao,
                Versao = produto.Versao,
                DataCriacao = DateTime.UtcNow
            });

            if (produto.Saldo > 0)
            {
                await _eventPublisher.PublishAsync(new EstoqueAtualizadoEvent
                {
                    ProdutoId = produto.Id,
                    NovoSaldo = produto.Saldo,
                    Versao = produto.Versao,
                    DataAtualizacao = DateTime.UtcNow
                });
            }

            return produto;
        }
        public async Task<Produto> AtualizarProdutoAsync(int produtoId, int novoSaldo, string novoCodigo, string novaDescricao)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);

            if (produto == null)
            {
                throw new KeyNotFoundException($"Produto com ID {produtoId} não encontrado.");
            }

            bool saldoAlterado = produto.Saldo != novoSaldo;
            bool produtoAlterado = produto.Codigo != novoCodigo || produto.Descricao != novaDescricao;

            produto.Saldo = novoSaldo;
            produto.Codigo = novoCodigo;
            produto.Descricao = novaDescricao;

            if (saldoAlterado || produtoAlterado)
            {
                produto.Versao++;
            }

            await _context.SaveChangesAsync();

            var dataAtualizacao = DateTime.UtcNow;

            if (saldoAlterado)
            {
                await _eventPublisher.PublishAsync(new EstoqueAtualizadoEvent
                {
                    ProdutoId = produto.Id,
                    NovoSaldo = novoSaldo,
                    Versao = produto.Versao,
                    DataAtualizacao = dataAtualizacao
                });
            }

            if (produtoAlterado)
            {
                await _eventPublisher.PublishAsync(new ProdutoAtualizadoEvent
                {
                    ProdutoId = produto.Id,
                    Codigo = novoCodigo,
                    Descricao = novaDescricao,
                    Versao = produto.Versao,
                    DataAtualizacao = dataAtualizacao
                });
            }

            return produto;
        }
        public async Task<Produto> GetProdutoByIdAsync(int produtoId)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);

            if (produto == null)
            {
                throw new KeyNotFoundException($"Produto com ID {produtoId} não encontrado.");
            }

            return produto;
        }
        public async Task<List<Produto>> GetAllProdutosAsync()
        {
            return await _context.Produtos.ToListAsync();
        }
        public async Task DesativarProdutoAsync(int produtoId)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);

            if (produto == null)
            {
                throw new KeyNotFoundException($"Produto com ID {produtoId} não encontrado.");
            }

            produto.Status = StatusProduto.Inativo;
            produto.Versao++;

            await _context.SaveChangesAsync();

            await _eventPublisher.PublishAsync(new ProdutoDesativadoEvent
            {
                ProdutoId = produto.Id,
                Versao = produto.Versao,
                DataDesativacao = DateTime.UtcNow
            });
        }
        public async Task ProcessarNotaFiscalAsync(int notaFiscalId, List<NotaFiscalItem> itens)
        {
            var itensFalhos = new List<NotaFiscalItemFalhou>();
            var itensOk = new List<NotaFiscalItem>();
            var saldosAtualizados = new Dictionary<int, int>();
            var produtosAlterados = new HashSet<int>();
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var notaProcessada = new NotaProcessada
            {
                NotaFiscalId = notaFiscalId,
                DataProcessamento = DateTime.UtcNow
            };

            _context.NotasProcessadas.Add(notaProcessada);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception) when (NotaProcessadaDuplicada(exception))
            {
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear();
                return;
            }

            var produtoIds = itens
                .Select(item => item.ProdutoId)
                .Distinct()
                .ToList();

            var produtos = await _context.Produtos
                .Where(produto => produtoIds.Contains(produto.Id))
                .OrderBy(produto => produto.Id)
                .ToDictionaryAsync(produto => produto.Id);

            foreach (var item in itens)
            {
                if (!produtos.TryGetValue(item.ProdutoId, out var produto))
                {
                    itensFalhos.Add(new NotaFiscalItemFalhou
                    {
                        ProdutoId = item.ProdutoId,
                        Quantidade = item.Quantidade,
                        MotivoFalha = "Produto não encontrado"
                    });
                }
                else
                {
                    if (produto.Saldo == 0)
                    {
                        itensFalhos.Add(new NotaFiscalItemFalhou
                        {
                            ProdutoId = item.ProdutoId,
                            Quantidade = item.Quantidade,
                            MotivoFalha = "Saldo insuficiente"
                        });
                    }
                    else if (produto.Saldo < item.Quantidade)
                    {
                        itensOk.Add(new NotaFiscalItem
                        {
                            ProdutoId = item.ProdutoId,
                            Quantidade = produto.Saldo
                        });

                        itensFalhos.Add(new NotaFiscalItemFalhou
                        {
                            ProdutoId = item.ProdutoId,
                            Quantidade = item.Quantidade - produto.Saldo,
                            MotivoFalha = "Saldo insuficiente"
                        });

                        produto.Saldo = 0;
                        produtosAlterados.Add(produto.Id);

                        saldosAtualizados[produto.Id] = produto.Saldo;
                    }
                    else
                    {
                        produto.Saldo -= item.Quantidade;
                        produtosAlterados.Add(produto.Id);

                        itensOk.Add(item);

                        saldosAtualizados[produto.Id] = produto.Saldo;
                    }
                }
            }

            foreach (var produtoId in produtosAlterados)
            {
                produtos[produtoId].Versao++;
            }

            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear();
                throw;
            }

            foreach (var (produtoId, novoSaldo) in saldosAtualizados)
            {
                await _eventPublisher.PublishAsync(new EstoqueAtualizadoEvent
                {
                    ProdutoId = produtoId,
                    NovoSaldo = novoSaldo,
                    Versao = produtos[produtoId].Versao,
                    DataAtualizacao = DateTime.UtcNow
                });
            }

            await _eventPublisher.PublishAsync(new ProcessamentoDeNotaConcluidoEvent
            {
                NotaFiscalId = notaFiscalId,
                Itens = itensOk,
                ItensFalhos = itensFalhos
            });
        }

        private static bool NotaProcessadaDuplicada(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: "PK_notasProcessadas"
            };
        }
    }
}