import { Store } from '@ngrx/store';
import { toggleSidebar } from '../ai-sidebar/store/sidebar.actions';
import { selectIsSidebarOpen } from '../ai-sidebar/store/sidebar.selectors';
import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { Observable } from 'rxjs';
import { AppState } from '../ai-sidebar/store';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent {
  isAuthenticated$!: Observable<boolean>;
  isMenuOpen = false;
  isSidebarOpen$!: Observable<boolean>;

  constructor(
    private authService: AuthService,
    private router: Router,
    private store: Store<AppState>
  ) {
    this.isAuthenticated$ = this.authService.isAuthenticated$;
    this.isSidebarOpen$ = this.store.select(selectIsSidebarOpen);
  }

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  closeMenu() {
    this.isMenuOpen = false;
  }

  toggleSidebar() {
    this.store.dispatch(toggleSidebar());
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
    this.closeMenu();
  }

  @ViewChild('dropdownRef') dropdownRef!: ElementRef;

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.dropdownRef.nativeElement.contains(event.target)) {
      this.closeMenu();
    }
  }
}
