import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Nota, NotaFiscalItem, ProdutoProjection } from './nota.model';
import { NotaDataService } from './nota-data.service';

@Injectable({ providedIn: 'root' })
export class NotaService implements NotaDataService {
    private readonly http = inject(HttpClient);
    private readonly apiUrl = 'http://localhost:5081/Notas';

    listar(): Observable<Nota[]> {
        return this.http.get<Nota[]>(`${this.apiUrl}/todos`);
    }

    listarProdutos(): Observable<ProdutoProjection[]> {
        return this.http.get<ProdutoProjection[]>(`${this.apiUrl}/produtos`);
    }

    obter(id: number): Observable<Nota> {
        return this.http.get<Nota>(`${this.apiUrl}/${id}`);
    }

    criar(nota: { itens: NotaFiscalItem[] }): Observable<Nota> {
        return this.http.post<Nota>(this.apiUrl, nota);
    }

    atualizar(notaId: number, itens: NotaFiscalItem[]): Observable<Nota> {
        return this.http.put<Nota>(this.apiUrl, { notaId: notaId, itens });
    }

    excluir(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

    processar(id: number): Observable<Nota> {
        return this.http.post<Nota>(`${this.apiUrl}/${id}/processar`, null);
    }
}
