import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import {
  BehaviorSubject,
  catchError,
  EMPTY,
  map,
  Observable,
  of,
  startWith,
  Subject,
  switchMap,
  tap,
} from 'rxjs';
import { EventService, EventDto, CalendarEventDto } from '../event.service';
import { AuthService } from '../../auth/auth.service';

interface EventAction {
  type: 'join' | 'leave' | 'delete';
  id: string;
}

interface EventActionMessage {
  success: boolean;
  message: string;
}

@Component({
  selector: 'app-event-details',
  standalone: true,
  imports: [CommonModule, RouterModule, MatButtonModule, MatIconModule],
  templateUrl: './event-details.component.html',
  styleUrls: ['./event-details.component.scss'],
})
export class EventDetailsComponent implements OnInit {
  Number = Number;
  event$!: Observable<EventDto>;
  userEvents$!: Observable<CalendarEventDto[]>;
  submitResult$!: Observable<EventActionMessage | null>;
  isAuthenticated: boolean;
  userId: string | null;

  private refreshUserEvents$ = new BehaviorSubject<void>(undefined);
  private refreshEvents$ = new BehaviorSubject<void>(undefined);
  private actionTrigger$ = new Subject<EventAction>();

  constructor(
    private eventService: EventService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.isAuthenticated = this.authService.isAuthenticated();
    this.userId = this.authService.getUserId();
  }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.event$ = this.refreshEvents$.pipe(
      switchMap(() => this.eventService.getEventById(id))
    );
    if (this.isAuthenticated) {
      this.userEvents$ = this.refreshUserEvents$.pipe(
        switchMap(() => this.eventService.getUserEvents())
      );
    }
    this.submitResult$ = this.actionTrigger$.pipe(
      switchMap(({ type, id }) => {
        if (type === 'delete') {
          return this.eventService.deleteEvent(id).pipe(
            tap(() => {
              this.refreshUserEvents$.next();
              this.refreshEvents$.next();
              this.router.navigate(['/events'], {
                state: {
                  message: 'Event deleted successfully!',
                },
              });
            }),
            switchMap(() => EMPTY),
            catchError((err) =>
              of({
                success: false,
                message: err.error?.error || 'Failed to delete event',
              })
            )
          );
        }
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

  deleteEvent(id: string) {
    this.actionTrigger$.next({ type: 'delete', id });
  }

  isUserParticipant(eventId: string, userEvents: CalendarEventDto[]): boolean {
    return userEvents.some((e) => e.id === eventId && !e.isCreator);
  }
}
