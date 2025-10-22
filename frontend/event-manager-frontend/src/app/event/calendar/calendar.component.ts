import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { CalendarOptions } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridWeek from '@fullcalendar/timegrid';
import { FullCalendarModule } from '@fullcalendar/angular';
import { EventService, CalendarEventDto } from '../event.service';
import { BehaviorSubject, catchError, map, Observable, of } from 'rxjs';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule, RouterModule, MatButtonModule, FullCalendarModule],
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss'],
})
export class CalendarComponent implements OnInit {
  calendarOptions: CalendarOptions = {
    plugins: [dayGridPlugin, timeGridWeek],
    initialView: 'dayGridMonth',
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek',
    },
    events: [],
    eventClick: this.handleEventClick.bind(this),
    height: 'auto',
    eventTimeFormat: {
      hour: 'numeric',
      minute: '2-digit',
      meridiem: 'short',
    },
  };

  events$!: Observable<CalendarEventDto[]>;
  message$ = new BehaviorSubject<string | null>(null);

  constructor(private eventService: EventService) {}

  ngOnInit() {
    this.events$ = this.eventService.getUserEvents().pipe(
      map((events: CalendarEventDto[]) => {
        this.calendarOptions = {
          ...this.calendarOptions,
          events: events.map((event) => ({
            id: event.id,
            title: event.title,
            start: event.start,
            backgroundColor: event.isCreator ? '#2563eb' : '#16a34a',
            borderColor: event.isCreator ? '#2563eb' : '#16a34a',
          })),
        };
        return events;
      }),
      catchError((error) => {
        this.message$.next(error.error?.error || 'Failed to load events');
        return of([]);
      })
    );
  }

  handleEventClick(arg: any) {
    window.location.href = `/events/${arg.event.id}`;
  }
}
