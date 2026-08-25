export enum StatusNota {
    Aberta,
    Fechada,
}

export enum StatusProdutoProjection {
    Ativo,
    Inativo,
}

export interface ProdutoProjection {
    produtoId: number;
    codigo: string;
    descricao: string;
    saldo: number;
    status: StatusProdutoProjection;
}

export interface NotaFiscalItem {
    produtoId: number;
    quantidade: number;
}

export interface NotaFiscalItemFalhado extends NotaFiscalItem {
    motivoFalha: string;
}

export interface Nota {
    id: number;
    dataCriacao: string;
    dataFechamento: string | null;
    status: StatusNota;
    emProcessamento: boolean;
    itens: NotaFiscalItem[];
    itensOk: NotaFiscalItem[];
    itensFalhados: NotaFiscalItemFalhado[];
}