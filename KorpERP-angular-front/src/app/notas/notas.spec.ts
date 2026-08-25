import { WritableSignal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { vi } from 'vitest';
import { NotaDataService } from './nota-data.service';
import { NotaMockService } from './nota-mock.service';
import { Nota, NotaFiscalItem, ProdutoProjection } from './nota.model';
import { Notas } from './notas';

interface NotasTestHarness {
  notas: WritableSignal<Nota[]>;
  produtos: WritableSignal<ProdutoProjection[]>;
  notaSelecionadaId: WritableSignal<number | null>;
  notaSelecionada(): Nota | null;
  editando: WritableSignal<boolean>;
  processandoNota: WritableSignal<boolean>;
  itensRascunho: WritableSignal<NotaFiscalItem[]>;
  notaSelecionada(): Nota | null;
  produtosDisponiveis(): ProdutoProjection[];
  podeSalvar(): boolean;
  selecionarNota(id: number | null): void;
  editarNota(id: number): void;
  novaNota(): void;
  editarOuSalvar(): void;
  excluirNota(): void;
  processarNota(): void;
  adicionarProduto(produtoId: number | null): void;
  incrementar(item: NotaFiscalItem): void;
  decrementar(item: NotaFiscalItem): void;
  removerItem(produtoId: number): void;
  formatarCampoProduto(valor: string | null | undefined): string;
  atingiuEstoque(item: NotaFiscalItem): boolean;
}

describe('Notas', () => {
  let component: Notas;
  let notas: NotasTestHarness;
  let fixture: ComponentFixture<Notas>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Notas],
      providers: [{ provide: NotaDataService, useClass: NotaMockService }],
    }).compileComponents();

    fixture = TestBed.createComponent(Notas);
    component = fixture.componentInstance;
    notas = component as unknown as NotasTestHarness;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('carrega os produtos do servico ao inicializar', () => {
    const service = TestBed.inject(NotaDataService);
    const listarProdutos = vi.spyOn(service, 'listarProdutos');

    component.ngOnInit();

    expect(listarProdutos).toHaveBeenCalledOnce();
    expect(notas.produtos()).not.toEqual([]);
  });

  it('inicia com uma nova nota vazia em edicao', () => {
    expect(notas.notaSelecionadaId()).toBeNull();
    expect(notas.editando()).toBe(true);
    expect(notas.itensRascunho()).toEqual([]);
    expect(notas.podeSalvar()).toBe(false);
  });

  it('lista somente os dados basicos das notas na tabela', () => {
    fixture.detectChanges();

    const tabela = (fixture.nativeElement as HTMLElement).querySelector('.table-scroll');
    expect(tabela?.textContent).toContain('#1001');
    expect(tabela?.textContent).toContain('18/08/2026 10:30');
    expect(tabela?.textContent).toContain('Aberta');
    expect(tabela?.textContent).not.toContain('PRD-001');
    expect(tabela?.textContent).not.toContain('Quantidade');
  });

  it('seleciona a nota pelo botao de editar da linha', () => {
    fixture.detectChanges();
    const scrollIntoView = vi.fn();
    fixture.nativeElement.scrollIntoView = scrollIntoView;
    const botaoEditar = fixture.nativeElement.querySelector(
      'button[aria-label="Editar nota #1001"]',
    ) as HTMLButtonElement;

    botaoEditar.click();
    fixture.detectChanges();

    expect(notas.notaSelecionadaId()).toBe(1001);
    expect(notas.itensRascunho()).toHaveLength(3);
    expect(fixture.nativeElement.querySelector('.selected-row')?.textContent).toContain('#1001');
    expect(scrollIntoView).toHaveBeenCalledWith({ behavior: 'smooth', block: 'start' });
  });

  it('formata campos vazios de produto sem alterar campos preenchidos', () => {
    expect(notas.formatarCampoProduto('')).toBe('(Vazio)');
    expect(notas.formatarCampoProduto('   ')).toBe('(Vazio)');
    expect(notas.formatarCampoProduto(null)).toBe('(Vazio)');
    expect(notas.formatarCampoProduto('PRD-007')).toBe('PRD-007');
  });

  it('carrega uma nota existente bloqueada para edicao', () => {
    notas.selecionarNota(1001);
    fixture.detectChanges();

    expect(notas.notaSelecionadaId()).toBe(1001);
    expect(notas.editando()).toBe(false);
    expect(notas.itensRascunho()).toEqual([
      { produtoId: 1, quantidade: 4 },
      { produtoId: 2, quantidade: 2 },
      { produtoId: 7, quantidade: 3 },
    ]);

    const element = fixture.nativeElement as HTMLElement;
    const projectionIncompleta = element.querySelectorAll<HTMLElement>('.note-item')[2];
    expect(projectionIncompleta.querySelector('.product-data')?.textContent).toContain('(Vazio)');
    expect(projectionIncompleta.querySelector('.product-data')?.textContent).not.toContain('Cadastro indisponível');
    expect(projectionIncompleta.querySelector('.stock')?.textContent).toContain('5');
  });

  it('permite editar item com cadastro incompleto durante a edicao da nota', () => {
    notas.selecionarNota(1001);
    notas.editarOuSalvar();
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const itemSemCadastro = element.querySelectorAll<HTMLElement>('.note-item')[2];
    expect(itemSemCadastro.querySelector('.quantity-control')).not.toBeNull();
    expect(itemSemCadastro.querySelector('[aria-label="Diminuir quantidade"]')).not.toBeNull();
    expect(itemSemCadastro.querySelector('[aria-label="Aumentar quantidade"]')).not.toBeNull();
    expect(itemSemCadastro.querySelector('[aria-label="Remover (Vazio)"]')).not.toBeNull();
  });

  it('exibe itens OK e falhados no lugar dos itens originais ao concluir a nota', () => {
    notas.selecionarNota(1002);
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const successItems = element.querySelectorAll<HTMLElement>('.success-item');
    const failedItems = element.querySelectorAll('.failed-item');
    const editButton = element.querySelector<HTMLButtonElement>('[aria-label="Editar nota"]');
    const printButton = element.querySelector<HTMLButtonElement>('[aria-label="Imprimir nota"]');

    expect(element.textContent).toContain('Resultado do processamento');
    expect(element.textContent).toContain('Itens OK');
    expect(element.textContent).toContain('Itens falhados');
    expect(element.textContent).toContain('Saldo insuficiente');
    expect(element.textContent).toContain('Produto nao encontrado');
    expect(element.textContent).toContain('Produto #999');
    expect(successItems).toHaveLength(2);
    expect(successItems[1].querySelector('.product-data')?.textContent).toContain('(Vazio)');
    expect(successItems[1].querySelector('.product-data')?.textContent).not.toContain('Cadastro indisponível');
    expect(successItems[1].querySelector('.stock')?.textContent).toContain('5');
    expect(failedItems).toHaveLength(2);
    expect(element.querySelector('.add-item-row')).toBeNull();
    expect(editButton?.disabled).toBe(true);
    expect(printButton?.disabled).toBe(true);
  });

  it('mantem os itens originais enquanto a nota fechada esta em processamento', () => {
    notas.selecionarNota(1002);
    notas.notas.update((lista) => lista.map((nota) => nota.id === 1002
      ? { ...nota, emProcessamento: true }
      : nota));
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.textContent).toContain('Itens da nota');
    expect(element.querySelector('.status')?.textContent).toContain('Em processamento');
    expect(element.textContent).not.toContain('Resultado do processamento');
    expect(element.querySelector('.failed-list')).toBeNull();
  });

  it('processa a nota e atualiza o resultado em background mesmo apos desselecionar', () => {
    vi.useFakeTimers();
    notas.selecionarNota(1001);
    const service = TestBed.inject(NotaDataService);
    const notaAberta = notas.notas().find((nota) => nota.id === 1001)!;
    const emProcessamento: Nota = {
      ...notaAberta,
      status: 1,
      dataFechamento: new Date().toISOString(),
      emProcessamento: true,
    };
    const concluida: Nota = {
      ...emProcessamento,
      emProcessamento: false,
      itensOk: [{ produtoId: 1, quantidade: 4 }],
      itensFalhados: [{ produtoId: 2, quantidade: 2, motivoFalha: 'Saldo insuficiente' }],
    };
    const processar = vi.spyOn(service, 'processar').mockReturnValue(of(emProcessamento));
    const listarProdutos = vi.spyOn(service, 'listarProdutos');
    const obter = vi.spyOn(service, 'obter')
      .mockReturnValueOnce(of(emProcessamento))
      .mockReturnValueOnce(of(concluida));

    notas.processarNota();
    vi.advanceTimersByTime(0);

    expect(processar).toHaveBeenCalledWith(1001);
    expect(obter).toHaveBeenCalledTimes(1);
    expect(listarProdutos).not.toHaveBeenCalled();
    expect(notas.notaSelecionada()?.emProcessamento).toBe(true);

    notas.novaNota();
    expect(notas.notaSelecionada()).toBeNull();

    vi.advanceTimersByTime(3000);
    fixture.detectChanges();

    expect(obter).toHaveBeenCalledTimes(2);
    expect(listarProdutos).toHaveBeenCalledOnce();
    expect(notas.notaSelecionada()).toBeNull();
    expect(notas.notas().find((nota) => nota.id === 1001)?.emProcessamento).toBe(false);
    expect(notas.notas().find((nota) => nota.id === 1001)?.itensOk)
      .toEqual([{ produtoId: 1, quantidade: 4 }]);

    vi.advanceTimersByTime(6000);
    expect(obter).toHaveBeenCalledTimes(2);
    vi.useRealTimers();
  });

  it('limita verticalmente a lista de itens da nota', () => {
    notas.selecionarNota(1001);
    fixture.detectChanges();

    const lista = (fixture.nativeElement as HTMLElement).querySelector('.items-list') as HTMLElement;
    const estilo = getComputedStyle(lista);
    expect(estilo.maxHeight).toBe('410px');
    expect(estilo.overflowY).toBe('auto');
  });

  it('filtra produtos inativos, sem saldo e ja adicionados', async () => {
    notas.selecionarNota(1001);
    notas.editarOuSalvar();
    notas.produtos.update((produtos) => produtos.map((produto) => {
      if (produto.produtoId === 3) {
        return { ...produto, codigo: '', descricao: 'Porca travante' };
      }
      if (produto.produtoId === 6) {
        return { ...produto, codigo: 'PRD-006', descricao: '   ' };
      }
      return produto;
    }));
    fixture.detectChanges();

    expect(notas.produtosDisponiveis().map((produto) => produto.produtoId)).toEqual([3, 6]);

    const seletor = (fixture.nativeElement as HTMLElement).querySelector<HTMLElement>(
      '.add-item-row mat-select',
    );
    seletor?.click();
    fixture.detectChanges();
    await fixture.whenStable();

    const opcoes = Array.from(
      document.body.querySelectorAll('mat-option'),
      (opcao) => opcao.textContent?.trim(),
    );
    expect(opcoes).toEqual([
      '(Vazio) - Porca travante (7 disponiveis)',
      'PRD-006 - (Vazio) (40 disponiveis)',
    ]);

    notas.adicionarProduto(4);
    notas.adicionarProduto(5);
    expect(notas.itensRascunho()).toEqual([
      { produtoId: 1, quantidade: 4 },
      { produtoId: 2, quantidade: 2 },
      { produtoId: 7, quantidade: 3 },
    ]);
  });

  it('adiciona um produto com uma unidade e permite remove-lo', () => {
    notas.adicionarProduto(6);

    expect(notas.itensRascunho()).toEqual([{ produtoId: 6, quantidade: 1 }]);
    notas.removerItem(6);
    expect(notas.itensRascunho()).toEqual([]);
  });

  it('respeita os limites inferior e de estoque da quantidade', () => {
    notas.adicionarProduto(3);
    const item = notas.itensRascunho()[0];

    notas.decrementar(item);
    expect(notas.itensRascunho()[0].quantidade).toBe(1);

    notas.itensRascunho.set([{ produtoId: 3, quantidade: 6 }]);
    notas.incrementar(notas.itensRascunho()[0]);
    expect(notas.itensRascunho()[0].quantidade).toBe(7);
    expect(notas.atingiuEstoque(notas.itensRascunho()[0])).toBe(true);

    notas.incrementar(notas.itensRascunho()[0]);
    expect(notas.itensRascunho()[0].quantidade).toBe(7);
  });

  it('cria, atualiza e exclui notas na lista local', () => {
    notas.adicionarProduto(6);
    notas.editarOuSalvar();

    const novoId = notas.notaSelecionadaId();
    expect(novoId).toBe(1003);
    expect(notas.notas().find((nota) => nota.id === novoId)?.itens).toEqual([
      { produtoId: 6, quantidade: 1 },
    ]);
    expect(notas.editando()).toBe(false);

    notas.editarOuSalvar();
    notas.incrementar(notas.itensRascunho()[0]);
    notas.editarOuSalvar();
    expect(notas.notas().find((nota) => nota.id === novoId)?.itens[0].quantidade).toBe(2);

    notas.excluirNota();
    expect(notas.notas().some((nota) => nota.id === novoId)).toBe(false);
    expect(notas.notaSelecionadaId()).toBeNull();
    expect(notas.editando()).toBe(true);
  });
});
