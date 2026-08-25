import { DatePipe } from '@angular/common';
import { Component, computed, DestroyRef, ElementRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { finalize, retry, Subject, switchMap, takeUntil, takeWhile, timer } from 'rxjs';
import { NotaDataService } from './nota-data.service';
import {
  Nota,
  NotaFiscalItem,
  ProdutoProjection,
  StatusNota,
  StatusProdutoProjection,
} from './nota.model';
import { criarProdutosProjectionMock } from './notas.mock';

@Component({
  imports: [
    DatePipe,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatSelectModule,
    MatSnackBarModule,
    MatTooltipModule,
  ],
  selector: 'app-notas',
  styleUrl: './notas.scss',
  templateUrl: './notas.html',
})
export class Notas {
  private readonly notaService = inject(NotaDataService);
  private readonly elementRef = inject<ElementRef<HTMLElement>>(ElementRef);
  private readonly destroyRef = inject(DestroyRef);
  private readonly snackBar = inject(MatSnackBar);
  private readonly cancelarPolling = new Subject<void>();

  protected readonly notas = signal<Nota[]>([]);
  protected readonly produtos = signal<ProdutoProjection[]>(criarProdutosProjectionMock());
  protected readonly notaSelecionadaId = signal<number | null>(null);
  protected readonly editando = signal(true);
  protected readonly processandoNota = signal(false);
  protected readonly itensRascunho = signal<NotaFiscalItem[]>([]);
  protected readonly StatusNota = StatusNota;

  protected readonly notaSelecionada = computed(() => {
    const id = this.notaSelecionadaId();
    return id === null ? null : this.notas().find((nota) => nota.id === id) ?? null;
  });

  protected readonly notaConcluida = computed(() => {
    const nota = this.notaSelecionada();
    return nota?.status === StatusNota.Fechada && !nota.emProcessamento ? nota : null;
  });

  protected readonly produtosDisponiveis = computed(() => {
    const idsAdicionados = new Set(this.itensRascunho().map((item) => item.produtoId));
    return this.produtos().filter(
      (produto) => produto.status === StatusProdutoProjection.Ativo
        && produto.saldo > 0
        && !idsAdicionados.has(produto.produtoId),
    );
  });

  protected readonly podeSalvar = computed(
    () => this.editando() && this.itensRascunho().length > 0,
  );

  ngOnInit(): void {
    this.carregarNotas();
  }

  protected selecionarNota(id: number | null): void {
    this.cancelarPolling.next();

    if (id === null) {
      this.novaNota();
      return;
    }

    this.notaService
      .obter(id)
      .pipe(finalize(() => undefined))
      .subscribe({
        next: (nota) => {
          this.notaSelecionadaId.set(nota.id);
          this.itensRascunho.set(this.copiarItens(nota.itens));
          this.editando.set(false);
          this.atualizarNotaNaLista(nota);
          if (nota.emProcessamento) {
            this.acompanharProcessamento(nota.id);
          }
        },
        error: () => this.exibirErro('Nao foi possivel carregar a nota.'),
      });
  }

  protected editarNota(id: number): void {
    this.selecionarNota(id);
    this.elementRef.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

  protected novaNota(): void {
    this.cancelarPolling.next();
    this.notaSelecionadaId.set(null);
    this.itensRascunho.set([]);
    this.editando.set(true);
  }

  protected editarOuSalvar(): void {
    if (this.editando()) {
      this.salvar();
      return;
    }

    const nota = this.notaSelecionada();
    if (!nota || nota.status === StatusNota.Fechada) {
      return;
    }

    this.itensRascunho.set(this.copiarItens(nota.itens));
    this.editando.set(true);
  }

  protected excluirNota(): void {
    const id = this.notaSelecionadaId();
    if (id === null) {
      return;
    }

    this.notaService.excluir(id).subscribe({
      next: () => {
        this.novaNota();
        this.carregarNotas();
      },
    });
  }

  protected processarNota(): void {
    const nota = this.notaSelecionada();
    if (!nota || nota.status === StatusNota.Fechada || this.processandoNota()) {
      return;
    }

    this.processandoNota.set(true);
    this.notaService
      .processar(nota.id)
      .pipe(
        finalize(() => this.processandoNota.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (notaProcessada) => {
          this.atualizarNotaNaLista(notaProcessada);
          this.itensRascunho.set(this.copiarItens(notaProcessada.itens));
          if (notaProcessada.emProcessamento) {
            this.acompanharProcessamento(notaProcessada.id);
          }
        },
        error: () => this.exibirErro('Nao foi possivel iniciar o processamento da nota.'),
      });
  }

  protected adicionarProduto(produtoId: number | null): void {
    if (produtoId === null || !this.editando()) {
      return;
    }

    const disponivel = this.produtosDisponiveis().some(
      (produto) => produto.produtoId === produtoId,
    );
    if (!disponivel) {
      return;
    }

    this.itensRascunho.update((itens) => [...itens, { produtoId, quantidade: 1 }]);
  }

  protected incrementar(item: NotaFiscalItem): void {
    const produto = this.obterProduto(item.produtoId);
    if (!this.editando() || !produto || item.quantidade >= produto.saldo) {
      return;
    }

    this.alterarQuantidade(item.produtoId, item.quantidade + 1);
  }

  protected decrementar(item: NotaFiscalItem): void {
    if (!this.editando() || item.quantidade <= 1) {
      return;
    }

    this.alterarQuantidade(item.produtoId, item.quantidade - 1);
  }

  protected removerItem(produtoId: number): void {
    if (!this.editando()) {
      return;
    }

    this.itensRascunho.update((itens) => itens.filter((item) => item.produtoId !== produtoId));
  }

  protected obterProduto(produtoId: number): ProdutoProjection | undefined {
    return this.produtos().find((produto) => produto.produtoId === produtoId);
  }

  protected formatarCampoProduto(valor: string | null | undefined): string {
    return valor?.trim() || '(Vazio)';
  }

  protected atingiuEstoque(item: NotaFiscalItem): boolean {
    const saldo = this.obterProduto(item.produtoId)?.saldo ?? 0;
    return item.quantidade >= saldo;
  }

  protected nomeStatus(nota: Nota): string {
    if (nota.emProcessamento) {
      return 'Em processamento';
    }

    return nota.status === StatusNota.Fechada ? 'Fechada' : 'Aberta';
  }

  private salvar(): void {
    if (!this.podeSalvar()) {
      return;
    }

    const id = this.notaSelecionadaId();
    const itens = this.copiarItens(this.itensRascunho());
    const operacao = id === null
      ? this.notaService.criar({ itens })
      : this.notaService.atualizar(id, itens);

    operacao.subscribe({
      next: (nota) => {
        this.notaSelecionadaId.set(nota.id);
        this.itensRascunho.set(this.copiarItens(nota.itens));
        this.editando.set(false);
        this.carregarNotas();
      },
    });
  }

  private alterarQuantidade(produtoId: number, quantidade: number): void {
    this.itensRascunho.update((itens) => itens.map(
      (item) => item.produtoId === produtoId ? { ...item, quantidade } : item,
    ));
  }

  private carregarNotas(): void {
    this.notaService
      .listar()
      .pipe(finalize(() => undefined))
      .subscribe({
        next: (notas) => this.notas.set(notas),
      });
  }

  private acompanharProcessamento(notaId: number): void {
    this.cancelarPolling.next();
    timer(0, 3000)
      .pipe(
        switchMap(() => this.notaService.obter(notaId).pipe(
          retry({ count: 2, delay: 1000 }),
        )),
        takeWhile((nota) => nota.emProcessamento, true),
        takeUntil(this.cancelarPolling),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (nota) => {
          this.atualizarNotaNaLista(nota);
          if (!nota.emProcessamento) {
            this.itensRascunho.set(this.copiarItens(nota.itens));
          }
        },
        error: () => this.exibirErro('Nao foi possivel acompanhar o processamento da nota.'),
      });
  }

  private atualizarNotaNaLista(notaAtualizada: Nota): void {
    this.notas.update((notas) => notas.map(
      (nota) => nota.id === notaAtualizada.id ? notaAtualizada : nota,
    ));
  }

  private exibirErro(mensagem: string): void {
    this.snackBar.open(mensagem, 'Fechar', { duration: 5000 });
  }

  private copiarItens(itens: NotaFiscalItem[]): NotaFiscalItem[] {
    return itens.map((item) => ({ ...item }));
  }
}
