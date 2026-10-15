
import { CommonModule, isPlatformBrowser } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  OnInit,
  PLATFORM_ID,
  inject
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  Fornecedor,
  FornecedorService
} from '../../../core/fornecedor.service';

@Component({
  selector: 'app-fornecedores',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './fornecedores.html',
  styleUrl: './fornecedores.scss'
})
export class Fornecedores implements OnInit {
  private readonly fornecedorService = inject(FornecedorService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly platformId = inject(PLATFORM_ID);

  fornecedores: Fornecedor[] = [];
  busca = '';
  carregando = false;
  mensagemErro = '';

  ngOnInit(): void {
    // Evita chamar a API durante a renderização no servidor.
    if (isPlatformBrowser(this.platformId)) {
      this.carregarFornecedores();
    }
  }

  get fornecedoresFiltrados(): Fornecedor[] {
    const termo = this.busca.trim().toLocaleLowerCase('pt-BR');

    if (!termo) {
      return this.fornecedores;
    }

    return this.fornecedores.filter(fornecedor =>
      (fornecedor.razaoSocial ?? '')
        .toLocaleLowerCase('pt-BR')
        .includes(termo) ||
      (fornecedor.cnpj ?? '').includes(termo)
    );
  }

  carregarFornecedores(): void {
    this.carregando = true;
    this.mensagemErro = '';

    this.fornecedorService.listar().subscribe({
      next: fornecedores => {
        this.fornecedores = fornecedores;
        this.carregando = false;
        this.cdr.detectChanges();
      },
      error: (erro: { status?: number }) => {
        this.carregando = false;

        this.mensagemErro = erro.status === 401
          ? 'Sua sessão expirou. Entre novamente no sistema.'
          : 'Não foi possível carregar os fornecedores. Verifique a API.';

        this.cdr.detectChanges();
      }
    });
  }

  trackByFornecedorId(
    index: number,
    fornecedor: Fornecedor
  ): number {
    return fornecedor.id;
  }
}
