import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { finalize, Observable } from 'rxjs';
import { ProdutoDataService } from './produto-data.service';
import { Produto, StatusProduto } from './produto.model';

@Component({
  imports: [
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatSnackBarModule,
    MatTooltipModule,
    ReactiveFormsModule,
  ],
  selector: 'app-produtos',
  styleUrl: './produtos.scss',
  templateUrl: './produtos.html',
})
export class Produtos implements OnInit {
  private readonly produtoService = inject(ProdutoDataService);
  private readonly snackBar = inject(MatSnackBar);

  protected readonly produtos = signal<Produto[]>([]);
  protected readonly selecionadoId = signal<number | null>(null);
  protected readonly carregando = signal(false);
  protected readonly salvando = signal(false);
  protected readonly StatusProduto = StatusProduto;

  protected readonly formulario = new FormGroup({
    codigo: new FormControl('', { nonNullable: true, validators: Validators.required }),
    descricao: new FormControl('', { nonNullable: true, validators: Validators.required }),
    saldo: new FormControl(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0)],
    }),
  });
  private readonly valoresIniciais = signal(this.formulario.getRawValue());
  private readonly valoresAtuais = toSignal(this.formulario.valueChanges, {
    initialValue: this.formulario.getRawValue(),
  });
  protected readonly temAlteracoes = computed(() => {
    const atuais = this.valoresAtuais();
    const iniciais = this.valoresIniciais();
    return atuais.codigo !== iniciais.codigo
      || atuais.descricao !== iniciais.descricao
      || atuais.saldo !== iniciais.saldo;
  });
  protected readonly podeDesativar = computed(() => {
    const selecionadoId = this.selecionadoId();
    return selecionadoId !== null
      && this.produtos().some((produto) => produto.id === selecionadoId
        && produto.status === StatusProduto.Ativo);
  });

  ngOnInit(): void {
    this.carregarProdutos();
  }

  protected selecionarProduto(id: number | null): void {
    if (id === null) {
      this.novoProduto();
      return;
    }

    this.carregando.set(true);
    this.produtoService
      .obter(id)
      .pipe(finalize(() => this.carregando.set(false)))
      .subscribe({
        next: (produto) => {
          this.selecionadoId.set(produto.id);
          const valores = {
            codigo: produto.codigo,
            descricao: produto.descricao,
            saldo: produto.saldo,
          };
          this.valoresIniciais.set(valores);
          this.formulario.reset(valores);
        },
        error: () => this.exibirErro('Nao foi possivel carregar o produto.'),
      });
  }

  protected novoProduto(): void {
    this.selecionadoId.set(null);
    const valores = { codigo: '', descricao: '', saldo: 0 };
    this.valoresIniciais.set(valores);
    this.formulario.reset(valores);
  }

  protected editarProduto(id: number): void {
    this.selecionarProduto(id);
  }

  protected salvar(): void {
    if (this.formulario.invalid || !this.temAlteracoes() || this.salvando()) {
      return;
    }

    const { codigo, descricao, saldo } = this.formulario.getRawValue();
    const selecionadoId = this.selecionadoId();
    const operacao: Observable<unknown> = selecionadoId === null
      ? this.produtoService.criar({ codigo, descricao, saldoInicial: saldo })
      : this.produtoService.atualizar({
        produtoId: selecionadoId,
        novoCodigo: codigo,
        novoDescricao: descricao,
        novoSaldo: saldo,
      });

    this.salvando.set(true);
    operacao.pipe(finalize(() => this.salvando.set(false))).subscribe({
      next: () => {
        this.snackBar.open('Produto salvo com sucesso.', 'Fechar', { duration: 3000 });
        this.novoProduto();
        this.carregarProdutos();
      },
      error: () => this.exibirErro('Nao foi possivel salvar o produto.'),
    });
  }

  protected desativar(): void {
    const id = this.selecionadoId();
    if (id === null || !this.podeDesativar() || this.salvando()) {
      return;
    }

    this.salvando.set(true);
    this.produtoService
      .desativar(id)
      .pipe(finalize(() => this.salvando.set(false)))
      .subscribe({
        next: () => {
          this.snackBar.open('Produto desativado.', 'Fechar', { duration: 3000 });
          this.novoProduto();
          this.carregarProdutos();
        },
        error: () => this.exibirErro('Nao foi possivel desativar o produto.'),
      });
  }

  protected nomeStatus(status: StatusProduto): string {
    return status === StatusProduto.Inativo ? 'Inativo' : 'Ativo';
  }

  private carregarProdutos(): void {
    this.carregando.set(true);
    this.produtoService
      .listar()
      .pipe(finalize(() => this.carregando.set(false)))
      .subscribe({
        next: (produtos) => this.produtos.set(produtos),
        error: () => this.exibirErro('Nao foi possivel carregar os produtos.'),
      });
  }

  private exibirErro(mensagem: string): void {
    this.snackBar.open(mensagem, 'Fechar', { duration: 5000 });
  }
}
