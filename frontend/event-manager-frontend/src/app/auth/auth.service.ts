import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../environments/environment';
import { SafeJwtHelperService } from '../shared/safe-jwt-helper.service';

export interface RegisterResponse {
  message: string;
}

export interface LoginResponse {
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

  private authSubject: BehaviorSubject<boolean>;
  isAuthenticated$: Observable<boolean>;

  constructor(
    private http: HttpClient,
    private jwtHelper: SafeJwtHelperService
  ) {
    this.authSubject = new BehaviorSubject<boolean>(this.isAuthenticated());
    this.isAuthenticated$ = this.authSubject.asObservable();
  }

  register(data: AuthRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(
      `${this.apiUrl}/auth/register`,
      data
    );
  }

  login(data: AuthRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.apiUrl}/auth/login`, data)
      .pipe(
        tap((response) => {
          this.setToken(response.token);
          this.authSubject.next(true);
        })
      );
  }

  logout(): void {
    if (typeof window === 'undefined') return;
    localStorage.removeItem('token');
    this.authSubject.next(false);
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
