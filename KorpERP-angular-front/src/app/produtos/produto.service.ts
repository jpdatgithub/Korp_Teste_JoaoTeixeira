import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProdutoDataService } from './produto-data.service';
import { Produto, ProdutoAtualizado, ProdutoCriado } from './produto.model';

@Injectable({ providedIn: 'root' })
export class ProdutoService implements ProdutoDataService {
    private readonly http = inject(HttpClient);
    private readonly apiUrl = 'http://localhost:5086/Produtos';

    listar(): Observable<Produto[]> {
        return this.http.get<Produto[]>(`${this.apiUrl}/todos`);
    }

    obter(id: number): Observable<Produto> {
        return this.http.get<Produto>(`${this.apiUrl}/${id}`);
    }

    criar(produto: ProdutoCriado): Observable<ProdutoCriado> {
        return this.http.post<ProdutoCriado>(this.apiUrl, produto);
    }

    atualizar(produto: ProdutoAtualizado): Observable<void> {
        return this.http.put<void>(`${this.apiUrl}/atualizar-produto`, produto);
    }

    desativar(id: number): Observable<number> {
        return this.http.post<number>(`${this.apiUrl}/desativar-produto/${id}`, null);
    }
}