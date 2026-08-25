import { Observable } from 'rxjs';
import { Nota, NotaFiscalItem, ProdutoProjection } from './nota.model';

export abstract class NotaDataService {
    abstract listar(): Observable<Nota[]>;
    abstract listarProdutos(): Observable<ProdutoProjection[]>;
    abstract obter(id: number): Observable<Nota>;
    abstract criar(nota: { itens: NotaFiscalItem[] }): Observable<Nota>;
    abstract atualizar(notaId: number, itens: NotaFiscalItem[]): Observable<Nota>;
    abstract excluir(id: number): Observable<void>;
    abstract processar(id: number): Observable<Nota>;
}
