import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { EventsListComponent } from './event/events-list/events-list.component';
import { EventDetailsComponent } from './event/event-details/event-details.component';
import { AuthGuard } from './auth/auth.guard';

export const routes: Routes = [
  { path: 'auth/login', component: LoginComponent },
  { path: 'auth/register', component: RegisterComponent },
  { path: 'auth', redirectTo: 'auth/login', pathMatch: 'full' },
  { path: 'events', component: EventsListComponent },
  {
    path: 'events/:id',
    component: EventDetailsComponent,
  },
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
];
