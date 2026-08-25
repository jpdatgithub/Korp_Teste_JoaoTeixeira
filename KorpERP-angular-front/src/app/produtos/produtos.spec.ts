import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormControl, FormGroup } from '@angular/forms';
import { WritableSignal } from '@angular/core';
import { ProdutoDataService } from './produto-data.service';
import { ProdutoMockService } from './produto-mock.service';
import { Produto, StatusProduto } from './produto.model';
import { Produtos } from './produtos';

interface ProdutosTestHarness {
  produtos: WritableSignal<Produto[]>;
  selecionadoId: WritableSignal<number | null>;
  formulario: FormGroup<{
    codigo: FormControl<string>;
    descricao: FormControl<string>;
    saldo: FormControl<number>;
  }>;
  podeDesativar(): boolean;
  selecionarProduto(id: number | null): void;
  salvar(): void;
  desativar(): void;
}

describe('Produtos', () => {
  let component: Produtos;
  let fixture: ComponentFixture<Produtos>;
  let harness: ProdutosTestHarness;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Produtos],
      providers: [
        { provide: ProdutoDataService, useClass: ProdutoMockService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Produtos);
    component = fixture.componentInstance;
    harness = component as unknown as ProdutosTestHarness;
    fixture.detectChanges();
  });

  it('carrega os produtos iniciais pelo mock', () => {
    expect(component).toBeTruthy();
    expect(harness.produtos()).toHaveLength(6);
    expect(harness.produtos()[0].codigo).toBe('PRD-001');
  });

  it('seleciona o produto pelo botao de editar da linha', () => {
    const scrollIntoView = vi.fn();
    fixture.nativeElement.scrollIntoView = scrollIntoView;
    const botaoEditar = fixture.nativeElement.querySelector(
      'button[aria-label="Editar PRD-001"]',
    ) as HTMLButtonElement;

    botaoEditar.click();

    expect(harness.selecionadoId()).toBe(1);
    expect(harness.formulario.getRawValue()).toEqual({
      codigo: 'PRD-001',
      descricao: 'Parafuso sextavado',
      saldo: 24,
    });
    expect(scrollIntoView).toHaveBeenCalledWith({ behavior: 'smooth', block: 'start' });
  });

  it('desabilita a desativacao para um produto inativo', () => {
    harness.selecionarProduto(5);
    fixture.detectChanges();

    const botaoDesativar = fixture.nativeElement.querySelector(
      'button[aria-label="Desativar produto"]',
    ) as HTMLButtonElement;

    expect(harness.podeDesativar()).toBe(false);
    expect(botaoDesativar.disabled).toBe(true);
  });

  it('cria um produto e atualiza a lista', () => {
    harness.formulario.setValue({
      codigo: 'PRD-007',
      descricao: 'Produto novo',
      saldo: 15,
    });

    harness.salvar();

    expect(harness.produtos()).toHaveLength(7);
    expect(harness.produtos().at(-1)).toMatchObject({
      id: 7,
      codigo: 'PRD-007',
      saldo: 15,
      status: StatusProduto.Ativo,
    });
  });

  it('edita um produto existente', () => {
    harness.selecionarProduto(1);
    harness.formulario.setValue({
      codigo: 'PRD-001-A',
      descricao: 'Parafuso atualizado',
      saldo: 30,
    });

    harness.salvar();

    expect(harness.produtos().find((produto) => produto.id === 1)).toMatchObject({
      codigo: 'PRD-001-A',
      descricao: 'Parafuso atualizado',
      saldo: 30,
    });
  });

  it('desativa um produto existente', () => {
    harness.selecionarProduto(1);

    harness.desativar();

    expect(harness.produtos().find((produto) => produto.id === 1)?.status)
      .toBe(StatusProduto.Inativo);
  });
});
