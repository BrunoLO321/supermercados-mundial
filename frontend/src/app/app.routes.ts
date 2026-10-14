
import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Dashboard } from './dashboard';
import { Products } from './features/products/products/products';
import { authGuard } from './core/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    component: Login
  },
  {
    path: 'dashboard',
    component: Dashboard,
    canActivate: [authGuard]
  },
  {
    path: 'produtos',
    component: Products,
    canActivate: [authGuard]
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'dashboard'
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
