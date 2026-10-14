import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';

export interface Produto {
  id: number;
  codigo: string;
  descricao: string;
  preco: number;
  status: string;
  estoque?: {
    quantidade: number;
  } | null;
}

export interface CriarProdutoRequest {
  codigo: string;
  descricao: string;
  preco: number;
}

export interface CriarProdutoResponse {
  id: number;
  mensagem: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProdutoService {
  private readonly http = inject(HttpClient);
  private readonly platformId = inject(PLATFORM_ID);
  private readonly apiUrl = 'http://localhost:5072/api/produtos';

  listar(): Observable<Produto[]> {
    if (!isPlatformBrowser(this.platformId)) {
      return of([]);
    }

    return this.http.get<Produto[]>(this.apiUrl);
  }

  criar(
    produto: CriarProdutoRequest
  ): Observable<CriarProdutoResponse> {
    return this.http.post<CriarProdutoResponse>(
      this.apiUrl,
      produto
    );
  }
}