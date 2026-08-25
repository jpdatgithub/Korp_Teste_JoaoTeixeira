
using KorpERP.Produtos.API.Models;
using KorpERP.Produtos.API.Persistence;
using KorpERP.Produtos.API.Interfaces;
using KorpERP.Shared.Events;
using Microsoft.EntityFrameworkCore;
using KorpERP.Shared.Contracts.Produto;
using KorpERP.Shared.Contracts.NotaFiscal;

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
                DataCriacao = DateTime.UtcNow
            });

            if (produto.Saldo > 0)
            {
                await _eventPublisher.PublishAsync(new EstoqueAtualizadoEvent
                {
                    ProdutoId = produto.Id,
                    NovoSaldo = produto.Saldo,
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
            await _context.SaveChangesAsync();

            var dataAtualizacao = DateTime.UtcNow;

            if (saldoAlterado)
            {
                await _eventPublisher.PublishAsync(new EstoqueAtualizadoEvent
                {
                    ProdutoId = produto.Id,
                    NovoSaldo = novoSaldo,
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

            await _eventPublisher.PublishAsync(new ProdutoDesativadoEvent
            {
                ProdutoId = produto.Id,
                DataDesativacao = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }
        public async Task ProcessarNotaFiscalAsync(int notaFiscalId, List<NotaFiscalItem> itens)
        {
            var itensFalhos = new List<NotaFiscalItemFalhou>();
            var itensOk = new List<NotaFiscalItem>();
            var estoqueAtualizadoEvents = new List<EstoqueAtualizadoEvent>();
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var produtos = new Dictionary<int, Produto>();
            foreach (var produtoId in itens.Select(item => item.ProdutoId).Distinct().OrderBy(id => id))
            {
                var produto = await _context.Produtos
                    .FromSqlInterpolated($"SELECT * FROM \"Produtos\" WHERE \"Id\" = {produtoId} FOR UPDATE")
                    .SingleOrDefaultAsync();

                if (produto != null)
                {
                    produtos.Add(produtoId, produto);
                }
            }

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

                        estoqueAtualizadoEvents.Add(new EstoqueAtualizadoEvent
                        {
                            ProdutoId = produto.Id,
                            NovoSaldo = produto.Saldo,
                            DataAtualizacao = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        produto.Saldo -= item.Quantidade;

                        itensOk.Add(item);

                        estoqueAtualizadoEvents.Add(new EstoqueAtualizadoEvent
                        {
                            ProdutoId = produto.Id,
                            NovoSaldo = produto.Saldo,
                            DataAtualizacao = DateTime.UtcNow
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            foreach (var estoqueEvent in estoqueAtualizadoEvents)
            {
                await _eventPublisher.PublishAsync(estoqueEvent);
            }

            await _eventPublisher.PublishAsync(new ProcessamentoDeNotaConcluidoEvent
            {
                NotaFiscalId = notaFiscalId,
                Itens = itensOk,
                ItensFalhos = itensFalhos
            });
        }
    }
}