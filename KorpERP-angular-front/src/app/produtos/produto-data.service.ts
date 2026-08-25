import { Observable } from 'rxjs';
import { Produto, ProdutoAtualizado, ProdutoCriado } from './produto.model';

export abstract class ProdutoDataService {
    abstract listar(): Observable<Produto[]>;
    abstract obter(id: number): Observable<Produto>;
    abstract criar(produto: ProdutoCriado): Observable<ProdutoCriado>;
    abstract atualizar(produto: ProdutoAtualizado): Observable<void>;
    abstract desativar(id: number): Observable<number>;
}