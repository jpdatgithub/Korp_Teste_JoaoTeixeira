import { Produto, StatusProduto } from './produto.model';

const PRODUTOS: Produto[] = [
    {
        id: 1,
        codigo: 'PRD-001',
        descricao: 'Parafuso sextavado',
        saldo: 24,
        status: StatusProduto.Ativo,
    },
    {
        id: 2,
        codigo: 'PRD-002',
        descricao: 'Arruela lisa',
        saldo: 12,
        status: StatusProduto.Ativo,
    },
    {
        id: 3,
        codigo: 'PRD-003',
        descricao: 'Porca travante',
        saldo: 7,
        status: StatusProduto.Ativo,
    },
    {
        id: 4,
        codigo: 'PRD-004',
        descricao: 'Chapa galvanizada',
        saldo: 0,
        status: StatusProduto.Ativo,
    },
    {
        id: 5,
        codigo: 'PRD-005',
        descricao: 'Cantoneira de aco',
        saldo: 18,
        status: StatusProduto.Inativo,
    },
    {
        id: 6,
        codigo: 'PRD-006',
        descricao: 'Rebite de aluminio',
        saldo: 40,
        status: StatusProduto.Ativo,
    },
];

export function criarProdutosMock(): Produto[] {
    return PRODUTOS.map((produto) => ({ ...produto }));
}