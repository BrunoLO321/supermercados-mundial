import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  OnInit,
  inject
} from '@angular/core';
import { FormsModule } from '@angular/forms';

import {
  CriarProdutoRequest,
  Produto,
  ProdutoService
} from '../../../core/produto.service';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './products.html',
  styleUrl: './products.scss'
})
export class Products implements OnInit {
  private readonly produtoService = inject(ProdutoService);
  private readonly cdr = inject(ChangeDetectorRef);

  produtos: Produto[] = [];
  busca = '';

  modalAberto = false;
  carregando = false;
  salvando = false;

  mensagemSucesso = '';
  mensagemErro = '';

  codigo = '';
  descricao = '';
  preco: number | null = null;

  ngOnInit(): void {
    this.carregarProdutos();
  }

  /**
   * Filtra os produtos por código ou descrição.
   */
  get produtosFiltrados(): Produto[] {
    const termo = this.busca.trim().toLocaleLowerCase('pt-BR');

    if (!termo) {
      return this.produtos;
    }

    return this.produtos.filter(produto =>
      (produto.codigo ?? '')
        .toLocaleLowerCase('pt-BR')
        .includes(termo) ||
      (produto.descricao ?? '')
        .toLocaleLowerCase('pt-BR')
        .includes(termo)
    );
  }

  /**
   * Carrega os produtos cadastrados na API.
   */
  carregarProdutos(): void {
    this.carregando = true;
    this.mensagemErro = '';

    this.produtoService.listar().subscribe({
      next: (produtos: Produto[]) => {
        this.produtos = produtos;
        this.carregando = false;

        console.log('Produtos carregados:', this.produtos);

        this.cdr.detectChanges();
      },

      error: (erro: unknown) => {
        this.carregando = false;
        this.mensagemErro =
          'Não foi possível carregar os produtos. Verifique a API e sua autenticação.';

        console.error('Erro ao carregar produtos:', erro);

        this.cdr.detectChanges();
      }
    });
  }

  /**
   * Abre o formulário de cadastro.
   */
  abrirModal(): void {
    this.limparFormulario();
    this.mensagemErro = '';
    this.modalAberto = true;
  }

  /**
   * Fecha o formulário, exceto durante o salvamento.
   */
  fecharModal(): void {
    if (this.salvando) {
      return;
    }

    this.modalAberto = false;
  }

  /**
   * Valida e cadastra um produto na API.
   */
  cadastrar(): void {
    this.mensagemErro = '';
    this.mensagemSucesso = '';

    const codigo = this.codigo.trim();
    const descricao = this.descricao.trim();
    const preco = this.preco;

    if (
      !codigo ||
      !descricao ||
      preco === null ||
      !Number.isFinite(preco) ||
      preco < 0
    ) {
      this.mensagemErro =
        'Informe o código, a descrição e um preço válido igual ou superior a zero.';
      return;
    }

    const request: CriarProdutoRequest = {
      codigo,
      descricao,
      preco
    };

    this.salvando = true;

    this.produtoService.criar(request).subscribe({
      next: response => {
        this.salvando = false;
        this.modalAberto = false;
        this.limparFormulario();

        this.mensagemSucesso =
          response.mensagem || 'Produto cadastrado com sucesso.';

        this.cdr.detectChanges();

        // Atualiza a listagem após o cadastro.
        this.carregarProdutos();
      },

      error: (erro: { status?: number }) => {
        this.salvando = false;

        if (erro.status === 409) {
          this.mensagemErro =
            'Já existe um produto com esse código.';
        } else if (erro.status === 400) {
          this.mensagemErro =
            'Os dados informados são inválidos. Confira os campos.';
        } else {
          this.mensagemErro =
            'Não foi possível cadastrar o produto. Verifique os dados e tente novamente.';
        }

        this.cdr.detectChanges();
      }
    });
  }

  /**
   * Identifica cada linha da tabela pelo ID do produto.
   */
  trackByProdutoId(index: number, produto: Produto): number {
    return produto.id;
  }

  /**
   * Limpa os campos do formulário.
   */
  private limparFormulario(): void {
    this.codigo = '';
    this.descricao = '';
    this.preco = null;
  }
}
