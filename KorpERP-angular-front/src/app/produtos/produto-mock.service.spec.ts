import { firstValueFrom } from 'rxjs';
import { ProdutoMockService } from './produto-mock.service';
import { StatusProduto } from './produto.model';

describe('ProdutoMockService', () => {
    let service: ProdutoMockService;

    beforeEach(() => {
        service = new ProdutoMockService();
    });

    it('lista copias dos produtos iniciais', async () => {
        const produtos = await firstValueFrom(service.listar());
        produtos[0].descricao = 'Alterado fora do servico';

        const novaListagem = await firstValueFrom(service.listar());

        expect(novaListagem).toHaveLength(6);
        expect(novaListagem[0].descricao).toBe('Parafuso sextavado');
    });

    it('obtem um produto e rejeita um id inexistente', async () => {
        await expect(firstValueFrom(service.obter(1))).resolves.toMatchObject({
            id: 1,
            codigo: 'PRD-001',
        });
        await expect(firstValueFrom(service.obter(999))).rejects.toThrow(
            'Produto 999 nao encontrado.',
        );
    });

    it('cria um produto ativo com id incremental', async () => {
        await firstValueFrom(service.criar({
            codigo: 'PRD-007',
            descricao: 'Produto novo',
            saldoInicial: 15,
        }));

        const criado = await firstValueFrom(service.obter(7));

        expect(criado).toEqual({
            id: 7,
            codigo: 'PRD-007',
            descricao: 'Produto novo',
            saldo: 15,
            status: StatusProduto.Ativo,
        });
    });

    it('atualiza e desativa um produto', async () => {
        await firstValueFrom(service.atualizar({
            produtoId: 1,
            novoCodigo: 'PRD-001-A',
            novoDescricao: 'Parafuso atualizado',
            novoSaldo: 30,
        }));
        await firstValueFrom(service.desativar(1));

        await expect(firstValueFrom(service.obter(1))).resolves.toMatchObject({
            codigo: 'PRD-001-A',
            descricao: 'Parafuso atualizado',
            saldo: 30,
            status: StatusProduto.Inativo,
        });
    });

    it('rejeita atualizacao e desativacao de produto inexistente', async () => {
        await expect(firstValueFrom(service.atualizar({
            produtoId: 999,
            novoCodigo: 'PRD-999',
            novoDescricao: 'Inexistente',
            novoSaldo: 0,
        }))).rejects.toThrow('Produto 999 nao encontrado.');
        await expect(firstValueFrom(service.desativar(999))).rejects.toThrow(
            'Produto 999 nao encontrado.',
        );
    });
});