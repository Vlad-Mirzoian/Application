import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { Observable } from 'rxjs';
import { EventService, EventDto, CalendarEventDto } from '../event.service';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-event-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatSnackBarModule,
    MatIconModule,
  ],
  templateUrl: './event-details.component.html',
  styleUrls: ['./event-details.component.scss'],
})
export class EventDetailsComponent implements OnInit {
  Number = Number;
  event$!: Observable<EventDto>;
  userEvents$!: Observable<CalendarEventDto[]>;
  isAuthenticated: boolean;
  userId: string | null;

  constructor(
    private eventService: EventService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {
    this.isAuthenticated = this.authService.isAuthenticated();
    this.userId = this.authService.getUserId();
  }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.event$ = this.eventService.getEventById(id);
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
