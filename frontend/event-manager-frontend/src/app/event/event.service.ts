import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

export interface EventDto {
  id: string;
  title: string;
  description?: string;
  startDateTime: string;
  location: string;
  capacity?: number;
  visibility: boolean;
  creatorId: string;
  creatorEmail: string;
  participantCount: number;
}

export interface CalendarEventDto {
  id: string;
  title: string;
  start: string;
  isCreator: boolean;
}

export interface EventCreateDto {
  title: string;
  description?: string;
  startDateTime: string;
  location: string;
  capacity?: number;
  visibility: boolean;
}

export interface EventUpdateDto {
  title?: string;
  description?: string;
  startDateTime?: string;
  location?: string;
  capacity?: number;
  visibility?: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class EventService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getPublicEvents(): Observable<EventDto[]> {
    return this.http.get<EventDto[]>(`${this.apiUrl}/events`);
  }

  getEventById(id: string): Observable<EventDto> {
    return this.http.get<EventDto>(`${this.apiUrl}/events/${id}`);
  }

  joinEvent(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/events/${id}/join`, {});
  }

  leaveEvent(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/events/${id}/leave`, {});
  }

  getUserEvents(): Observable<CalendarEventDto[]> {
    return this.http.get<CalendarEventDto[]>(`${this.apiUrl}/users/me/events`);
  }

  createEvent(dto: EventCreateDto): Observable<EventDto> {
    return this.http.post<EventDto>(`${this.apiUrl}/events`, dto);
  }

  updateEvent(id: string, dto: EventUpdateDto): Observable<EventDto> {
    return this.http.patch<EventDto>(`${this.apiUrl}/events/${id}`, dto);
  }

  deleteEvent(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/events/${id}`);
  }
}
