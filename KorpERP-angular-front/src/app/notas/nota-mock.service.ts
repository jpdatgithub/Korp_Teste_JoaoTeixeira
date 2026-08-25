import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { criarNotasMock, criarProdutosProjectionMock } from './notas.mock';
import { NotaDataService } from './nota-data.service';
import { Nota, NotaFiscalItem, ProdutoProjection, StatusNota } from './nota.model';

@Injectable()
export class NotaMockService implements NotaDataService {
    private notas = criarNotasMock();
    private produtos = criarProdutosProjectionMock();

    listar(): Observable<Nota[]> {
        return of(this.notas.map((nota) => ({ ...nota, itens: this.clonarItens(nota.itens), itensOk: this.clonarItens(nota.itensOk), itensFalhados: nota.itensFalhados.map((item) => ({ ...item })) })));
    }

    listarProdutos(): Observable<ProdutoProjection[]> {
        return of(this.produtos.map((produto) => ({ ...produto })));
    }

    obter(id: number): Observable<Nota> {
        const nota = this.notas.find((item) => item.id === id);
        return nota
            ? of(this.clonarNota(nota))
            : throwError(() => new Error(`Nota ${id} nao encontrada.`));
    }

    criar(nota: { itens: NotaFiscalItem[] }): Observable<Nota> {
        const id = Math.max(0, ...this.notas.map((item) => item.id)) + 1;
        const novaNota: Nota = {
            id,
            dataCriacao: new Date().toISOString(),
            dataFechamento: null,
            status: StatusNota.Aberta,
            emProcessamento: false,
            itens: this.clonarItens(nota.itens),
            itensOk: [],
            itensFalhados: [],
        };

        this.notas = [...this.notas, novaNota];
        return of(this.clonarNota(novaNota));
    }

    atualizar(notaId: number, itens: NotaFiscalItem[]): Observable<Nota> {
        const indice = this.notas.findIndex((item) => item.id === notaId);
        if (indice < 0) {
            return throwError(() => new Error(`Nota ${notaId} nao encontrada.`));
        }

        const notaAtualizada: Nota = {
            ...this.notas[indice],
            itens: this.clonarItens(itens),
            dataFechamento: this.notas[indice].status === StatusNota.Fechada ? this.notas[indice].dataFechamento : null,
        };

        this.notas = this.notas.map((item) => item.id === notaId ? notaAtualizada : item);
        return of(this.clonarNota(notaAtualizada));
    }

    excluir(id: number): Observable<void> {
        const existe = this.notas.some((nota) => nota.id === id);
        if (!existe) {
            return throwError(() => new Error(`Nota ${id} nao encontrada.`));
        }

        this.notas = this.notas.filter((nota) => nota.id !== id);
        return of(undefined);
    }

    processar(id: number): Observable<Nota> {
        const nota = this.notas.find((item) => item.id === id);
        if (!nota) {
            return throwError(() => new Error(`Nota ${id} nao encontrada.`));
        }

        const itensOk: Array<{ produtoId: number; quantidade: number }> = [];
        const itensFalhados: Array<{ produtoId: number; quantidade: number; motivoFalha: string }> = [];

        for (const item of nota.itens) {
            const produto = this.obterProduto(item.produtoId);

            if (produto && produto.saldo >= item.quantidade) {
                itensOk.push({ ...item });
                continue;
            }

            itensFalhados.push({
                produtoId: item.produtoId,
                quantidade: item.quantidade,
                motivoFalha: produto == null ? 'Produto nao encontrado' : 'Saldo insuficiente',
            });
        }

        const atualizada: Nota = {
            ...nota,
            status: StatusNota.Fechada,
            dataFechamento: new Date().toISOString(),
            emProcessamento: false,
            itensOk: itensOk.map((item) => ({ ...item })),
            itensFalhados: itensFalhados.map((item) => ({ ...item })),
        };

        this.notas = this.notas.map((item) => item.id === id ? atualizada : item);
        return of(this.clonarNota(atualizada));
    }

    private clonarNota(nota: Nota): Nota {
        return {
            ...nota,
            itens: this.clonarItens(nota.itens),
            itensOk: this.clonarItens(nota.itensOk),
            itensFalhados: nota.itensFalhados.map((item) => ({ ...item })),
        };
    }

    private clonarItens(itens: Array<{ produtoId: number; quantidade: number }>): Array<{ produtoId: number; quantidade: number }> {
        return itens.map((item) => ({ ...item }));
    }

    private obterProduto(produtoId: number) {
        return this.produtos.find((produto) => produto.produtoId === produtoId);
    }
}
