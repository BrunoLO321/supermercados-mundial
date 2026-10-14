
import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CanActivateFn, Router } from '@angular/router';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);
  const platformId = inject(PLATFORM_ID);

  // Durante o SSR, o localStorage não existe.
  // A validação real será feita no navegador.
  if (!isPlatformBrowser(platformId)) {
    return true;
  }

  const token = localStorage.getItem('mundial_access_token');

  if (token) {
    return true;
  }

  return router.createUrlTree(['/login']);
};
