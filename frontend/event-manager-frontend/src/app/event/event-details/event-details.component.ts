import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import {
  BehaviorSubject,
  catchError,
  Observable,
  of,
  switchMap,
  tap,
} from 'rxjs';
import { EventService, EventDto, CalendarEventDto } from '../event.service';
import { AuthService } from '../../auth/auth.service';
import { ConfirmationDialogComponent } from '../../shared/confirmation-dialog/confirmation-dialog.component';
import { MatDialog } from '@angular/material/dialog';

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
  message$ = new BehaviorSubject<string | null>(null);
  isAuthenticated: boolean;
  userId: string | null;

  private refreshUserEvents$ = new BehaviorSubject<void>(undefined);
  private refreshEvents$ = new BehaviorSubject<void>(undefined);

  constructor(
    private eventService: EventService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog
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

  deleteEvent(id: string) {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '400px',
      data: {
        title: 'Delete Event',
        message:
          'Are you sure you want to delete this event? This action cannot be undone.',
      },
    });

    dialogRef
      .afterClosed()
      .pipe(
        switchMap((result) => {
          if (!result) return of(null);
          return this.eventService.deleteEvent(id).pipe(
            tap(() => this.message$.next('Event deleted successfully!')),
            tap(() =>
              setTimeout(() => this.router.navigate(['/events']), 2000)
            ),
            catchError((err) => {
              this.message$.next(err.error?.error || 'Failed to delete event');
              return of(null);
            })
          );
        })
      )
      .subscribe();
  }

  isUserParticipant(eventId: string, userEvents: CalendarEventDto[]): boolean {
    return userEvents.some((e) => e.id === eventId && !e.isCreator);
  }
}
