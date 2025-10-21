import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({ providedIn: 'root' })
export class SafeJwtHelperService {
  private helper: JwtHelperService | null = null;
  private platformId = inject(PLATFORM_ID);

  constructor() {
    if (isPlatformBrowser(this.platformId)) {
      this.helper = new JwtHelperService();
    }
  }

  isTokenExpired(token: string): boolean {
    if (!this.helper) return true;
    return this.helper.isTokenExpired(token);
  }

  decodeToken(token: string): any {
    if (!this.helper) return null;
    return this.helper.decodeToken(token);
  }
}
