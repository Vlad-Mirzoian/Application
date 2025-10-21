import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from '../environments/environment';
import { SafeJwtHelperService } from '../shared/safe-jwt-helper.service';

export interface AuthResponse {
  token: string;
}

export interface AuthRequest {
  email: string;
  password: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient, private jwtHelper: SafeJwtHelperService) {}

  register(data: AuthRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/auth/register`, data)
      .pipe(tap((response) => this.setToken(response.token)));
  }

  login(data: AuthRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/auth/login`, data)
      .pipe(tap((response) => this.setToken(response.token)));
  }

  logout(): void {
    if (typeof window === 'undefined') return;
    localStorage.removeItem('token');
  }

  isAuthenticated(): boolean {
    if (typeof window === 'undefined') return false;
    const token = localStorage.getItem('token');
    return !!token && !this.jwtHelper.isTokenExpired(token);
  }

  getUserId(): string | null {
    if (typeof window === 'undefined') return null;
    const token = localStorage.getItem('token');
    if (token) {
      const decoded = this.jwtHelper.decodeToken(token);
      return decoded[
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
      ];
    }
    return null;
  }

  private setToken(token: string): void {
    if (typeof window === 'undefined') return;
    localStorage.setItem('token', token);
  }
}
