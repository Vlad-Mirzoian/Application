import { Component, HostListener } from '@angular/core';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { NavbarComponent } from './navbar/navbar.component';
import { filter, Observable } from 'rxjs';
import { CommonModule, NgIf } from '@angular/common';
import { AiSidebarComponent } from './ai-sidebar/ai-sidebar.component';
import { AuthService } from './auth/auth.service';
import { Store } from '@ngrx/store';
import { AppState } from './ai-sidebar/store';
import { selectIsSidebarOpen } from './ai-sidebar/store/sidebar.selectors';
import { toggleSidebar } from './ai-sidebar/store/sidebar.actions';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, NavbarComponent, AiSidebarComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  showNavbar = true;
  showAiSidebar = true;
  isSidebarOpen$!: Observable<boolean>;

  constructor(
    private router: Router,
    private authService: AuthService,
    private store: Store<AppState>
  ) {
    this.isSidebarOpen$ = this.store.select(selectIsSidebarOpen);

    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        const isAuthPage = event.url.startsWith('/auth');
        const isLoggedIn = this.authService.isAuthenticated();
        this.showNavbar = !isAuthPage;
        this.showAiSidebar = !isAuthPage && isLoggedIn;
        if (!this.showAiSidebar) {
          this.store.dispatch(toggleSidebar());
        }
      });
  }

  toggleSidebar() {
    this.store.dispatch(toggleSidebar());
  }
}
