import {
    Nota,
    ProdutoProjection,
    StatusNota,
    StatusProdutoProjection,
} from './nota.model';

const NOTAS: Nota[] = [
    {
        id: 1001,
        dataCriacao: '2026-08-18T13:30:00.000Z',
        dataFechamento: null,
        status: StatusNota.Aberta,
        emProcessamento: false,
        itens: [
            { produtoId: 1, quantidade: 4 },
            { produtoId: 2, quantidade: 2 },
            { produtoId: 7, quantidade: 3 },
        ],
        itensOk: [],
        itensFalhados: [],
    },
    {
        id: 1002,
        dataCriacao: '2026-08-19T16:45:00.000Z',
        dataFechamento: '2026-08-19T17:10:00.000Z',
        status: StatusNota.Fechada,
        emProcessamento: false,
        itens: [
            { produtoId: 3, quantidade: 7 },
            { produtoId: 4, quantidade: 2 },
            { produtoId: 7, quantidade: 3 },
            { produtoId: 999, quantidade: 1 },
        ],
        itensOk: [
            { produtoId: 3, quantidade: 7 },
            { produtoId: 7, quantidade: 3 },
        ],
        itensFalhados: [
            { produtoId: 4, quantidade: 2, motivoFalha: 'Saldo insuficiente' },
            { produtoId: 999, quantidade: 1, motivoFalha: 'Produto nao encontrado' },
        ],
    },
];

const PRODUTOS: ProdutoProjection[] = [
    {
        produtoId: 1,
        codigo: 'PRD-001',
        descricao: 'Parafuso sextavado',
        saldo: 24,
        status: StatusProdutoProjection.Ativo,
    },
    {
        produtoId: 2,
        codigo: 'PRD-002',
        descricao: 'Arruela lisa',
        saldo: 12,
        status: StatusProdutoProjection.Ativo,
    },
    {
        produtoId: 3,
        codigo: 'PRD-003',
        descricao: 'Porca travante',
        saldo: 7,
        status: StatusProdutoProjection.Ativo,
    },
    {
        produtoId: 4,
        codigo: 'PRD-004',
        descricao: 'Chapa galvanizada',
        saldo: 0,
        status: StatusProdutoProjection.Ativo,
    },
    {
        produtoId: 5,
        codigo: 'PRD-005',
        descricao: 'Cantoneira de aco',
        saldo: 18,
        status: StatusProdutoProjection.Inativo,
    },
    {
        produtoId: 6,
        codigo: 'PRD-006',
        descricao: 'Rebite de aluminio',
        saldo: 40,
        status: StatusProdutoProjection.Ativo,
    },
    {
        produtoId: 7,
        codigo: '',
        descricao: '',
        saldo: 5,
        status: StatusProdutoProjection.Ativo,
    },
];

export function criarNotasMock(): Nota[] {
    return NOTAS.map((nota) => ({
        ...nota,
        itens: nota.itens.map((item) => ({ ...item })),
        itensOk: nota.itensOk.map((item) => ({ ...item })),
        itensFalhados: nota.itensFalhados.map((item) => ({ ...item })),
    }));
}

export function criarProdutosProjectionMock(): ProdutoProjection[] {
    return PRODUTOS.map((produto) => ({ ...produto }));
}