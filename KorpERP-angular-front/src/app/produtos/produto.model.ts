export enum StatusProduto {
    Ativo,
    Inativo,
}

export interface Produto {
    id: number;
    codigo: string;
    descricao: string;
    saldo: number;
    status: StatusProduto;
}

export interface ProdutoCriado {
    codigo: string;
    descricao: string;
    saldoInicial: number;
}

export interface ProdutoAtualizado {
    produtoId: number;
    novoSaldo: number;
    novoCodigo: string;
    novoDescricao: string;
}