import { HttpClient, HttpInterceptorFn } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginRequest } from './models/auth/login-request.model';
import { LoginResponse } from './models/auth/login-response.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly tokenKey = 'reviews-assistant-token';

  readonly token = () => localStorage.getItem(this.tokenKey);

  login(request: LoginRequest) {
    return this.http
      .post<LoginResponse>(`${environment.apiBaseUrl}/api/auth/login`, request)
      .pipe(tap((response) => this.setToken(response.token)));
  }

  setToken(token: string) {
    localStorage.setItem(this.tokenKey, token);
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
  }
}

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const token = inject(AuthService).token();
  return next(
    token ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : request,
  );
};

export const adminGuard: CanActivateFn = () =>
  inject(AuthService).token() ? true : inject(Router).createUrlTree(['/login']);
