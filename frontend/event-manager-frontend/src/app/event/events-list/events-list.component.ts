import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Navigation, Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Observable, BehaviorSubject, of, Subject, concat, timer } from 'rxjs';
import { switchMap, catchError, tap, map, startWith } from 'rxjs/operators';
import { EventService, EventDto, CalendarEventDto } from '../event.service';
import { AuthService } from '../../auth/auth.service';

interface EventAction {
  type: 'join' | 'leave';
  id: string;
}

interface EventActionMessage {
  success: boolean;
  message: string;
}

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
  submitResult$!: Observable<EventActionMessage | null>;
  temporaryMessage$: Observable<string | null> = of(null);
  isAuthenticated: boolean;

  private refreshUserEvents$ = new BehaviorSubject<void>(undefined);
  private refreshEvents$ = new BehaviorSubject<void>(undefined);
  private actionTrigger$ = new Subject<EventAction>();

  constructor(
    private eventService: EventService,
    private authService: AuthService,
    private router: Router
  ) {
    this.isAuthenticated = this.authService.isAuthenticated();
    const nav = this.router.getCurrentNavigation();
    const stateMessage = nav?.extras?.state?.['message'] || null;
    this.temporaryMessage$ = stateMessage
      ? concat(of(stateMessage), timer(3000).pipe(map(() => null)))
      : of(null);
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
    this.submitResult$ = this.actionTrigger$.pipe(
      switchMap(({ type, id }) => {
        const action$ =
          type === 'join'
            ? this.eventService.joinEvent(id)
            : this.eventService.leaveEvent(id);
        return action$.pipe(
          tap(() => {
            this.refreshUserEvents$.next();
            this.refreshEvents$.next();
          }),
          map(() => ({
            success: true,
            message:
              type === 'join'
                ? 'Joined event successfully!'
                : 'Left event successfully!',
          })),
          catchError((err) =>
            of({
              success: false,
              message:
                err.error?.error ||
                (type === 'join'
                  ? 'Failed to join event'
                  : 'Failed to leave event'),
            })
          )
        );
      }),
      startWith(null)
    );
  }

  joinEvent(id: string) {
    this.actionTrigger$.next({ type: 'join', id });
  }

  leaveEvent(id: string) {
    this.actionTrigger$.next({ type: 'leave', id });
  }

  isUserParticipant(eventId: string, userEvents: CalendarEventDto[]): boolean {
    return userEvents.some((e) => e.id === eventId && !e.isCreator);
  }
}
