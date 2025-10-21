import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { Observable, combineLatest } from 'rxjs';
import { map } from 'rxjs/operators';
import { EventService, EventDto, CalendarEventDto } from '../event.service';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-events-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatSnackBarModule,
    MatIconModule,
  ],
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.scss'],
})
export class EventsListComponent implements OnInit {
  Number = Number;
  events$!: Observable<EventDto[]>;
  userEvents$: Observable<CalendarEventDto[]> | null = null;
  isAuthenticated: boolean;

  constructor(
    private eventService: EventService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {
    this.isAuthenticated = this.authService.isAuthenticated();
  }

  ngOnInit() {
    this.events$ = this.eventService.getPublicEvents();
    if (this.isAuthenticated) {
      this.userEvents$ = this.eventService.getUserEvents();
    }
  }

  joinEvent(id: string) {
    this.eventService.joinEvent(id).subscribe({
      next: () => {
        this.snackBar.open('Joined event successfully!', 'Close', {
          duration: 3000,
        });
        this.userEvents$ = this.eventService.getUserEvents();
      },
      error: (error) =>
        this.snackBar.open(
          error.error?.error || 'Failed to join event',
          'Close',
          { duration: 3000 }
        ),
    });
  }

  leaveEvent(id: string) {
    this.eventService.leaveEvent(id).subscribe({
      next: () => {
        this.snackBar.open('Left event successfully!', 'Close', {
          duration: 3000,
        });
        this.userEvents$ = this.eventService.getUserEvents();
      },
      error: (error) =>
        this.snackBar.open(
          error.error?.error || 'Failed to leave event',
          'Close',
          { duration: 3000 }
        ),
    });
  }

  isUserParticipant(eventId: string, userEvents: CalendarEventDto[]): boolean {
    return userEvents.some((e) => e.id === eventId && !e.isCreator);
  }
}
