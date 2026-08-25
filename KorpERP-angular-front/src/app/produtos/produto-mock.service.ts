import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { ProdutoDataService } from './produto-data.service';
import { Produto, ProdutoAtualizado, ProdutoCriado, StatusProduto } from './produto.model';
import { criarProdutosMock } from './produto.mock';

@Injectable()
export class ProdutoMockService implements ProdutoDataService {
    private produtos = criarProdutosMock();

    listar(): Observable<Produto[]> {
        return of(this.produtos.map((produto) => ({ ...produto })));
    }

    obter(id: number): Observable<Produto> {
        const produto = this.produtos.find((item) => item.id === id);
        return produto
            ? of({ ...produto })
            : throwError(() => new Error(`Produto ${id} nao encontrado.`));
    }

    criar(produto: ProdutoCriado): Observable<ProdutoCriado> {
        const id = Math.max(0, ...this.produtos.map((item) => item.id)) + 1;
        this.produtos = [
            ...this.produtos,
            {
                id,
                codigo: produto.codigo,
                descricao: produto.descricao,
                saldo: produto.saldoInicial,
                status: StatusProduto.Ativo,
            },
        ];
        return of({ ...produto });
    }

    atualizar(produto: ProdutoAtualizado): Observable<void> {
        const indice = this.produtos.findIndex((item) => item.id === produto.produtoId);
        if (indice < 0) {
            return throwError(() => new Error(`Produto ${produto.produtoId} nao encontrado.`));
        }

        this.produtos = this.produtos.map((item) => item.id === produto.produtoId
            ? {
                ...item,
                codigo: produto.novoCodigo,
                descricao: produto.novoDescricao,
                saldo: produto.novoSaldo,
            }
            : item);
        return of(undefined);
    }

    desativar(id: number): Observable<number> {
        const existe = this.produtos.some((produto) => produto.id === id);
        if (!existe) {
            return throwError(() => new Error(`Produto ${id} nao encontrado.`));
        }

        this.produtos = this.produtos.map((produto) => produto.id === id
            ? { ...produto, status: StatusProduto.Inativo }
            : produto);
        return of(id);
    }
}