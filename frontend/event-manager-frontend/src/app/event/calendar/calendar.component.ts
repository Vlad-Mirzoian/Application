import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { CalendarOptions } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import { FullCalendarModule } from '@fullcalendar/angular';
import { EventService, CalendarEventDto } from '../event.service';
import {
  BehaviorSubject,
  catchError,
  concat,
  map,
  Observable,
  of,
  timer,
} from 'rxjs';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule, RouterModule, FullCalendarModule],
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss'],
})
export class CalendarComponent implements OnInit {
  calendarOptions: CalendarOptions = {
    plugins: [dayGridPlugin, timeGridPlugin],
    initialView: window.innerWidth < 640 ? 'dayGridWeek' : 'dayGridMonth',
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,dayGridWeek',
    },
    events: [],
    eventClick: this.handleEventClick.bind(this),
    height: 'auto',
    eventTimeFormat: {
      hour: '2-digit',
      minute: '2-digit',
      hour12: false,
    },
  };

  events$!: Observable<CalendarEventDto[]>;
  errorMessage$ = new BehaviorSubject<string | null>(null);
  temporaryMessage$: Observable<string | null> = of(null);

  constructor(private eventService: EventService, private router: Router) {
    const nav = this.router.getCurrentNavigation();
    const stateMessage = nav?.extras?.state?.['message'] || null;
    this.temporaryMessage$ = stateMessage
      ? concat(of(stateMessage), timer(3000).pipe(map(() => null)))
      : of(null);
  }

  ngOnInit() {
    this.events$ = this.eventService.getUserEvents().pipe(
      map((events: CalendarEventDto[]) => {
        this.calendarOptions = {
          ...this.calendarOptions,
          events: events.map((event) => ({
            id: event.id,
            title: event.title,
            start: event.start,
            classNames: [event.isCreator ? 'creator' : 'participant'],
          })),
        };
        return events;
      }),
      catchError((error) => {
        this.errorMessage$.next(error.error?.error || 'Failed to load events');
        return of([]);
      })
    );
  }

  handleEventClick(arg: any) {
    window.location.href = `/events/${arg.event.id}`;
  }
}
