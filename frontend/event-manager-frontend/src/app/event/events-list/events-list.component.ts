import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Observable, BehaviorSubject, of, combineLatest } from 'rxjs';
import { switchMap, catchError, tap, map } from 'rxjs/operators';
import { EventService, EventDto, CalendarEventDto } from '../event.service';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-events-list',
  standalone: true,
  imports: [CommonModule, RouterModule, MatButtonModule, MatIconModule],
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.scss'],
})
export class EventsListComponent implements OnInit {
  Number = Number;
  events$!: Observable<EventDto[]>;
  userEvents$!: Observable<CalendarEventDto[]>;
  message$ = new BehaviorSubject<string | null>(null);
  isAuthenticated: boolean;

  private refreshUserEvents$ = new BehaviorSubject<void>(undefined);
  private refreshEvents$ = new BehaviorSubject<void>(undefined);

  constructor(
    private eventService: EventService,
    private authService: AuthService
  ) {
    this.isAuthenticated = this.authService.isAuthenticated();
  }

  ngOnInit() {
    this.events$ = this.refreshEvents$.pipe(
      switchMap(() => this.eventService.getPublicEvents())
    );
    if (this.isAuthenticated) {
      this.userEvents$ = this.refreshUserEvents$.pipe(
        switchMap(() => this.eventService.getUserEvents())
      );
    }
  }

  joinEvent(id: string) {
    this.eventService
      .joinEvent(id)
      .pipe(
        tap(() => this.message$.next('Joined event successfully!')),
        tap(() => this.refreshUserEvents$.next()),
        tap(() => this.refreshEvents$.next()),
        switchMap(() => {
          this.refreshUserEvents$.next();
          return of(null);
        }),
        catchError((err) => {
          this.message$.next(err.error?.error || 'Failed to join event');
          return of(null);
        })
      )
      .subscribe();
  }

  leaveEvent(id: string) {
    this.eventService
      .leaveEvent(id)
      .pipe(
        tap(() => this.message$.next('Left event successfully!')),
        tap(() => this.refreshUserEvents$.next()),
        tap(() => this.refreshEvents$.next()),
        switchMap(() => {
          this.refreshUserEvents$.next();
          return of(null);
        }),
        catchError((err) => {
          this.message$.next(err.error?.error || 'Failed to leave event');
          return of(null);
        })
      )
      .subscribe();
  }

  isUserParticipant(eventId: string, userEvents: CalendarEventDto[]): boolean {
    return userEvents.some((e) => e.id === eventId && !e.isCreator);
  }
}
