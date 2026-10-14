
import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const router = inject(Router);

  const isLoginRequest = request.url.endsWith('/api/auth/login');

  const token =
    typeof localStorage !== 'undefined'
      ? localStorage.getItem('mundial_access_token')
      : null;

  const authenticatedRequest =
    token && !isLoginRequest
      ? request.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`
          }
        })
      : request;

  return next(authenticatedRequest).pipe(
    catchError((error: unknown) => {
      if (
        error instanceof HttpErrorResponse &&
        error.status === 401 &&
        !isLoginRequest
      ) {
        if (typeof localStorage !== 'undefined') {
          localStorage.removeItem('mundial_access_token');
        }

        void router.navigate(['/login']);
      }

      return throwError(() => error);
    })
  );
};
